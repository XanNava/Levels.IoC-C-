using System;
using System.Collections.Generic;

namespace Levels.Universal {
	using ScopeID = String.ID<DI.Scope>;
	public partial class IoC {
		public partial class Notification {
			/// <summary>
			/// The Manager/Dispatcher of the notifications.
			/// </summary>
			/// <typeparam name="T"> Type of Notification.</typeparam>
			public struct Mediator<T> : Mediator.Chain.IRespond<T>
				where T : INotification, IEquatable<T> {
				private Reciever.Collection<T> _listeners;

				// TODO: Modify the chain so they can cross comunicate.
				// So each chain can cancel a notification, and determine if it should stop
				// and/or respect other cancel tokens.
				public readonly Mediator.Chain<T> chain;

				public object Source {
					get => this;
				}

				private float _weight;
				public float Weight {
					get => 0;
				}

				public Func<T, Mediator.Chain.CancelToken> Handler {
					get => throw new NotImplementedException();
					set => throw new NotImplementedException();
				}

				public Mediator(float weight = 0, object source = null, Mediator.Chain<T> chain = default) {
					handledLocker = default;
					_weight = weight;
					_listeners = new Reciever.Collection<T>(false);
					this.chain = chain;
				}

				public Mediator.NotifyLock<T> handledLocker;
				public void Notify(
					in T notification) {

					if (!handledLocker.notification.Equals(notification) ||
						!chain.settings.doIgnoreCancels &&
						handledLocker.token.isCancelled) {
						//TODO: Probably return why it cancelled.
						handledLocker = default; // clear the locker for future states.

						return;
					}

					handledLocker = default; // clear the locker for future states.

					_listeners.Notify(in notification);
				}

				public static void NotifyScope(
					ScopeID Scope,
					in T notification) {

					Mediator.Collection<T>.NotifyScope(Scope, in notification);
				}

				public static void BroadcastNotification(
					in T notification) {

					Mediator.Collection<T>.BroadcastNotification(in notification);
				}

				/// <summary>
				/// 
				/// </summary>
				/// <param name="notification"></param>
				/// <param name="token"></param>
				/// <returns>Indecates if it will wait for other chains.</returns>
				public bool Handle(in T notification, out Mediator.Chain.CancelToken token) {
					chain.PushToResponsabilities(notification, out token);

					handledLocker = new Mediator.NotifyLock<T>() { notification = notification, token = token };

					if (chain.settings.doCancleAcrossScope || token.isCancelled) {
						return true;
					}

					Notify(notification);
					return false;
				}

				public void Cancel(T notification) {
					handledLocker = new Mediator.NotifyLock<T>() {
						notification = notification,
						token = new Mediator.Chain.CancelToken() { isCancelled = true }
					};
				}

				public void SubscribeListener(
					in Recieve<T> listener,
					object source) {

					_listeners.AddListener(source, in listener);
				}

				public void UnsubscribeListener(
					object source) {

					_listeners.RemoveListener(source);
				}

				public void SubToChain(
					ref Mediator.Chain.Responce<T> toSub) {

					chain.SubscribeResponsability(ref toSub);
				}

				public void UnsubFromChain(
					ref Mediator.Chain.Responce<T> toUnsud) {

					chain.UnsubscribeResponsability(ref toUnsud);
				}
			}
			public partial class Mediator {
				// TODO: Make this croos thread safe?
				/// <summary>
				/// Used to make sure the notification passes through the chain(CoR) first.
				/// </summary>
				/// <typeparam name="T">The type of Notification this is mediating for.</typeparam>
				public struct NotifyLock<T>
					where T : INotification, IEquatable<T> {

					public T notification;
					public Mediator.Chain.CancelToken token;
				}

				/// <summary>
				/// Contains Notifications
				/// </summary>
				/// <typeparam name="T">Type of Notification this collection Mediats</typeparam>
				public static class Collection<T>
					where T : INotification, IEquatable<T> {

					public static readonly Dictionary<ScopeID, Mediator<T>> collection = new Dictionary<ScopeID, Mediator<T>>();

					public static void BroadcastNotification(
						in T notification) {
						Mediator.Chain.CancelToken tokenScoped = new Mediator.Chain.CancelToken() { isCancelled = false };

						foreach (var key in collection.Keys) {
							Mediator.Chain.CancelToken token;

							collection[key].Handle(in notification, out token);

							if (token.isCancelled) {
								tokenScoped.isCancelled = true;
							}
						}

						if (tokenScoped.isCancelled) {
							foreach (var key in collection.Keys) {
								if (collection[key].chain.settings.doCancleAcrossScope)
									collection[key].Cancel(notification);
							}
						}

						foreach (var key in collection.Keys) {
							collection[key].Notify(in notification);
						}
					}

					public static void NotifyScope(
						ScopeID Scope,
						in T notification) {


						if (collection.ContainsKey(Scope)) {
							collection[Scope].Handler(notification);

							collection[Scope].Notify(in notification);
						}
					}

					public static void SubToChain(
						ScopeID scope,
						ref Mediator.Chain.Responce<T> toSub) {

						if (!collection.ContainsKey(scope))
							return;

						collection[scope].SubToChain(ref toSub);
					}

					public static void UnsubFromChain(
						in ScopeID scope,
						ref Mediator.Chain.Responce<T> toUnsud) {

						if (!collection.ContainsKey(scope))
							return;

						collection[scope].UnsubFromChain(ref toUnsud);
					}

					public static void SubReciever(
						in ScopeID scope,
						ref Recieve<T> listener,
						object source) {

						if (!collection.ContainsKey(scope)) {
							collection.Add(scope, new Mediator<T>());
						}

						collection[scope].SubscribeListener(in listener, source);
					}

					public static void UnsubReciever(
						ScopeID scope,
						object source) {

						if (!collection.ContainsKey(scope)) {
							return;
						}

						collection[scope].UnsubscribeListener(source);
					}

					public static void AddHandler(
						ScopeID scope,
						Mediator<T> Mediator) {

						if (collection.ContainsKey(scope)) {
							return;
						}
						collection.Add(scope, Mediator);
					}

					public static void RemoveHandler(
						ScopeID scope) {

						if (!collection.ContainsKey(scope)) {
							return;
						}

						collection.Remove(scope);
					}

					public static bool HasHandler(
						ScopeID scope) {

						return collection.ContainsKey(scope);
					}
				}
			}
		}
	}

}
