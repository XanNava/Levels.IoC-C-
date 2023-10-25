
namespace Levels.Universal {

	using System;
	using System.Collections.Generic;

	using UnityEngine;
	using UnityEngine.Playables;

	using Universal.General;

	using static Levels.Universal.Events;

	public interface INotificationBoard<NOTFIFICATION, HANDLER> : INotificationReceiver
		where HANDLER : INotificationHandler
		where NOTFIFICATION : UnityEngine.Playables.INotification {

		void Publish(INotification notice);
		void Publish(Playable origin, PropertyName filter, NOTFIFICATION notification, object package);

		void Subscribe(Type recieveType, HANDLER handler);

		void Unsubscribe(Type recieveType, HANDLER handler);
	}

	[Settings.DI(RegisterNotificationReceiver = false, AutoInject = false)]
	public class NotificationBoard : INotificationBoard<INotification, INotificationHandler> {
		public string Name;
		public readonly List<INotification> Notifications = new List<INotification>();
		public readonly Dictionary<Type, List<INotificationHandler>> Handlers = new Dictionary<Type, List<INotificationHandler>>();

		/// <summary>
		/// Call this to send a default notification, usually used for interaction from the Playable API.
		/// </summary>
		/// <param name="origin"></param>
		/// <param name="filter"></param>
		/// <param name="notification"></param>
		/// <param name="context"></param>
		public void Publish(
			Playable origin,
			PropertyName filter,
			INotification notification,
			object context) {
			INotification newNotification = new Notification().Setup((origin, filter, context));

			Publish(newNotification);
		}

		public void Publish(
			INotification notice) {
			Type key = notice.GetType();

			Notifications.Add(notice);

			if (!Handlers.ContainsKey(key)) {
				Debug.Log($"$[BOARD_PUBLISH][FAILED]\n|>[Board='{Name}'][{key}]\n|?[ContainsHandlerForKey='{Handlers.ContainsKey(key)}']#");

				return;
			}

			Debug.Log($"$[BOARD_PUBLISH][SUCCESS]\n|>[Board='{Name}'][{key}]#");

			foreach (int i in Handlers[key].Count) {
				List<INotificationHandler> handlers = Handlers[key];
				handlers[i].Notify(notice, notice.Filter);
			}
		}

		public void Subscribe(
			Type notificationType,
			INotificationHandler handler) {

			Debug.Log($"$[BOARD_SUBSCRIBE]\n|>[Board='{Name}'][{notificationType}]#");

			if (Handlers.ContainsKey(notificationType)) {
				Handlers[notificationType].Add(handler);

				return;
			}

			Handlers.Add(notificationType, new List<INotificationHandler>() { handler });
		}

		public void Unsubscribe(
			Type notificationType,
			INotificationHandler handler) {

			Debug.Log($"$[BOARD_UNSUBSCRIBE]\n|>[Board='{Name}'][{notificationType}]");

			if (Handlers.ContainsKey(notificationType))
				Handlers[notificationType].Remove(handler);
		}

		public void OnNotify(
			Playable origin,
			UnityEngine.Playables.INotification notification,
			object context) {

			Publish(origin, notification.id, (INotification)notification, context);
		}
	}
}