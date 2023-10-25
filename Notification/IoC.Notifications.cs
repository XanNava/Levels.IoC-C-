namespace Levels.Universal {

    using System;
    using System.Reflection;

    using global::Serilog;

    using Levels.Universal.General;

    using UnityEngine.Playables;

    using static Levels.Universal.Events;

    // TODO: Move Notification setting instances to a default setting for the scope.
    // TODO: I want this more seperate from Retriaval/Registery/Injection as those are more DI where this is an facade for the board really.
    public sealed partial class IoC {
        public partial interface IScope {
            public interface Notifying : IInject, INotificationReceiver {
                public void SubscribeHandler(INotificationHandler handler, Settings.Notifications settings = null);
                public void UnsubscribeHandler(INotificationHandler handler, Settings.Notifications settings = null);

                public void UnsubscribeReciever(INotificationReceiver notifiable, Settings.Notifications settings = null);
                public void SubscribeReciever(INotificationReceiver notifiable, Settings.Notifications settings = null);

                public void publish(INotification notification, Settings.Notifications settings = null);
            }
        }

        [Settings.DI(AutoInject = false)]
        public partial class ScopeNotificationsDefault : IScope.Notifying, IInject {
            public IScope Scope;

            public void SubscribeHandler(INotificationHandler handler, Settings.Notifications settings = null) {
                if (settings is null)
                    settings = new Settings.Notifications();

                IScope.Requesting retrieval = Scope.Requisition;
                var board = (INotificationBoard<INotification, INotificationHandler>)retrieval.RetrieveAsSingleton(typeof(NotificationBoard), Scope.GetScopeName(), new Settings.DI());

                board.Subscribe(handler.GetRecieveType(), handler);

                if (settings.SubscribeUp && Scope.GetParentScope() != null) {
                    SubscribeHandler(handler, settings);
                }
            }

            public Settings.Notifications GetNotificationSettings(
                INotificationReceiver notifiable) {
                Settings.Notifications settings = new Settings.Notifications();

                Type recieverType = notifiable.GetType();
                MethodInfo methodInfo = recieverType.GetMethod(nameof(notifiable.OnNotify));
                object[] customAttributes = methodInfo.GetCustomAttributes(typeof(Settings.Notifications), false);

                if (customAttributes.Length > 0) {
                    settings = (Settings.Notifications)customAttributes[0];
                }

                settings ??= new Settings.Notifications();

                return settings;
            }

            public void SubscribeReciever(
                INotificationReceiver notifiable,
                Settings.Notifications settings = null) {

                settings ??= GetNotificationSettings(notifiable);

                if (settings.Hooks is null) {
                    Scope.GetLogs().AppendLine("[SUB_RECEIVER][FAILED][NO_HOOKS]");
                    return;
                }

                HookupReciever(notifiable, settings);
            }

            private void HookupReciever(INotificationReceiver notifiable, Settings.Notifications settings) {
                foreach (int i in settings.Hooks.Length) {
                    IScope.Requesting retrieval = Scope.Requisition;
                    Type hook = settings.Hooks[i];
                    var handler = (INotificationHandler)retrieval.Retrieve(typeof(INotificationHandler), hook.ToString());

                    handler.SubscribeReciever(notifiable, settings.Filter);

                    Scope.GetLogs().AppendLine($"[SUB_RECEIVER]\n|>[hook='{hook}'][handler='{handler.GetRecieveType()}']");

                    SubscribeRecieverUp(notifiable, settings);
                }
            }

            private void SubscribeRecieverUp(
                INotificationReceiver notifiable,
                Settings.Notifications settings) {
                IScope parentScope = Scope.GetParentScope();
                IScope.Notifying notifications = parentScope.Notification;

                if (settings.SubscribeUp && parentScope != null) {
                    notifications.SubscribeReciever(notifiable, settings);
                }
            }

            public void UnsubscribeReciever(
                INotificationReceiver notifiable,
                Settings.Notifications settings) {
                var retrieval = Scope.Requisition;

                settings = GetNotificationSettings(notifiable);

                foreach (int i in settings.Hooks.Length) {
                    var handler = (INotificationHandler)retrieval.Retrieve(typeof(INotificationHandler), settings.Hooks[i].ToString());

                    handler.SubscribeReciever(notifiable, settings.Filter);

                    unsubscribeRecieverUp(notifiable, settings);
                }
            }

            private void unsubscribeRecieverUp(
                INotificationReceiver notifiable,
                Settings.Notifications settings) {
                IScope scope = Scope.GetParentScope();

                if (settings.SubscribeUp && scope != null) {
                    IScope.Notifying notifications = scope.Notification;

                    notifications.UnsubscribeReciever(notifiable, settings);
                }
            }

            public void UnsubscribeHandler(
                INotificationHandler handler,
                Settings.Notifications settings = null) {
                IScope parentScope = Scope.GetParentScope();
                settings = GetNotificationSettings(handler);
                var board = Scope.Requisition.Retrieve(
                    typeof(NotificationBoard),
                    "")
                    as NotificationBoard;

                board.Unsubscribe(handler.GetRecieveType(), handler);

                if (settings.SubscribeUp && parentScope != null) {
                    IScope.Notifying parentNotifications = parentScope.Notification;

                    parentNotifications.UnsubscribeHandler(handler, settings);
                }
            }

            public void publish(INotification notification, Settings.Notifications settings = null) {
                settings ??= new Settings.Notifications();

                IScope.Requesting retrieval = Scope.Requisition;
                var board = retrieval.RetrieveAsSingleton(typeof(NotificationBoard), "") as NotificationBoard;

                UnityEngine.Debug.Log($"[NOTIF][{notification.Readout()}]");

                board.Publish(notification);

                if (settings.SubscribeUp && Scope.IsChildScope()) {
                    IScope scope = Scope.GetParentScope();
                    IScope.Notifying notifications = scope.Notification;

                    notifications.publish(notification, settings);
                }
            }

            public void Receive(object[] values) => Scope = (IScope)values[0];

            public (Type, string)[] Request() => new[] { (typeof(IScope), "Current"), (typeof(ILogger), nameof(ScopeNotificationsDefault)) };

            public void OnNotify(Playable origin, UnityEngine.Playables.INotification notification, object context) {
                publish(new Notification().Setup((origin, notification.id, context)));
            }
        }
    }
}