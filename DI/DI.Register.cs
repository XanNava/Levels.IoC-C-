// TODO: Make it so we can register values.

namespace Levels.Universal {

    using System;
    using System.Text;

    using global::Serilog;

    using JetBrains.Annotations;

    using Universal.Creational;
    using Universal.Functional;

    public sealed partial class IoC {
        public partial interface IScope {
            public interface Registering : IInject {
                /// <summary>
                /// Call to register service as a interface.
                /// </summary>
                /// <typeparam name="T">Service registering.</typeparam>
                /// <typeparam name="V">Interface registering the service to.</typeparam>
                /// <param name="tag">Optional: Tag for injection.</param>
                /// <param name="ctor">Optional: A Func to be used when retrieving service </param>
                /// <todo>Create test coverage</todo>
                IScope /*.*/ Register<T>(
                    string tag = "",
                    Func<IScope, String, T> ctor = null) where T : new();

                IScope /*.*/ RegisterAs<T, V>(
                    string tag = "",
                    Func<IScope, String, V> ctor = null) where T : V, new();

                IoC.IScope /*.*/ RegisterSingleton(
                    Type type,
                    object instance,
                    out bool exists,
                    string tag = "");

                IScope /*.*/ RegisterSingleton<T>(
                    [NotNull] T instance,
                    string tag = "");

                IScope /*.*/ RegisterSingletonAs<T, V>(
                    [NotNull] T instance,
                    string tag = "") where T : V;

                IScope /*.*/ RegisterSingletonAs(
                    [NotNull] object instance,
                    Type asType,
                    string tag = "");

                IScope /*.*/ RegisterPooled<T, ARGS>(
                    [NotNull] ARGS defaultSetup,
                    string tag = "") where ARGS : class where T : class, ISetup<T, ARGS>, new();

                IScope /*.*/ RegisterPooledAs<T, A, ARGS>(
                    [NotNull] ARGS defaultSetup,
                    string tag = "") where T : class, ISetup<T, ARGS>, A, new();

                IScope /*.*/ RegisterFactory<T>(
                    Func<IScope, String, T> ctor,
                    string tag = "");


                bool /*.*/ HasRegistry<T>(string tag = "");

                bool /*.*/ HasRegistry(Type type, string tag = "");

                string /*.*/ PrintOutRegistry();
            }
        }

        [Settings.DI(AutoInject = false)]
        public sealed partial class ScopeRegistration : IScope.Registering, IInject {
            public IScope Scope;
            private ILogger _log;
            /// <summary>
            /// Call to register service as a interface.
            /// </summary>
            /// <typeparam name="T">Service registering.</typeparam>
            /// <typeparam name="V">Interface registering the service to.</typeparam>
            /// <param name="tag">Optional: Tag for injection.</param>
            /// <param name="ctor">Optional: A Func to be used when retrieving service </param>
            /// <todo>Create test coverage</todo>
            [NotNull]
            public IScope /*.*/ Register<T>(
                string tag = "",
                Func<IScope, String, T> ctor = null) where T : new() {
                (Type, string tag) key = (typeof(T), tag);

                var exports = Scope.GetExports();
                var exportsDictionary = exports.Dictionary;
                if (exportsDictionary.ContainsKey(key)) {
                    _log.Debug("$[REG][FAILED]\n|>[register='{TypeOf}'][tag='{Tag}']#", typeof(T), tag);
                    return Scope;
                }

                ctor ??= (IScope s, string v) => { return new T(); };

                exportsDictionary.Add(key, (IScope s, string v) => ctor.Invoke(s, v));
                _log.Debug("$[REG][SUCCESS]\n|>[register='{TypeOf}'][tag='{Tag}']#", typeof(T), tag);

                return Scope;
            }

            [NotNull]
            public IScope /*.*/ RegisterAs<T, V>(
                string tag = "",
                Func<IScope, String, V> ctor = null) where T : V, new() {
                (Type, string tag) key = (typeof(V), tag);

                var exports = Scope.GetExports();
                var exportsDictionary = exports.Dictionary;

                if (exportsDictionary.ContainsKey(key)) {
                    _log.Debug("$[REG_AS][FAILED]\n|>[register='{TypeOfT}'][as='{TypeOfV}'][tag='{Tag}']#", typeof(T), typeof(V), tag);
                    return Scope;
                }

                ctor ??= (IScope s, string v) => { return new T(); };

                exportsDictionary.Add(key, (IScope s, string v) => ctor.Invoke(s, v));
                _log.Debug("$[REG_AS][SUCCESS]\n|>[register='{TypeOfT}'][as='{TypeOfV}'][tag='{Tag}']#", typeof(T), typeof(V), tag);

                return Scope;
            }

            /// <summary>
            /// With Register the passed <see cref="instance"/> as a singleton.
            /// If one already exists it won't override <todo msg="Make optional"/>
            /// </summary>
            /// <param name="type"></param>
            /// <param name="instance"></param>
            /// <param name="tag"></param>
            /// <returns>this</returns>
            [NotNull]
            public IScope RegisterSingleton(
                Type type,
                object instance,
                out bool exists,
                string tag = "") {
                (Type, string tag) key = (type, tag);

                var logBuilder = Scope.GetLogs();
                var exports = Scope.GetExports();
                var exportsDictionary = exports.Dictionary;

                if (exists = exportsDictionary.ContainsKey(key)) {
                    _log.Debug("$[REG_SINGLE][EXISTED]\n|>[type='{Type}'][instance='{Instance}'][tag='{Tag}']#", type, instance, tag);

                    return Scope;
                }

                exists = false;

                exportsDictionary.Add(key, (IScope s, string v) => { return instance; });

                _log.Debug("$[REG_SINGLE][SUCCESS]\n|>[type='{Type}'][instance='{Instance}'][tag='{Tag}']#", type, instance, tag);

                return Scope;
            }

            [NotNull]
            public IScope /*.*/ RegisterSingleton<T>(
                [NotNull] T instance,
                string tag = "") {

                bool dummy;
                RegisterSingleton(instance.GetType(), instance, out dummy, tag);

                return Scope;
            }

            [NotNull]
            public IScope /*.*/ RegisterSingletonAs<T, V>(
                [NotNull] T instance,
                string tag = "") where T : V {

                bool dummy;
                RegisterSingleton(typeof(V), instance, out dummy, tag);

                return Scope;
            }

            public IScope RegisterSingletonAs(object instance, Type asType, string tag = "") {
                bool dummy;

                RegisterSingleton(asType, instance, out dummy, tag);

                return Scope;
            }

            [NotNull]
            public IScope /*.*/ RegisterPooled<T, ARGS>(
                [NotNull] ARGS defaultSetup,
                string tag = "") where ARGS : class where T : class, ISetup<T, ARGS>, new() {
                (Type, string tag) key = (typeof(T), tag);

                var exportsDictionary = Scope.GetExports();
                var dictionary = exportsDictionary.Dictionary;

                if (dictionary.ContainsKey(key))
                    return Scope;

                dictionary.Add(
                    key, (IScope s, string v) => { return Pool<T, ARGS>.ServiceReference.RetrieveResource(defaultSetup); });

                return Scope;
            }

            [NotNull]
            public IScope /*.*/ RegisterPooledAs<T, A, ARGS>(
                [NotNull] ARGS defaultSetup,
                string tag = "") where T : class, ISetup<T, ARGS>, A, new() {
                (Type, string tag) key = (typeof(A), tag);

                var exportsDictionary = Scope.GetExports();
                var dictionary = exportsDictionary.Dictionary;

                if (dictionary.ContainsKey(key))
                    return Scope;

                dictionary.Add(
                    key, (IScope s, string v) => { return Pool<T, ARGS>.ServiceReference.RetrieveResource(defaultSetup); });

                return Scope;
            }

            public bool HasRegistry<T>(string tag = "") {
                return HasRegistry(typeof(T), tag);
            }

            public bool HasRegistry(
                Type type,
                string tag = "") {
                var exportsDictionary = Scope.GetExports();
                var dictionary = exportsDictionary.Dictionary;

                return dictionary.ContainsKey((type, tag));
            }

            public string PrintOutRegistry() {
                StringBuilder readout = new();
                var exportsDictionary = Scope.GetExports();
                var dictionary = exportsDictionary.Dictionary;

                foreach (var export in dictionary.Keys) {
                    readout.AppendLine(export.ToString());
                }

                return readout.ToString();
            }

            public void Receive(object[] values) {
                Scope = (IScope)values[0];
                _log = (ILogger)values[1];
            }

            public (Type, string)[] Request() => new[] { (typeof(IScope), "Current"), (typeof(ILogger), nameof(ScopeRegistration)) };

            public IScope RegisterFactory<T>(Func<IScope, string, T> ctor, string tag = "") {
                (Type, string tag) key = (typeof(T), tag);

                var exports = Scope.GetExports();
                var exportsDictionary = exports.Dictionary;
                if (exportsDictionary.ContainsKey(key)) {
                    _log.Debug("$[REG][FAILED]\n|>[register='{TypeOf}'][tag='{Tag}']#", typeof(T), tag);
                    return Scope;
                }

                return Scope;
            }
        }
    }
}