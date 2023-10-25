namespace Levels.Universal {

    using System;
    using System.Collections.Generic;
    using System.Text;

    public sealed partial class IoC {
        public partial interface IScope : IScopeHierarchy {
            public IScope.Injecting Injection { get; }

            public IScope.Registering Registration { get; }

            public IScope.Notifying Notification { get; }

            public IScope.Requesting Requisition { get; }

            public ExportsDictionary GetExports();

            public StringBuilder GetLogs();

            public Settings.DI GetDefaultRetrievalSettings();

            public Settings.Notifications GetDefaultNotificationSettings();
        }

        public sealed partial class Scope : IScope, IInject {
            // TODO: Move the public settings variables to a properties class, and have it used for setup.
            // TODO: https://www.youtube.com/watch?v=rygIP5sh00M
            static IScope _rootScope;

            public static IScope RootScope {
                get {
                    UnityEngine.Debug.Log(_rootScope);
                    if (_rootScope == null) {
                        UnityEngine.Debug.Log("Setting root scope");
                        // TODO: Move the setup to after the creation of a scope. Currently the creation of the scope is
                        // Triggering the creation of a root scope, which is looping on itself.
                        _rootScope = new Scope("Root");
                        UnityEngine.Debug.Log(_rootScope);
                    }

                    return _rootScope;
                }
                private set {
                    UnityEngine.Debug.Log("Setter root scope");

                    _rootScope = value;
                }
            }

            public readonly List<IScope> childScopes = new List<IScope>();

            public Settings.DI defaultRetrievalSettings = new Settings.DI();

            public IScope parentScope;
            public string scopeName;
            public readonly ExportsDictionary exports = new ExportsDictionary();

            public StringBuilder logs = new StringBuilder();

            public IScope.Registering _registering;
            public IScope.Injecting _injection;
            public IScope.Notifying _notifications;
            public IScope.Requesting _retrieval;

            public void Receive(object[] values) {
                _registering = (IScope.Registering)values[0];
                _injection = (IScope.Injecting)values[1];
                _notifications = (IScope.Notifying)values[2];
                _retrieval = (IScope.Requesting)values[3];
            }

            public (Type, string)[] Request() => new[] {
                (typeof(IScope.Registering), ""),
                (typeof(IScope.Injecting), ""),
                (typeof(IScope.Notifying), ""),
                (typeof(IScope.Requesting), "")
            };


            public IScope.Registering Registration {
                get {
                    return _registering;
                }
            }

            public IScope.Injecting Injection {
                get {
                    return _injection;
                }
            }

            public IScope.Notifying Notification {
                get {
                    return _notifications;
                }
            }

            public IScope.Requesting Requisition {
                get {
                    return _retrieval;
                }
            }

            public ExportsDictionary GetExports() {
                return exports;
            }

            public StringBuilder GetLogs() {
                return logs;
            }

            public Settings.DI GetDefaultRetrievalSettings() {
                return defaultRetrievalSettings;
            }

            public Settings.Notifications GetDefaultNotificationSettings() {
                throw new NotImplementedException();
            }
        }
    }
}

//-|ChildScope [\/]
//-|Register[\/]
//--|Singleton[\/]
//--|ServieLocator[\/]
//--|Generate[\/]
//---|Create[\/]
//---|Factory[\/]
//---|Pool[\/]
//-|IRequire[\/]
//--|Attribute Injection[-]
//---|Constructor[-] Started
//---|Variable[\/]
//---|Method[-] Started
//---|Interface[\/]
//--|Interface Injection[\/]
//|Messaging[\/]