using UnityEngine.Playables;

namespace Levels.Universal.Experimental {
	public struct NotificationHandler<T> : INotificationReceiver {
		public IScope Scope;
		private NotificationListeners _listeners;
		private NotificationCheckers _checkers;

		public NotificationHandler(IScope scope) {
			Scope = scope;
			_listeners = new NotificationListeners(false);
			_checkers = new NotificationCheckers(Scope);
		}

		public void OnNotify(
			Playable origin,
			UnityEngine.Playables.INotification notification,
			object context) {

			bool cancelToken = false;
			_checkers.
				Check(origin, notification, context, out cancelToken);

			if (cancelToken) {
				return;
			}

			_listeners.
				OnNotify(origin, notification, context);
		}

		public void SubscribeListener(
			IScope scope,
			INotificationReceiver listener,
			object source) {

			_listeners.
				AddListener(source, listener);
		}

		public void UnsubscribeListener(
			object source) {

			_listeners.
				RemoveListener(source);
		}

		public void SubChecker(
			object src,
			IAmChecker toSub) {

			_checkers.
				SubChecker(src, toSub);
		}

		public void UnsubChecker(
			object src,
			IAmChecker toUnsud) {

			_checkers.
				UnsubChecker(src, toUnsud);
		}
	}
}
