namespace Levels.Universal {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using global::Serilog;

    using JetBrains.Annotations;

    using Levels.Universal.General;

    using UnityEngine.Playables;

    public sealed partial class IoC {
        public partial interface IScope {
            public interface Injecting : IInject {
                /// <summary>
                /// The main IRequire function.
                /// </summary>
                /// <typeparam name="T"></typeparam>
                /// <param name="target"></param>
                IScope /*.*/ Inject<T>([NotNull] T target, Settings.DI injectSettings = null);

                IScope Inject(Type type, [NotNull] object target, Settings.DI injectSettings = null);

                Settings.DI GetInjectSettings(Type t);
            }
        }


        [Settings.DI(AutoInject = false)]
        public sealed partial class ScopeInject : IScope.Injecting {
            public IScope Scope;
            private StringBuilder _logBuilder = new StringBuilder();
            private ILogger _log;

            public IScope /*.*/ Inject<T>([NotNull] T target, Settings.DI injectSettings = null) {

                Inject(target.GetType(), target, injectSettings);

                return Scope;
            }

            public IScope /*.*/ Inject(Type type, [NotNull] object target, Settings.DI settings = null) {
                settings ??= GetInjectSettings(type);

                _log.Debug("$[INJECT][START]\n|>[variables='{Variables}']\n|?[InjectPublicFields='{InjectFeilds}'][InjectInterface='{IsRequest}'][IsNotificationReciever='{IsReciever}']#", (type, target, settings, Scope.GetScopeName()), settings.InjectPublicFields, settings.InjectInterface && target is IInject, type.GetInterface(nameof(INotificationReceiver)) != null);

                if (settings.InjectPublicFields)
                    InjectPublicFields(target, type, settings);

                if (settings.InjectInterface && target is IInject inject)
                    InjectInterface(inject, type, settings);

                if (settings.RegisterNotificationReceiver && type.GetInterface(nameof(INotificationReceiver)) != null) {
                    IScope.Notifying notifications = Scope.Notification;
                    Settings.Notifications attribute = GetNotificationSettings(type);

                    notifications.SubscribeReciever((INotificationReceiver)target, attribute);
                }

                return Scope;
            }

            private static Settings.Notifications GetNotificationSettings(
                Type type) {

                MethodInfo notify = type.GetMethod(nameof(INotificationReceiver.OnNotify));
                var attribute = notify.GetCustomAttribute(typeof(Settings.Notifications)) as Settings.Notifications;
                attribute ??= new Settings.Notifications();

                return attribute;
            }

            [NotNull]
            public Settings.DI GetInjectSettings(
                Type t) {
                Settings.DI injectSettings = null;
                System.Attribute[] customAttributes = System.Attribute.GetCustomAttributes(t);
                bool flag = false;

                foreach (System.Attribute attribute in customAttributes) {
                    if (attribute is not Settings.DI)
                        continue;

                    injectSettings = (Settings.DI)attribute;
                    flag = true;
                }

                if (!flag) {
                    injectSettings = new Settings.DI();
                }

                if (t == typeof(IInject) || t.GetInterface(nameof(IInject)) != null) {
                    injectSettings.InjectInterface = true;
                }

                return injectSettings;
            }

            private void InjectPublicFields<T>(
                [NotNull] T target,
                Settings.DI injectSettings = null) {

                Type t = typeof(T);
                injectSettings ??= GetInjectSettings(t);

                InjectPublicFields(target, t);
            }

            private void InjectPublicFields<T>(
                [NotNull] T target,
                Type t,
                Settings.DI injectSettings = null) {

                injectSettings ??= GetInjectSettings(t);
                var fieldsWithAttribute = t.GetFields();

                parseFieldsInfo(target, fieldsWithAttribute);
            }

            private void parseFieldsInfo<T>(T target, FieldInfo[] fieldsWithAttribute) {
                foreach (FieldInfo fieldInfo in fieldsWithAttribute) {
                    var fieldsAttribute = fieldInfo.GetCustomAttributes();

                    parseFeildAttributes(target, fieldInfo, fieldsAttribute);
                }
            }

            private void parseFeildAttributes<T>(T target, FieldInfo fieldInfo, IEnumerable<System.Attribute> fieldsAttribute) {
                foreach (System.Attribute fieldAttribute in fieldsAttribute) {
                    if (fieldAttribute is not Attribute.Inject)
                        continue;

                    var attribute = fieldAttribute as Attribute.Inject;
                    IScope.Requesting retrieval = Scope.Requisition;
                    Type fieldType = fieldInfo.FieldType;
                    string tag = attribute.GetTag();

                    object value = retrieval.Retrieve(fieldType, tag);

                    _log.Debug("$[INJECT_FEILDS][SUCCESS]\n|>[field.Type='{FieldType}'][feild.tag='{Tag}'][target='{Target}'][value='{Value}']#", fieldType, tag, target, value);
                    fieldInfo.SetValue(target, value);
                }
            }

            private void InjectVariable<T, V>(
                [NotNull] out V val,
                string tag = "",
                Settings.DI injectSettings = null) {

                var type = typeof(T);
                injectSettings ??= GetInjectSettings(type);

                IScope.Requesting retrieval = Scope.Requisition;
                val = (V)retrieval.Retrieve(type, tag);

                if (val == null)
                    val = default;
            }

            private void InjectInterface<T>([NotNull] T target, Type t,
                Settings.DI injectSettings = null) where T : IInject {
                injectSettings ??= GetInjectSettings(target.GetType());

                var retrieval = Scope.Requisition;
                var keys = target.Request();

                string keysReadout = KeysReadout(keys);

                var values = retrieval.RetrieveMultiple(keys, injectSettings);

                string valuesReadout = ValuesReadout(values);

                _log.Debug("$[INJECT_INTRFC][SUCCESS]\n|>[target='{Target}'][type='{TypeOf}'][injectSettings='{InjectSettings}'][keys='{KeysReadout}'][values='{ValuesReadout}']#", target, t, injectSettings, keysReadout, valuesReadout);

                target.Receive(values);
            }

            private string KeysReadout((Type, string)[] keys) {
                string keysReadout = "";

                foreach (var i in keys.Length) {
                    _logBuilder.Append($"{i}=({keys[i].Item1},{keys[i].Item2}),");
                }

                keysReadout = _logBuilder.ToString();
                _logBuilder.Clear();
                return keysReadout;
            }

            private string ValuesReadout(object[] values) {
                string valuesReadout = "";
                foreach (var i in values.Length) {
                    _logBuilder.Append($"{i}=({values[i]}),");
                }

                valuesReadout = _logBuilder.ToString();
                _logBuilder.Clear();
                return valuesReadout;
            }

            // TODO: Finish
            private void InjectConstructor([NotNull] Type target,
                Settings.DI injectSettings = null) {
                injectSettings ??= GetInjectSettings(target.GetType());

                ConstructorInfo constructor = null;
                int maxArgCount = 0;

                foreach (ConstructorInfo ci in target.GetConstructors()) {
                    if (ci.GetParameters().Length > maxArgCount) {
                        constructor = ci;
                        maxArgCount = constructor.GetParameters().Length;
                    }
                }

                var parameterInfos = constructor.GetParameters();
                object[] arguments = new object[parameterInfos.Length];

                for (int i = 0; i < parameterInfos.Length; i++) {

                    IScope.Requesting retrieval = Scope.Requisition;
                    arguments[i] = retrieval.Retrieve(target, "");
                }
            }

            // TODO: Finish
            private void InjectMethods([NotNull] Type target,
                Settings.DI injectSettings = null) {

                injectSettings ??= GetInjectSettings(target.GetType());

                MethodInfo[] methodInfos = target.GetMethods();
                MethodInfo method = methodInfos.FirstOrDefault(m => m.GetCustomAttribute<Attribute.Inject>() != null);
                if (method == null) {
                    return;
                }

                var parameterInfos = method.GetParameters();
                object[] arguments = new object[parameterInfos.Length];

                for (int i = 0; i < parameterInfos.Length; i++) {
                    var key = (parameterInfos[i].ParameterType, "");
                    IScope.Requesting retrieval = Scope.Requisition;
                    arguments[i] = retrieval.Retrieve(target, "");
                }

                var instance = Activator.CreateInstance(target);
                method.Invoke(instance, arguments);
            }

            public void Receive(object[] values) {
                Scope = (IScope)values[0];
                _log = (ILogger)values[1];
            }

            public (Type, string)[] Request() => new[] { (typeof(IScope), "Current"), (typeof(ILogger), nameof(ScopeInject)) };
        }
    }
}