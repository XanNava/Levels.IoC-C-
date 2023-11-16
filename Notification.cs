using System;


namespace Levels.Universal {
	using ScopeID = String.ID<DI.Scope>;

	public partial class IoC {
		public partial interface INotification {
			public String.ID<INotification> ID {
				get;
			}
		}

		public interface INotification<T> : INotification, IEquatable<INotification<T>> {
			public object Source { get; }
			public T Context { get; }
		}

		public partial struct Notification<T> : INotification<T>, IEquatable<Notification<T>> {
			private String.ID<INotification> _id;
			public readonly String.ID<INotification> ID => _id;

			private readonly object _source;
			public readonly object Source => _source;

			private readonly T _context;
			public readonly T Context => _context;

			public Notification(T context, string id, object source = null) {
				_id = String.ID.Manager<INotification>.Reference.RequestID(id);
				_context = context;
				_source = source;
			}

			public bool Equals(Notification<T> other) {
				return _id == other._id;
			}

			public bool Equals(INotification<T> other) {
				return _id.Equals(other.ID);
			}
		}

		public static partial class Notification {
			public static void NotifyScopeMediator<I>(
				ScopeID scope,
				in I notification)
				where I : INotification, IEquatable<I> {

				Mediator.Collection<I>.NotifyScope(scope, in notification);
			}

			public static void BroadcastNotificationMediators<I>(
				in I notification)
				where I : INotification, IEquatable<I> {

				Mediator.Collection<I>.BroadcastNotification(in notification);
			}
		}
	}
}