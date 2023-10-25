namespace Levels.Universal {
    using System;
    using Levels.Universal.Functional;

    using UnityEngine;
    using UnityEngine.Playables;

    public static partial class Events {
        public interface INotificationHandler : INotificationReceiver {
            public Type GetRecieveType();

            public void SubscribeReciever(INotificationReceiver Sub, PropertyName filter);
            public void SubscribeChecker(INotificationChecker Sub, PropertyName filter);

            public void UnsubscribeReciever(INotificationReceiver Sub, PropertyName filter);
            public void UnsubscribeChecker(INotificationChecker Sub, PropertyName filter);
            public void UnsubscribeAll(PropertyName filter);

            void Notify(INotification notification, PropertyName filter);
        }

        public sealed class NotificationHandler : INotificationHandler, ISetup<NotificationHandler, Type> {
            public Type GetRecieveType() => _recieveType;
            private Type _recieveType;

            private readonly NotificationSubscribersDictionary _subscribers = new();

            public void SubscribeReciever(INotificationReceiver receiver, PropertyName filter) {
                _subscribers.SubscribeReciever(receiver, filter);
            }

            public void SubscribeChecker(INotificationChecker checker, PropertyName filter) {
                _subscribers.SubscribeChecker(checker, filter);
            }

            public void UnsubscribeReciever(INotificationReceiver receiver, PropertyName filter) {
                _subscribers.UnsubscribeReciever(receiver, filter);
            }


            public void UnsubscribeChecker(INotificationChecker checker, PropertyName filter) {
                _subscribers.UnsubscribeChecker(checker, filter);
            }

            public void UnsubscribeAll(PropertyName filter) {
                _subscribers.UnsubscribeAll(filter);
            }

            public ISetup<NotificationHandler, Type>
                _Setup { get; set; }

            public NotificationHandler Setup(Type recieveType, NotificationHandler source = null) {
                _recieveType = recieveType;

                return this;
            }

            public void Notify(INotification notification, PropertyName filter) {
                Debug.Log($"[Events.Handler][Notify][Start]\n[{_recieveType}][{filter}]\n[{notification.Readout()}]");
                var subscribers = _subscribers.Subscribers;
                if (!subscribers.ContainsKey(filter)) {
                    return;
                }

                bool cancelToken = false;

                foreach (var checker in subscribers[filter].checkers) {
                    checker.Check(this, notification, out cancelToken);
                }

                if (cancelToken) {
                    Debug.Log($"[Events.Handler][Notify][CancelToken]\n[{_recieveType}][{filter}]\n[{notification.Readout()}]");
                    return;
                }

                foreach (var reciever in subscribers[filter].recievers) {
                    Debug.Log($"[Events.Handler][Notify][Received]\n[{_recieveType}][{filter}]\n[{notification.Readout()}]");

                    reciever.OnNotify(notification.Origin, notification, notification.Context);
                }
            }

            public void OnNotify(Playable origin, UnityEngine.Playables.INotification notification, object context) {
                Notify(new Notification().Setup((origin, notification.id, context)), notification.id);
            }
        }
    }
}
