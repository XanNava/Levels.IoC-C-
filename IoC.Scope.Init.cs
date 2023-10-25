namespace Levels.Universal {
    public sealed partial class IoC {
        public interface IScopeHierarchy {
            IScope CreateChildScope(string name = "");

            bool IsChildScope();

            IScope SetAsRootScope();

            IScope SetParentScope(IScope parent);

            IScope GetParentScope();

            IScope RegisterChildScope(IScope child);

            string GetScopeName();
        }

        public sealed partial class Scope : IScopeHierarchy {
            public Scope(
                string name = "",
                Scope parentScope = null) {

                scopeName = name;
                this.parentScope = parentScope;

                // Have to do this as nothing has been created yet.
                SetupInjection();

                SetupRegistration();

                SetupRetrieval();

                SetupNotifications();

                SetupHierarchy();
            }

            private void SetupInjection() {
                if (Injection is not null) {
                    return;
                }

                _injection = new ScopeInject();

                if (!exports.Dictionary.ContainsKey((typeof(IScope.Injecting), "")))
                    exports.Dictionary.Add((typeof(IScope.Injecting), ""), (IScope s, string v) => { return Injection; });
                _injection.Receive(new object[] { this, Levels.Universal.Serilog.LoggerFactory.Service.BuildLogger(typeof(ScopeInject)) });
            }

            private void SetupRegistration() {
                if (Registration is not null) {
                    return;
                }

                _registering = new ScopeRegistration();

                if (!exports.Dictionary.ContainsKey((typeof(IScope.Registering), "")))
                    exports.Dictionary.Add((typeof(IScope.Registering), ""), (IScope s, string v) => { return Registration; });
                _registering.Receive(new object[] { this, Levels.Universal.Serilog.LoggerFactory.Service.BuildLogger(typeof(ScopeRegistration)) });
            }

            private void SetupRetrieval() {
                if (Requisition is not null) {
                    return;
                }

                _retrieval = new ScopeRetrieval();

                if (!exports.Dictionary.ContainsKey((typeof(IScope.Requesting), "")))
                    exports.Dictionary.Add((typeof(IScope.Requesting), ""), (IScope s, string v) => { return Requisition; });
                _retrieval.Receive(new object[] { this, Levels.Universal.Serilog.LoggerFactory.Service.BuildLogger(typeof(ScopeRetrieval)) });
            }

            private void SetupNotifications() {
                if (Notification is not null) {
                    return;
                }

                _notifications = new ScopeNotificationsDefault();

                if (!exports.Dictionary.ContainsKey((typeof(IScope.Notifying), "")))
                    exports.Dictionary.Add((typeof(IScope.Notifying), ""), (IScope s, string v) => { return Notification; });
                _notifications.Receive(new object[] { this, Levels.Universal.Serilog.LoggerFactory.Service.BuildLogger(typeof(ScopeNotificationsDefault)) });
            }

            public void SetupHierarchy() {
                // TODO: This is backwards, Roots should register child scopes.
                if (RootScope != null && parentScope == null && this != RootScope) {
                    RootScope.RegisterChildScope(this);
                }
            }

            public static IScope CreateScope(
                string name = "",
                Scope parentScope = null) {

                return new Scope(name, parentScope);
            }

            public IScope CreateChildScope(
                string name = "") {

                return new Scope(name, this);
            }

            public bool IsChildScope() {
                if (parentScope == null)
                    return false;

                return true;
            }

            public IScope GetParentScope() => parentScope;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="child"></param>
            /// <returns>The current scope(aka not the child scope)</returns>
            public IScope RegisterChildScope(
                IScope child) {

                childScopes.Add(child);
                child.SetParentScope(this);

                return this;
            }

            public string GetScopeName() => scopeName;

            public IScope SetParentScope(
                IScope parent) {

                parentScope = parent;

                return this;
            }

            public IScope SetAsRootScope() {
                parentScope = null;

                RootScope = this;

                if (RootScope is not null && RootScope != this)
                    RegisterChildScope(RootScope);


                return this;
            }
        }
    }
}