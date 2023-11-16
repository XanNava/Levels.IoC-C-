using System;
using System.Collections.Generic;

using Levels.Universal.General;

using static Levels.Universal.DI;
using static Levels.Universal.IoC.Notification.Mediator.Chain;

namespace Levels.Universal {
	using ScopeKey = String.ID<DI.Scope>;
	public partial class IoC {
		public partial class Notification {
			public partial class Mediator {
				public struct Chain<T>
					where T : INotification, IEquatable<T> {

					public struct Settings {
						public bool doCancleAcrossScope;
						public bool doIgnoreCancels;
					}

					public Settings settings;

					public DI.IScope Scope;
					// DARN YOU UNITY
					public readonly List<Responce<T>> Responsabilities;

					public Chain(
						DI.IScope scope,
						Settings settings) {

						Scope = scope;
						Responsabilities = new List<Responce<T>>();

						this.settings = settings;
					}

					public void PushToResponsabilities(
						T notification,
						out Chain.CancelToken cancelToken) {
						cancelToken = Chain.CancelToken.False;
						Chain.CancelToken cancel = Chain.CancelToken.False;

						foreach (int i in Responsabilities.Count) {
							Responsabilities[i].Handle(notification, out cancel);

							if (cancel.isCancelled) {
								cancelToken = cancel;
							}
						}
					}

					public void SubscribeResponsability(
						ref Responce<T> toSub) {

						if (Responsabilities.Contains(toSub))
							return;

						Responsabilities.Add(toSub);

						_OrderChain();
					}

					private void _OrderChain() {
						Responsabilities.Sort((a, b) => a.Weight.CompareTo(b.Weight));
					}

					public void UnsubscribeResponsability(
						ref Responce<T> toUnsub) {

						if (!Responsabilities.Contains(toUnsub))
							return;

						Responsabilities.Remove(toUnsub);
					}
				}

				/// <summary>
				/// The conceptual level of Chain of Responsability
				/// </summary>
				public partial class Chain {
					public interface IRespond<T>
						where T : INotification, IEquatable<T> {
						public object Source {
							get;
						}

						public float Weight {
							get;
						}

						public Func<T, CancelToken> Handler {
							get;
						}

						public bool Handle(in T notification, out CancelToken token);
					}

					public struct CancelToken {
						public static CancelToken True = new CancelToken { isCancelled = true };
						public static CancelToken False = new CancelToken { isCancelled = true };

						public bool isCancelled;
					}

					public interface ImResponce<T> : IHaveRegisteries, IRespond<T>, IEquatable<ImResponce<T>> where T : INotification, IEquatable<T> {

					}

					public struct Responce<T> : ImResponce<T> where T : INotification, IEquatable<T> {
						private object _source;
						public object Source => _source;

						private ScopeKey _scope;
						ScopeKey Scope {
							get => _scope;
						}
						ScopeKey IHaveRegisteries.Scope {
							get => _scope;
							set => _scope = value;
						}

						object IHaveRegisteries.Source {
							get => _source;
							set => _source = value;
						}
						object Chain.IRespond<T>.Source {
							get => _source;
						}

						//TODO: What does this do?
						private float _weight;
						public float Weight => _weight;
						float Chain.IRespond<T>.Weight {
							get => _weight;
						}

						private Func<T, CancelToken> _handle;
						public Func<T, CancelToken> Handler {
							get => _handle; set => _handle = value;
						}

						public Responce(float weight = 0, object source = null, Func<T, CancelToken> handle = null) {
							_weight = weight;
							_source = source;
							_handle = handle;
							_scope = default;
						}

						public bool Handle(in T notification, out CancelToken token) {
							token = _handle.Invoke(notification);

							return true;
						}

						public bool Equals(ImResponce<T> other) => throw new NotImplementedException();

						public void Register(in ScopeKey scope, object source) {

							_scope = scope;

							Mediator.Collection<T>.SubToChain(scope, ref this);
						}

						public void Unregister() {
							Mediator.Collection<T>.UnsubFromChain(_scope, ref this);
						}
					}

					public partial class Responce {
						public static class factory<T> where T : INotification, IEquatable<T> {
							public static Responce<T> CreateCancleAndIntercept(Action intercept = null, float weight = -1, object source = null) {
								return new Responce<T>(weight, source, (n) => {
									if (intercept != null) {
										intercept.Invoke();
									}

									return CancelToken.True;
								});
							}

							public static Responce<T> CreateCancleIfAndIntercept(Func<bool> flag, Action intercept = null, float weight = -1, object source = null) {
								return new Responce<T>(weight, source, (n) => {
									if (!flag())
										return CancelToken.False;

									if (intercept != null) {
										intercept.Invoke();
									}

									return CancelToken.True;
								});
							}
						}
					}
				}
			}
		}
	}
}
