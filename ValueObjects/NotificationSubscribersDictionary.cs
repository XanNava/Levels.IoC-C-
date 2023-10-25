namespace Levels.Universal {
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.Playables;

    public class NotificationSubscribersDictionary {
        public readonly Dictionary<PropertyName, (List<INotificationReceiver> recievers, List<INotificationChecker> checkers)> Subscribers = new();

        public void CheckKey(PropertyName filter) {
            if (!Subscribers.ContainsKey(filter)) {
                Subscribers.Add(filter, new(new List<INotificationReceiver>(), new List<INotificationChecker>()));

                return;
            }

            if (Subscribers[filter].checkers is null) {
                Subscribers[filter] = new(Subscribers[filter].recievers, new List<INotificationChecker>());
            }

            if (Subscribers[filter].recievers is null) {
                Subscribers[filter] = new(new List<INotificationReceiver>(), Subscribers[filter].checkers);
            }
        }

        public void SubscribeReciever(INotificationReceiver subscriber, PropertyName filter) {
            CheckKey(filter);

            var recievers = Subscribers[filter].recievers;

            if (recievers.Contains(subscriber)) {
                return;
            }

            recievers.Add(subscriber);
        }

        public void UnsubscribeReciever(INotificationReceiver reciever, PropertyName filter) {
            if (!Subscribers.ContainsKey(filter))
                return;

            var recievers = Subscribers[filter].recievers;

            if (recievers != null)
                recievers.Remove(reciever);
        }

        public void SubscribeChecker(INotificationChecker checker, PropertyName filter) {
            CheckKey(filter);

            var checkers = Subscribers[filter].checkers;

            if (checkers.Contains(checker)) {
                return;
            }

            checkers.Add(checker);
        }

        public void UnsubscribeChecker(INotificationChecker checker, PropertyName filter) {
            if (!Subscribers.ContainsKey(filter))
                return;

            var checkers = Subscribers[filter].checkers;

            if (checkers != null)
                checkers.Remove(checker);
        }

        public void UnsubscribeAll(PropertyName filter) {
            if (!Subscribers.ContainsKey(filter))
                return;

            if (Subscribers[filter].checkers != null) {
                Subscribers[filter].checkers.Clear();
            }

            if (Subscribers[filter].recievers != null) {
                Subscribers[filter].recievers.Clear();
            }

            Subscribers.Remove(filter);
        }
    }
}
