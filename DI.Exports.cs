using System;
using System.Collections.Generic;
using System.Text;

using LanguageExt;
using LanguageExt.Common;

using UnityEngine;

namespace Levels.Universal.Experimental {
    public static class Exports<T> {
        public static Settings.DI settings;

        private readonly static Dictionary<IScope, IRegisteryTable<T>> _registeries = new Dictionary<IScope, IRegisteryTable<T>>();

        public static Dictionary<IScope, IRegisteryTable<T>> Registeries { get => _registeries; }

        private static StringBuilder _readoutBuilder { get; } = new StringBuilder();

        // TODO: Change to one of.
        public static Result<T> Retrieve(IScope scope, string key = "", bool fromChild = false, Settings.DI settings = null) {
            T value = default(T);
            Debug.Log($">> RETRIEVING : (type={typeof(T).Name}, key='{key}')\n[scope='{scope.Name}']\n");

            Exports<T>.settings ??= SetupSettings(settings);

            value ??= RetrieveFromRegistered(scope, key, settings, value);

            value ??= RetrieveFromParent(scope, key, settings, value);

            value ??= RetrieveDefault(value);

            if ((!value.IsNull<T>() || !value.IsDefault<T>())) {
                Debug.Log($"<< RETRIEVED : (type={typeof(T).Name}, key='{key}')\n[fromScope='{scope.Name}']\n");
                return value;
            }

            throw new KeyNotFoundException($"The was no entry for scope='{scope.Name}' for Type='{typeof(T)}' and tag='{key}'");
        }

        private static T RetrieveDefault(T value) {
            if ((value.IsNull<T>() || value.IsDefault<T>()) && typeof(T).IsClass && typeof(T).GetConstructor(Type.EmptyTypes) != null) {
                value = (T)Activator.CreateInstance(typeof(T));
            } else if ((value.IsNull<T>() || value.IsDefault<T>()) && typeof(T).IsValueType) {
                value = default(T);
            }

            return value;
        }

        private static T RetrieveFromParent(IScope scope, string key, Settings.DI settings, T value) {
            if ((value.IsNull<T>() || value.IsDefault<T>()) && settings.RetrieveFromParents && scope.Parent != null) {
                Debug.Log($"RETRIEVING_PARENT : (type={typeof(T).Name}, key='{key}')\n[scope='{scope.Parent.Name}, {IScope.Root.Name}']\n");
                value = Retrieve(scope.Parent, key, true, settings).Match<T>((r) => {
                    return r;
                }, (r) => {
                    throw r;
                });
            }

            return value;
        }

        private static Settings.DI SetupSettings(Settings.DI settings) {
            if (settings == null) {
                settings ??= settings;
                settings ??= new Settings.DI();
            }

            return settings;
        }

        private static T RetrieveFromRegistered(IScope scope, string key, Settings.DI settings, T value) {
            if (Registeries.ContainsKey(scope) && settings.PassTags) {
                value = Registeries[scope].Retrieve("", key);
            }
            if ((value.IsNull<T>() || value.IsDefault<T>()) && Registeries.ContainsKey(scope)) {
                value = Registeries[scope].Retrieve(key, key);
            }
            if ((value.IsNull<T>() || value.IsDefault<T>()) && settings.DefaultTags) {
                value = Registeries[scope].Retrieve("", "");
            }

            return value;
        }

        public static void Register(IScope scope, IEntry<T> reg) {
            if (!Registeries.ContainsKey(scope)) {
                Registeries.Add(scope, new RegisteryTable<T>(scope, _readoutBuilder));
            }

            Registeries[scope].Register(reg);
        }

        public static void TryRegister(IScope scope, object reg) {
            if (reg.GetType().GetInterface(nameof(IEntry<T>)) != null) {
                throw new InvalidCastException($"The reg object passed to Register needs to be of type IRegistery<T>, but was of type {reg.GetType()}");
            }

            var regInst = reg as IEntry<T>;

            if (!Registeries.ContainsKey(scope)) {
                Registeries.Add(scope, new RegisteryTable<T>(scope, _readoutBuilder));
            }

            Registeries[scope].Register(regInst);
        }
    }
}
