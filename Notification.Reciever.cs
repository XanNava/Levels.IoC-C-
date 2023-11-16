using System;
using System.Collections.Generic;

namespace Levels.Universal {
	using ScopeID = String.ID<DI.Scope>;

	public partial class IoC {
		public partial class Notification {
			/// <summary>
			/// The interfaces for Listeners.
			/// </summary>
			public interface IRecieve<T>
				 where T : INotification, IEquatable<T> {
				public void Notify(in T notification);
			}

			public interface IReciever<N1> : DI.IHaveRegisteries, IRecieve<N1>, IEquatable<IRecieve<N1>>
				where N1 : INotification, IEquatable<N1> { }

			public struct Recieve<T> : IReciever<T>, IEquatable<Recieve<T>>
				where T : INotification, IEquatable<T> {

				public Func<Recieve<T>> Self;

				public int ID;
				private ScopeID _scope;

				ScopeID Scope {
					get => _scope;
				}
				ScopeID DI.IHaveRegisteries.Scope {
					get => this.Scope;
					set => _scope = value;
				}

				object _source;
				object Source {
					get => _source;
				}
				object DI.IHaveRegisteries.Source {
					get => this.Source;
					set => _source = value;
				}

				public Action<T> OnNotify;

				public bool Equals(IRecieve<T> other) => throw new NotImplementedException();
				public void Notify(in T notification) {
					OnNotify(notification);
				}

				public bool Equals(Recieve<T> other) {
					return Source == other.Source && Scope == other.Scope && OnNotify == other.OnNotify;
				}


				public void Register(
					in ScopeID scope,
					object source) {

					_scope = scope;
					_source = source;
					Mediator.Collection<T>.SubReciever(scope, ref this, source);
				}

				public void Unregister() {
					Mediator.Collection<T>.UnsubReciever(Scope, Source);

				}

				public void Notify<T1>(in T1 notification) => throw new NotImplementedException();
			}

			public interface IReciever<N1, N2> : DI.IHaveRegisteries, IEquatable<IReciever<N1, N2>>
				where N1 : INotification, IEquatable<N1>
				where N2 : INotification, IEquatable<N2> {

				public Recieve<N1> Reciever1 {
					get;
				}
				public Recieve<N2> Reciever2 {
					get;
				}

			}

			public struct Recieve<N1, N2> : IReciever<N1, N2>
				where N1 : INotification, IEquatable<N1>
				where N2 : INotification, IEquatable<N2> {

				public int ID;
				private ScopeID _scope;

				ScopeID Scope {
					get => _scope;
				}
				ScopeID DI.IHaveRegisteries.Scope {
					get => this.Scope;
					set => _scope = value;
				}

				object _source;
				object Source {
					get => _source;
				}
				object DI.IHaveRegisteries.Source {
					get => this.Source;
					set => _source = value;
				}

				public Recieve<N1> Reciever1 {
					get;
				}

				public Recieve<N2> Reciever2 {
					get;
				}

				public void Register(
					in ScopeID scope,
					object source) {

					_scope = scope;
					_source = source;

					Reciever1.Register(scope, source);
					Reciever2.Register(scope, source);
				}

				public void Unregister() {
					Reciever1.Unregister();
					Reciever2.Unregister();
				}

				public bool Equals(IReciever<N1, N2> other) {
					return Reciever1.Equals(other.Reciever1) &&
							Reciever2.Equals(other.Reciever2);
				}
			}

			public interface IReciever<N1, N2, N3> : DI.IHaveRegisteries, IEquatable<IReciever<N1, N2, N3>>
				where N1 : INotification, IEquatable<N1>
				where N2 : INotification, IEquatable<N2>
				where N3 : INotification, IEquatable<N3> {

				public Recieve<N1> Reciever1 {
					get;
				}
				public Recieve<N2> Reciever2 {
					get;
				}
				public Recieve<N3> Reciever3 {
					get;
				}
			}


			public struct Recieve<N1, N2, N3> : IReciever<N1, N2, N3>
				where N1 : INotification, IEquatable<N1>
				where N2 : INotification, IEquatable<N2>
				where N3 : INotification, IEquatable<N3> {

				public int ID;

				private ScopeID _scope;
				ScopeID Scope { get => _scope; }
				ScopeID DI.IHaveRegisteries.Scope { get => this.Scope; set => _scope = value; }
				object _source;

				object Source { get => _source; }
				object DI.IHaveRegisteries.Source { get => this.Source; set => _source = value; }

				public Recieve<N1> Reciever1 { get; }
				public Recieve<N2> Reciever2 { get; }
				public Recieve<N3> Reciever3 { get; }

				public void Register(
					in ScopeID scope,
					object source) {

					_scope = scope;
					_source = source;

					Reciever1.Register(scope, source);
					Reciever2.Register(scope, source);
					Reciever3.Register(scope, source);
				}

				public void Unregister() {
					Reciever1.Unregister();
					Reciever2.Unregister();
					Reciever3.Unregister();
				}

				public bool Equals(IReciever<N1, N2, N3> other) {
					return Reciever1.Equals(other.Reciever1) &&
							Reciever2.Equals(other.Reciever2) &&
							Reciever3.Equals(other.Reciever3);
				}
			}

			public interface IReciever<N1, N2, N3, N4> : DI.IHaveRegisteries, IEquatable<IReciever<N1, N2, N3, N4>>
			where N1 : INotification, IEquatable<N1>
			where N2 : INotification, IEquatable<N2>
			where N3 : INotification, IEquatable<N3>
			where N4 : INotification, IEquatable<N4> {

				public Recieve<N1> Reciever1 { get; }
				public Recieve<N2> Reciever2 { get; }
				public Recieve<N3> Reciever3 { get; }
				public Recieve<N4> Reciever4 { get; }
			}

			public struct Recieve<N1, N2, N3, N4> : IReciever<N1, N2, N3, N4>
				where N1 : INotification, IEquatable<N1>
				where N2 : INotification, IEquatable<N2>
				where N3 : INotification, IEquatable<N3>
				where N4 : INotification, IEquatable<N4> {

				public int ID;

				private ScopeID _scope;
				ScopeID Scope { get => _scope; }
				ScopeID DI.IHaveRegisteries.Scope { get => this.Scope; set => _scope = value; }
				object _source;

				object Source { get => _source; }
				object DI.IHaveRegisteries.Source { get => this.Source; set => _source = value; }

				public Recieve<N1> Reciever1 { get; }
				public Recieve<N2> Reciever2 { get; }
				public Recieve<N3> Reciever3 { get; }
				public Recieve<N4> Reciever4 { get; }

				public void Register(
					in ScopeID scope,
					object source) {

					_scope = scope;
					_source = source;

					Reciever1.Register(scope, source);
					Reciever2.Register(scope, source);
					Reciever3.Register(scope, source);
					Reciever4.Register(scope, source);
				}

				public void Unregister() {
					Reciever1.Unregister();
					Reciever2.Unregister();
					Reciever3.Unregister();
					Reciever4.Unregister();
				}

				public bool Equals(IReciever<N1, N2, N3, N4> other) {
					return Reciever1.Equals(other.Reciever1) &&
							Reciever2.Equals(other.Reciever2) &&
							Reciever3.Equals(other.Reciever3) &&
							Reciever4.Equals(other.Reciever4);
				}
			}

			public interface IReciever<N1, N2, N3, N4, N5> : DI.IHaveRegisteries, IEquatable<IReciever<N1, N2, N3, N4, N5>>
				where N1 : INotification, IEquatable<N1>
				where N2 : INotification, IEquatable<N2>
				where N3 : INotification, IEquatable<N3>
				where N4 : INotification, IEquatable<N4>
				where N5 : INotification, IEquatable<N5> {

				public Recieve<N1> Reciever1 { get; }
				public Recieve<N2> Reciever2 { get; }
				public Recieve<N3> Reciever3 { get; }
				public Recieve<N4> Reciever4 { get; }
				public Recieve<N5> Reciever5 { get; }
			}

			public struct Recieve<N1, N2, N3, N4, N5> : IReciever<N1, N2, N3, N4, N5>
				where N1 : INotification, IEquatable<N1>
				where N2 : INotification, IEquatable<N2>
				where N3 : INotification, IEquatable<N3>
				where N4 : INotification, IEquatable<N4>
				where N5 : INotification, IEquatable<N5> {

				public int id;

				private ScopeID _scope;
				ScopeID Scope { get => _scope; }
				ScopeID DI.IHaveRegisteries.Scope { get => this.Scope; set => _scope = value; }
				object _source;

				object Source { get => _source; }
				object DI.IHaveRegisteries.Source { get => this.Source; set => _source = value; }

				public Recieve<N1> Reciever1 { get; }
				public Recieve<N2> Reciever2 { get; }
				public Recieve<N3> Reciever3 { get; }
				public Recieve<N4> Reciever4 { get; }
				public Recieve<N5> Reciever5 { get; }

				public void Register(
					in ScopeID scope,
					object source) {

					_scope = scope;
					_source = source;

					Reciever1.Register(scope, source);
					Reciever2.Register(scope, source);
					Reciever3.Register(scope, source);
					Reciever4.Register(scope, source);
					Reciever5.Register(scope, source);
				}

				public void Unregister() {
					Reciever1.Unregister();
					Reciever2.Unregister();
					Reciever3.Unregister();
					Reciever4.Unregister();
					Reciever5.Unregister();
				}

				public bool Equals(IReciever<N1, N2, N3, N4, N5> other) {
					return Reciever1.Equals(other.Reciever1) &&
							Reciever2.Equals(other.Reciever2) &&
							Reciever3.Equals(other.Reciever3) &&
							Reciever4.Equals(other.Reciever4) &&
							Reciever5.Equals(other.Reciever5);
				}
			}

			/// <summary>
			/// The static concept of Recievers and thier componensts
			/// </summary>
			public partial class Reciever {
				public struct Collection<T>
					where T : INotification, IEquatable<T> {
					// Stupid C#9.0 SUPID UNITY
					public readonly Dictionary<object, Recieve<T>> Listeners;
					public Collection(bool _) { Listeners = new Dictionary<object, Recieve<T>>(); }

					public void Notify(
						in T notification) {

						foreach (var key in Listeners.Keys) {
							Listeners[key].Notify(in notification);
						}
					}

					public void AddListener(
						object source,
						in Recieve<T> reciever) {

						if (Listeners.ContainsKey(source))
							return;

						Listeners.Add(source, reciever);
					}

					public void RemoveListener(
						object source) {

						if (!Listeners.ContainsKey(source))
							return;

						Listeners.Remove(source);
					}
				}
			}

			public class TestReciever<T>
				where T : INotification, IEquatable<T> {
				private Recieve<T> _test;

				public TestReciever() {
					_test = new Recieve<T>();

					_test.Self = () => {
						return _test;
					};
				}
			}
		}
	}

	public static class Notification_IReciever_Extends {
		/// <summary>
		/// WARNING: Boxing on structs.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static IoC.Notification.IRecieve<T> Interface_IReciever<T>(
			this IoC.Notification.IRecieve<T> instance)
			where T : IoC.INotification, IEquatable<T> {

			return instance;
		}

		/// <summary>
		/// WARNING: Boxing on structs.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static IoC.Notification.IReciever<T, T2> Interface_IReciever<T, T2>(
			this IoC.Notification.IReciever<T, T2> instance)
			where T : IoC.INotification, IEquatable<T>
			where T2 : IoC.INotification, IEquatable<T2> {

			return instance;
		}

		/// <summary>
		/// WARNING: Boxing on structs.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static IoC.Notification.IReciever<T, T2, T3> Interface_IReciever<T, T2, T3>(
			this IoC.Notification.IReciever<T, T2, T3> instance)
			where T : IoC.INotification, IEquatable<T>
			where T2 : IoC.INotification, IEquatable<T2>
			where T3 : IoC.INotification, IEquatable<T3> {

			return instance;
		}

		/// <summary>
		/// WARNING: Boxing on structs.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static IoC.Notification.IReciever<T, T2, T3, T4> Interface_IReciever<T, T2, T3, T4>(
			this IoC.Notification.IReciever<T, T2, T3, T4> instance)
			where T : IoC.INotification, IEquatable<T>
			where T2 : IoC.INotification, IEquatable<T2>
			where T3 : IoC.INotification, IEquatable<T3>
			where T4 : IoC.INotification, IEquatable<T4> {

			return instance;
		}

		/// <summary>
		/// WARNING: Boxing on structs.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static IoC.Notification.IReciever<T, T2, T3, T4, T5> Interface_IReciever<T, T2, T3, T4, T5>(
			this IoC.Notification.IReciever<T, T2, T3, T4, T5> instance)
			where T : IoC.INotification, IEquatable<T>
			where T2 : IoC.INotification, IEquatable<T2>
			where T3 : IoC.INotification, IEquatable<T3>
			where T4 : IoC.INotification, IEquatable<T4>
			where T5 : IoC.INotification, IEquatable<T5> {

			return instance;
		}
	}
}
