using System;
using System.Collections.Generic;

using UnityEngine.Events;
using UnityEngine.Playables;

namespace Levels.Universal.Experimental {
    public interface INotificationsBoard<T> : INotificationReceiver where T : UnityEngine.Playables.INotification {
        public NotificationHandlers<T> Handlers { get; protected set; }

        public void OnNotifyScope(IScope scope, Playable origin, UnityEngine.Playables.INotification notification, object context) {
            Handlers.OnNotifyScope(scope, origin, notification, context);
        }
    }

    public struct NotificationBoard<T> : INotificationsBoard<T> where T : UnityEngine.Playables.INotification {
        public static NotificationBoard<T> Service {
            get {
                if (_service.Equals(default(NotificationBoard<T>)))
                    _service = new NotificationBoard<T>(false);
                return _service;
            }
            set => _service = value;
        }

        private static NotificationBoard<T> _service;
        private NotificationHandlers<T> _handlers;

        public NotificationBoard(bool dummy) : this() {
            _handlers = new NotificationHandlers<T>(dummy);
            Service = this;
        }

        NotificationHandlers<T> INotificationsBoard<T>.Handlers { get => _handlers; set => _handlers = value; }

        /// <summary>
        /// Notifies {<see cref="Handler">} for <see cref="scope"/>
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="origin"></param>
        /// <param name="notification"></param>
        /// <param name="context"></param>
        public void OnNotifyScope(IScope scope, Playable origin, UnityEngine.Playables.INotification notification, object context) {
            _handlers.OnNotifyScope(scope, origin, notification, context);
        }

        /// <summary>
        /// Notifies all handlers of type <see cref="T"/>.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="notification"></param>
        /// <param name="context"></param>
        void INotificationReceiver.OnNotify(Playable origin, UnityEngine.Playables.INotification notification, object context) {


            _handlers.OnNotify(origin, notification, context);
        }

        public void SubChecker(
            IScope scope,
            object src,
            IAmChecker toSub) {

            _handlers.
                SubChecker(scope, src, toSub);
        }

        public void UnsubChecker(
            IScope scope,
            object src,
            IAmChecker toUnsud) {

            _handlers.
                UnsubChecker(scope, src, toUnsud);
        }

        public void SubscribeListener(IScope scope, INotificationReceiver listener, object source) {
            _handlers.SubscribeListener(scope, listener, source);
        }

        public void UnsubscribeListener(IScope scope, INotificationReceiver listener, object source) {
            _handlers.UnsubscribeListener(scope, source);
        }

        public void SubscribeHandler(IScope scope, NotificationHandler<T> handler) {
            _handlers.AddHandler(scope, handler);
        }

        public void UnsubscribeHandler(IScope scope) {
            _handlers.RemoveHandler(scope);
        }

        public void CheckHandler(IScope scope) {
            _handlers.HasHandler(scope);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <note>Might be able to get rid of this.</note>
    /// <typeparam name="T"></typeparam>
    public struct NotificationHandlers<T> : INotificationReceiver {
        public readonly Dictionary<IScope, NotificationHandler<T>> Handlers;

        public NotificationHandlers(
            bool dummy) {// Stupid C# 9.0

            Handlers = new Dictionary<IScope, NotificationHandler<T>>();
        }

        public void OnNotify(
            Playable origin,
            UnityEngine.Playables.INotification notification,
            object context) {

            foreach (var key in Handlers.Keys) {
                Handlers[key].
                    OnNotify(origin, notification, context);
            }
        }

        public void OnNotifyScope(
            IScope Scope,
            Playable origin,
            UnityEngine.Playables.INotification notification,
            object context) {

            if (Handlers.ContainsKey(Scope)) {
                Handlers[Scope].
                    OnNotify(origin, notification, context);
            }
        }

        public void SubChecker(
            IScope scope,
            object src,
            IAmChecker toSub) {

            if (!Handlers.ContainsKey(scope))
                return;

            Handlers[scope].
                SubChecker(src, toSub);
        }

        public void UnsubChecker(
            IScope scope,
            object src,
            IAmChecker toUnsud) {

            if (!Handlers.ContainsKey(scope))
                return;

            Handlers[scope].
                UnsubChecker(src, toUnsud);
        }

        public void SubscribeListener(
            IScope scope,
            INotificationReceiver listener,
            object source) {

            if (!Handlers.ContainsKey(scope)) {
                Handlers.
                    Add(scope, new NotificationHandler<T>(scope));
            }

            Handlers[scope].
                SubscribeListener(scope, listener, source);
        }

        public void UnsubscribeListener(
            IScope scope,
            object source) {

            if (!Handlers.ContainsKey(scope)) {
                return;
            }

            Handlers[scope].
                UnsubscribeListener(source);
        }

        public void AddHandler(
            IScope scope,
            NotificationHandler<T> handler) {

            if (Handlers.ContainsKey(scope)) {
                return;
            }
            Handlers.
                Add(scope, handler);
        }

        public void RemoveHandler(
            IScope scope) {

            if (!Handlers.ContainsKey(scope)) {
                return;
            }

            Handlers.
                Remove(scope);
        }

        public bool HasHandler(
            IScope scope) {

            return Handlers.ContainsKey(scope);
        }
    }

    public interface IAmChecker {
        public object Source { get; set; }
        public float Weight { get; set; }

        Func<Playable, UnityEngine.Playables.INotification, object, CancelToken> Check { get; set; }

        public Func<Playable, UnityEngine.Playables.INotification, object, CancelToken> GetCheckerFunc() {
            return Check;
        }

        void Subscribe(IScope scope);
    }

    public static class IAmChecker_Options {
        public static IAmChecker @CancelNotification(this IAmChecker src, UnityEvent call = null) {
            src.Check = (a, b, c) => {
                if (call != null) {
                    call.Invoke();
                }

                return CancelToken.True;
            };

            return src;
        }

        public static IAmChecker @CancelNotificationIf(this IAmChecker src, Func<bool> flag, UnityEvent call = null) {


            src.Check = (a, b, c) => {
                if (!flag())
                    return CancelToken.False;

                if (call != null) {
                    call.Invoke();
                }

                return CancelToken.True;
            };

            return src;
        }
    }


    public struct CancelToken {
        public static CancelToken True = new CancelToken { Value = true };
        public static CancelToken False = new CancelToken { Value = true };

        public bool Value;
    }

    public static class IAmChecker_Extends {
        public static IAmChecker Interface(this IAmChecker src) => src;
    }

    public interface INotificationChecker<T> : IAmChecker where T : INotification {

        void IAmChecker.Subscribe(IScope scope) {
            NotificationBoard<T>.Service.SubChecker(scope, this, this.Interface());
        }
    }

    public class NotificationChecker<T> : INotificationChecker<T> where T : INotification {
        private object _source;
        public object Source => _source;
        object IAmChecker.Source { get => _source; set => _source = value; }

        //TODO: What does this do?
        private float _weight;
        float IAmChecker.Weight { get => _weight; set => _weight = value; }

        private Func<Playable, UnityEngine.Playables.INotification, object, CancelToken> _check;
        public Func<Playable, UnityEngine.Playables.INotification, object, CancelToken> Check { get => _check; set => _check = value; }
    }

    public struct NotificationListeners : INotificationReceiver {
        // Stupid C#9.0
        public readonly Dictionary<object, INotificationReceiver> Listeners;

        public NotificationListeners(
            bool dummy) {

            Listeners = new Dictionary<object, INotificationReceiver>();
        }

        public void OnNotify(
            Playable origin,
            UnityEngine.Playables.INotification notification,
            object context) {

            foreach (var key in Listeners.Keys) {
                Listeners[key].
                    OnNotify(origin, notification, context);
            }
        }

        public void AddListener(
            object source,
            INotificationReceiver reciever) {

            if (Listeners.ContainsKey(source)) {
                return;
            }

            Listeners.
                Add(source, reciever);
        }

        public void RemoveListener(
            object source) {

            if (!Listeners.ContainsKey(source)) {
                return;
            }

            Listeners.
                Remove(source);
        }
    }

    public interface INotificationRecieverWrapper : INotificationReceiver {
        Action<Playable, UnityEngine.Playables.INotification, object> OnNotifiedHook { get; set; }
        Action OnNotified { get; set; }

        void RegisterToScope(IScope scope);
        void UnregisterToScope(IScope scope);
    }

    public interface INotificationReciever<T> : INotificationRecieverWrapper where T : UnityEngine.Playables.INotification {
        void INotificationRecieverWrapper.RegisterToScope(
            IScope scope) {
            scope.
                CheckHandler<T>();

            NotificationBoard<T>.Service.
                SubscribeListener(scope, this, this);
        }

        void INotificationRecieverWrapper.UnregisterToScope(
            IScope scope) {
            NotificationBoard<T>.Service.
                UnsubscribeListener(scope, this, this);
        }
    }

    public interface INotificationReciever<T1, T2> : INotificationRecieverWrapper
        where T1 : UnityEngine.Playables.INotification
        where T2 : UnityEngine.Playables.INotification {

        void INotificationRecieverWrapper.RegisterToScope(
            IScope scope) {
            scope.
                CheckHandler<T1>();
            NotificationBoard<T1>.Service.
                SubscribeListener(scope, this, this);

            scope.
                CheckHandler<T2>();
            NotificationBoard<T2>.Service.
                SubscribeListener(scope, this, this);
        }

        void INotificationRecieverWrapper.UnregisterToScope(
            IScope scope) {

            NotificationBoard<T1>.Service.
                UnsubscribeListener(scope, this, this);

            NotificationBoard<T2>.Service.
                UnsubscribeListener(scope, this, this);
        }
    }

    public interface INotificationReciever<T1, T2, T3> : INotificationRecieverWrapper
        where T1 : UnityEngine.Playables.INotification
        where T2 : UnityEngine.Playables.INotification
        where T3 : UnityEngine.Playables.INotification {

        void INotificationRecieverWrapper.RegisterToScope(
            IScope scope) {
            scope.
                CheckHandler<T1>();
            NotificationBoard<T1>.Service.
                SubscribeListener(scope, this, this);

            scope.
                CheckHandler<T2>();
            NotificationBoard<T2>.Service.
                SubscribeListener(scope, this, this);

            scope.
                CheckHandler<T3>();
            NotificationBoard<T3>.Service.
                SubscribeListener(scope, this, this);
        }

        void INotificationRecieverWrapper.UnregisterToScope(
            IScope scope) {

            NotificationBoard<T1>.Service.
                UnsubscribeListener(scope, this, this);

            NotificationBoard<T2>.Service.
                UnsubscribeListener(scope, this, this);

            NotificationBoard<T3>.Service.
                UnsubscribeListener(scope, this, this);
        }
    }

    public static class INotificationReciever_Extends {
        public static INotificationReciever<T> Interface<T>(
            this INotificationReciever<T> instance)
            where T : UnityEngine.Playables.INotification {

            return instance;
        }
    }
}
