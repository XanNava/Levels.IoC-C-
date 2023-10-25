#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

namespace Levels.Universal {

    using System;

    using global::Serilog;

    using Levels.Universal.General;

    using static Levels.Universal.Events;

    using Object = UnityEngine.Object;

    public sealed partial class IoC {
        public partial interface IScope {
            public interface Requesting : IInject {
                /// <summary>
                /// Call to retrieve a service from scope.
                /// </summary>
                /// <typeparam name="T"></typeparam>
                /// <param name="tag"></param>
                /// <returns></returns>
                /// <todo>Create test coverage.</todo>
                T Retrieve<T>(
                    string tag = "",
                    Settings.DI retrievalSettings = null);

                object Retrieve(
                    Type type,
                    string tag = "",
                    Settings.DI retrievalSettings = null);

                object[] RetrieveMultiple(
                    Type[] type,
                    string[] tag = null,
                    Settings.DI retrievalSettings = null);

                object[] RetrieveMultiple(
                    (Type type, string tag)[] keys,
                    Settings.DI retrievalSettings = null);

                T RetrieveAsSingleton<T>(
                    string tag = "",
                    Settings.DI retrievalSettings = null);

                object RetrieveAsSingleton(
                    Type type,
                    string tag = "",
                    Settings.DI retrievalSettings = null);

                object? TryGetNotificationHandler(
                    (Type type,
                    string tag) key,
                    Settings.DI retrievalSettings = null);

                bool Contains(
                    Type type,
                    string tag);
            }
        }

        [Settings.DI(AutoInject = false)]
        public sealed partial class ScopeRetrieval : IScope.Requesting, IInject {
            public IScope Scope;
            private ILogger _log;

            /// <summary>
            /// Call to retrieve a service from scope.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="tag"></param>
            /// <returns></returns>
            /// <todo>Create test coverage.</todo>
            public T Retrieve<T>(
                string tag = "",
                Settings.DI retrievalSettings = null) {
                retrievalSettings ??= Scope.GetDefaultRetrievalSettings();
                T value = (T)Retrieve(typeof(T), tag, retrievalSettings);

                return value;
            }

            public object Retrieve(
                Type type,
                string tag = "",
                Settings.DI retrievalSettings = null) {

                retrievalSettings ??= Scope.GetDefaultRetrievalSettings();

                var key = (type, tag);
                object value = null;

                if (value == null)
                    value = TryGetNotificationHandler(key, retrievalSettings);

                if (value == null)
                    value = TryByKey(key, retrievalSettings);

                if (value == null)
                    value = TryFromParent(key, retrievalSettings);

                if (value == null)
                    value = TryGetDefault(key, retrievalSettings);

                // Inject into new value if AutoInject is set.
                IScope.Injecting injection = Scope.Injection;

                var newSettings = injection.GetInjectSettings(value.GetType());

                _log.Debug("$[RETRIEVE][SUCCESS]\n|>[scope='{ScopeName}'][value='{Value}'][type='{Type}'][tag='{Tag}'][retrievalSettings='{RetrievalSettings}']\n|?[newSettings.AutoInject='{AutoInject}']#", Scope.GetScopeName(), value, type, tag, retrievalSettings.Readout(), newSettings.AutoInject);

                if (newSettings.AutoInject) {
                    injection.Inject(value.GetType(), value, newSettings);
                }

                return value;
            }

            private object TryFromParent((Type type, string tag) key, Settings.DI retrievalSettings) {
                retrievalSettings ??= Scope.GetDefaultRetrievalSettings();

                var parentScope = Scope.GetParentScope();

                if (retrievalSettings.RetrieveFromRoot && parentScope is not null) {
                    IScope.Requesting retrieval = parentScope.Requisition;

                    return retrieval.Retrieve(key.type, key.tag, retrievalSettings);
                }

                return null;
            }

            private object TryByKey((Type type, string tag) key, Settings.DI retrievalSettings) {
                retrievalSettings ??= Scope.GetDefaultRetrievalSettings();

                var exports = Scope.GetExports().Dictionary;

                // Try and get resource by key
                if (exports.ContainsKey(key)) {
                    return exports[key]?.Invoke(Scope, key.tag);
                } else if (retrievalSettings.DefaultTags && exports.ContainsKey((key.type, ""))) {
                    return exports[(key.type, "")].Invoke(Scope, key.tag);
                }

                return null;
            }

            private object TryGetDefault((Type type, string tag) key, Settings.DI retrievalSettings) {
                if (key.type.IsClass && !key.type.IsSubclassOf(typeof(Object))) {
                    return Activator.CreateInstance(key.type);
                }

                return null;
            }

            /// <summary>
            /// Will retrieve object from scope, but will register as a singleton before injecting(this will fix some dependency loops).
            /// </summary>
            /// <typeparam name="T">The type to retrieve.</typeparam>
            /// <param name="tag">The tag to use when retrieving the service.</param>
            /// <param name="defaultTags">If we should default tags if the requested one isn't present.</param>
            /// <returns>The object registered as a Singleton.</returns>
            public T RetrieveAsSingleton<T>(string tag = "", Settings.DI retrievalSettings = null) {
                retrievalSettings ??= Scope.GetDefaultRetrievalSettings();

                //Debug.Log(tag);
                T obj = (T)RetrieveAsSingleton(typeof(T), tag, retrievalSettings);

                return obj;
            }

            public object RetrieveAsSingleton(
                Type type,
                string tag = "",
                Settings.DI retrievalSettings = null) {
                Scope.GetLogs().AppendLine($"$[RETREIVE_AS_SINGLE][START]");
                var obj = Retrieve(type, tag, retrievalSettings);
                IScope.Registering registering = Scope.Registering;
                bool exists = false;

                // TODO, We have to figure out if there already is a singleton registered.
                registering.RegisterSingleton(type, obj, out exists, tag);

                _log.Debug("$[RETREIVE_AS_SINGLE][SUCCESS]\n|>[type='{Type}'][tag='{Tag}'][retrievalSettings='{TetrievalSettings}']\n|?[exists='{Exists}']#", type, tag, retrievalSettings?.Readout(), exists);

                return obj;
            }

            public object? TryGetNotificationHandler(
                (Type type, string tag) key,
                Settings.DI retrievalSettings = null) {

                retrievalSettings ??= Scope.GetDefaultRetrievalSettings();

                var exports = Scope.GetExports().Dictionary;

                // TODO: Make this a factory for INotificationHandler.
                // Intercept Notification Handlers.
                if (key.type == typeof(INotificationHandler) || key.type.GetInterface(nameof(INotificationHandler)) != null) {
                    if (exports.ContainsKey(key)) {
                        return exports[key];
                    }

                    IScope.Notifying notifications = Scope.Notification;
                    IScope.Registering registering = Scope.Registering;

                    var value = new NotificationHandler();
                    value.Setup(Type.GetType(key.tag));

                    notifications.SubscribeHandler(value);

                    registering.RegisterSingletonAs(value, key.type, key.tag);

                    Scope.GetLogs().AppendLine($"$[GET_HANDLER][SUCCESS]\n|>[value='{value}'][key='{key}'][retrievalSettings='{retrievalSettings?.Readout()}']#");
                    return value;
                }
                return null;
            }

            public object[] RetrieveMultiple(
                Type[] type,
                string[] tag = null, Settings.DI retrievalSettings = null) {
                object[] value = new object[type.Length];

                (Type, String)[] holder = new (Type, String)[type.Length];
                return RetrieveMultiple(holder, retrievalSettings);
            }

            public object[] RetrieveMultiple(
                (Type type, string tag)[] keys, Settings.DI retrievalSettings = null) {
                object[] values = new object[keys.Length];

                foreach (var i in keys.Length) {
                    (Type type, string tag) key = (keys[i].type, !string.IsNullOrEmpty(keys[i].tag) ? keys[i].tag : "");

                    values[i] = Retrieve(key.type, key.tag, retrievalSettings);
                }

                return values;
            }

            public void Receive(object[] values) {
                Scope = (IScope)values[0];
                _log = (ILogger)values[1];
            }

            public (Type, string)[] Request() => new[] { (typeof(IScope), "Current"), (typeof(ILogger), nameof(ScopeRetrieval)) };

            public bool Contains(Type type, string tag) {
                var dictionary = Scope.GetExports().Dictionary;
                return dictionary.ContainsKey((type, tag));
            }
        }
    }
}