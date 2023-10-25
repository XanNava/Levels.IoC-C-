using System;

using UnityEngine.Playables;

namespace Levels.Universal.Experimental {
	public class NotificationReciever<T> : INotificationReciever<T> where T : INotification {

		public Action<Playable, UnityEngine.Playables.INotification, object> OnNotifiedHook { get; set; }
		public Action OnNotified { get; set; }

		public void OnNotify(Playable origin, UnityEngine.Playables.INotification notification, object context) {
			OnNotifiedHook?.Invoke(origin, notification, context);
			OnNotified?.Invoke();
		}
	}
}
