using System;
using System.Reflection;

using UnityEngine;

namespace Levels.Universal.Experimental {
    [Settings.DI]
    public interface IsInjectable {
        // public IScope Scope { get; set; }
        public void Inject(IScope scope);
        public virtual Settings.DI GetAttribute() {
            return (Settings.DI)this.GetType().GetInterface(nameof(IsInjectable)).GetCustomAttribute(typeof(Settings.DI));
        }
    }

    public interface IInject<REQUEST_1> : IsInjectable {
        virtual string Tags { get => ""; }

        public void Recieve(IScope scope, REQUEST_1 request1);

        void IsInjectable.Inject(IScope scope) {
            if (GetAttribute().IsInjected)
                return;

            UnityEngine.Debug.Log($"// INJECT : [into='{this.GetType().Name}']\n[fromScope='{scope.Name}']\n|>[request1=({typeof(REQUEST_1).Name},{Tags})]");
            Recieve(scope, Request1(scope, Tags));

            GetAttribute().IsInjected = true;
            Debug.Log($"\\\\ INJECT_FIN : [into='{this.GetType().Name}']");
        }

        public REQUEST_1 Request1(IScope scope, string tag = "") {
            UnityEngine.Debug.Log($"@@ REQUEST_1 :({typeof(REQUEST_1)})\n[into='{this.GetType().Name}'][fromScope='{scope.Name}']\n|>[request1=({typeof(REQUEST_1).Name},{Tags})]");

            var result = Exports<REQUEST_1>.Retrieve(scope, tag).Match<REQUEST_1>((r) => {
                return r;
            },
            (r) => {
                UnityEngine.Debug.LogError($"XX INJECT : [FAILED][1]\n[{typeof(REQUEST_1).Name}]\n[{r}]");

                return default(REQUEST_1);
            });

            return result;
        }
    }

    public interface IInject<REQUEST_1, REQUEST_2> : IInject<REQUEST_1> {
        virtual new (string tag1, string tag2) Tags { get => ("", ""); }

        void IInject<REQUEST_1>.Recieve(IScope scope, REQUEST_1 request1) {
            throw new InvalidOperationException("Using level below inherited interface, please use method with matching generic arguments as interface.");
        }

        public void Recieve(IScope scope, REQUEST_1 request1, REQUEST_2 request2);

        void IsInjectable.Inject(IScope scope) {
            if (GetAttribute().IsInjected)
                return;

            var holder = Tags;
            UnityEngine.Debug.Log($"// INJECT : [into='{this.GetType().Name}']\n[fromScope='{scope.Name}']\n|>[request1=({typeof(REQUEST_1).Name},{Tags.tag1})]\n|>[request2=({typeof(REQUEST_2).Name},{Tags.tag2})]");
            Recieve(scope, Request1(scope, holder.tag1), Request2(scope, holder.tag2));

            GetAttribute().IsInjected = true;
            Debug.Log($"\\\\ INJECT_FIN : [into='{this.GetType().Name}']");
        }

        public REQUEST_2 Request2(IScope scope, string tag = "") {
            UnityEngine.Debug.Log($"@@ REQUEST_2 : ({typeof(REQUEST_2)})\n[into='{this.GetType().Name}'][fromScope='{scope.Name}']\n|>[request2=({typeof(REQUEST_2).Name},{Tags})]");

            var result = Exports<REQUEST_2>.Retrieve(scope, tag).Match<REQUEST_2>((r) => {
                return r;
            },
            (r) => {
                UnityEngine.Debug.LogError($"XX INJECT : [FAILED][2]\n[{typeof(REQUEST_2).Name}]\n[{r}]");

                return default(REQUEST_2);
            });

            return result;
        }
    }

    public interface IInject<REQUEST_1, REQUEST_2, REQUEST_3> : IInject<REQUEST_1, REQUEST_2> {
        virtual new (string tag1, string tag2, string tag3) Tags { get => ("", "", ""); }

        void IInject<REQUEST_1, REQUEST_2>.Recieve(IScope scope, REQUEST_1 request1, REQUEST_2 request2) {
            throw new InvalidOperationException("Using level below inherited interface, please use method with matching generic arguments as interface.");
        }

        public void Recieve(IScope scope, REQUEST_1 request1, REQUEST_2 request2, REQUEST_3 request3);

        void IsInjectable.Inject(IScope scope) {
            if (GetAttribute().IsInjected)
                return;

            var holder = Tags;
            UnityEngine.Debug.Log($"// INJECT : [into='{this.GetType().Name}']\n[fromScope='{scope.Name}']\n|>[request1=({typeof(REQUEST_1).Name},{Tags.tag1})]\n|>[request2=({typeof(REQUEST_2).Name},{Tags.tag2})]\n|>[request3=({typeof(REQUEST_3).Name},{Tags.tag3})]");
            Recieve(scope, Request1(scope, holder.tag1), Request2(scope, holder.tag2), Request3(scope, holder.tag3));

            GetAttribute().IsInjected = true;
            Debug.Log($"\\\\ INJECT_FIN : [into='{this.GetType().Name}']");
        }

        public REQUEST_3 Request3(IScope scope, string tag = "") {
            UnityEngine.Debug.Log($"@@ REQUEST_3 : ({typeof(REQUEST_3)})\n[into='{this.GetType().Name}'][fromScope='{scope.Name}']\n|>[request3=({typeof(REQUEST_3).Name},{Tags})]");

            var result = Exports<REQUEST_3>.Retrieve(scope, tag).Match<REQUEST_3>((r) => {
                return r;
            },
            (r) => {
                UnityEngine.Debug.LogError($"XX INJECT : [FAILED][3]\n[{typeof(REQUEST_3).Name}]\n[{r}]");

                return default(REQUEST_3);
            });

            return result;
        }
    }

    public interface IInject<REQUEST_1, REQUEST_2, REQUEST_3, REQUEST_4> : IInject<REQUEST_1, REQUEST_2, REQUEST_3> {
        virtual new (string tag1, string tag2, string tag3, string tag4) Tags { get => ("", "", "", ""); }

        void IInject<REQUEST_1, REQUEST_2, REQUEST_3>.Recieve(IScope scope, REQUEST_1 request1, REQUEST_2 request2, REQUEST_3 request3) {
            throw new InvalidOperationException("Using level below inherited interface, please use method with matching generic arguments as interface.");
        }

        public void Recieve(IScope scope, REQUEST_1 request1, REQUEST_2 request2, REQUEST_3 request3, REQUEST_4 request4);

        public new void Inject(IScope scope) {
            if (GetAttribute().IsInjected)
                return;

            var holder = Tags;
            UnityEngine.Debug.Log($"// INJECT : [into='{this.GetType().Name}']\n[fromScope='{scope.Name}']\n|>[request1=({typeof(REQUEST_1).Name},{Tags.tag1})]\n|>[request2=({typeof(REQUEST_2).Name},{Tags.tag2})]\n|>[request3=({typeof(REQUEST_3).Name},{Tags.tag3})]\n|>[request4=({typeof(REQUEST_4).Name},{Tags.tag4})]");
            Recieve(scope, Request1(scope, holder.tag1), Request2(scope, holder.tag2), Request3(scope, holder.tag3), Request4(scope, holder.tag4));

            GetAttribute().IsInjected = true;
            Debug.Log($"\\\\ INJECT_FIN : [into='{this.GetType().Name}']");
        }

        public REQUEST_4 Request4(IScope scope, string tag = "") {
            UnityEngine.Debug.Log($"@@ REQUEST_4 : ({typeof(REQUEST_4)})\n[into='{this.GetType().Name}'][fromScope='{scope.Name}']\n|>[request4=({typeof(REQUEST_4).Name},{Tags})]");

            var result = Exports<REQUEST_4>.Retrieve(scope, tag).Match<REQUEST_4>((r) => {
                return r;
            },
            (r) => {
                UnityEngine.Debug.LogError($"XX INJECT : [FAILED][4]\n[{typeof(REQUEST_4).Name}]\n[{r}]");

                return default(REQUEST_4);
            });

            return result;
        }
    }

    public interface IInject<REQUEST_1, REQUEST_2, REQUEST_3, REQUEST_4, REQUEST_5> : IInject<REQUEST_1, REQUEST_2, REQUEST_3, REQUEST_4> {
        virtual new (string tag1, string tag2, string tag3, string tag4, string tag5) Tags { get => ("", "", "", "", ""); }

        void IInject<REQUEST_1, REQUEST_2, REQUEST_3, REQUEST_4>.Recieve(IScope scope, REQUEST_1 request1, REQUEST_2 request2, REQUEST_3 request3, REQUEST_4 request4) {
            throw new InvalidOperationException("Using level below inherited interface, please use method with matching generic arguments as interface.");
        }

        public void Recieve(IScope scope, REQUEST_1 request1, REQUEST_2 request2, REQUEST_3 request3, REQUEST_4 request4, REQUEST_5 request5);

        public new void Inject(IScope scope) {
            if (GetAttribute().IsInjected)
                return;

            var holder = Tags;
            UnityEngine.Debug.Log($"// INJECT : [into='{this.GetType().Name}']\n[fromScope='{scope.Name}']\n|>[request1=({typeof(REQUEST_1).Name},{Tags.tag1})]\n|>[request2=({typeof(REQUEST_2).Name},{Tags.tag2})]\n|>[request3=({typeof(REQUEST_3).Name},{Tags.tag3})]\n|>[request4=({typeof(REQUEST_4).Name},{Tags.tag4})]\n|>[request5=({typeof(REQUEST_5).Name},{Tags.tag5})]");
            Recieve(scope, Request1(scope, holder.tag1), Request2(scope, holder.tag2), Request3(scope, holder.tag3), Request4(scope, holder.tag4), Request5(scope, holder.tag5));

            GetAttribute().IsInjected = true;
            Debug.Log($"\\\\ INJECT_FIN : [into='{this.GetType().Name}']");
        }

        public REQUEST_5 Request5(IScope scope, string tag = "") {
            UnityEngine.Debug.Log($"@@ REQUEST_5 : ({typeof(REQUEST_5)})\n[into='{this.GetType().Name}'][fromScope='{scope.Name}']\n|>[request5=({typeof(REQUEST_5).Name},{Tags})]");

            var result = Exports<REQUEST_5>.Retrieve(scope, tag).Match<REQUEST_5>((r) => {
                return r;
            },
            (r) => {
                UnityEngine.Debug.LogError($"XX INJECT : [FAILED][5]\n[{typeof(REQUEST_5).Name}]\n[{r}]");

                return default(REQUEST_5);
            });

            return result;
        }
    }

    public static class IAmInjecting_Extended {
        public static IInject<SERVICE_1, SERVICE_2>
            Interface<SERVICE_1, SERVICE_2>(
            this IInject<SERVICE_1, SERVICE_2> inst) {

            return inst;
        }

        public static IInject<SERVICE_1, SERVICE_2, SERVICE_3>
            Interface<SERVICE_1, SERVICE_2, SERVICE_3>(
            this IInject<SERVICE_1, SERVICE_2, SERVICE_3> inst) {

            return inst;
        }

        public static IInject<SERVICE_1, SERVICE_2, SERVICE_3, SERVICE_4>
            Interface<SERVICE_1, SERVICE_2, SERVICE_3, SERVICE_4>(
            this IInject<SERVICE_1, SERVICE_2, SERVICE_3, SERVICE_4> inst) {

            return inst;
        }

        public static IInject<SERVICE_1, SERVICE_2, SERVICE_3, SERVICE_4, SERVICE_5>
            Interface<SERVICE_1, SERVICE_2, SERVICE_3, SERVICE_4, SERVICE_5>(
            this IInject<SERVICE_1, SERVICE_2, SERVICE_3, SERVICE_4, SERVICE_5> inst) {

            return inst;
        }

        //		public static void ReceiveScope<SERVICES>(this IAmInjecting reference, IScope scope) {
        //			reference.Scope = scope;
        //		}
    }

    public class TestInjectable : IInject<global::Serilog.ILogger> {
        string IInject<global::Serilog.ILogger>.Tags { get => ""; }
        private (global::Serilog.ILogger _log, IScope scope) _services;
        public (global::Serilog.ILogger _log, IScope scope) Services { get => _services; set => _services = value; }

        private IScope _scope;
        public IScope Scope { get => _scope; set => _scope = value; }
        public bool IsInjected { get; set; }

        public void Recieve(IScope scope, global::Serilog.ILogger request1) {
            throw new NotImplementedException();
        }
    }

    public class TestInjectable2 : IInject<global::Serilog.ILogger, IScope> {
        (string, string) IInject<global::Serilog.ILogger, IScope>.Tags { get => ("", ""); }
        private (global::Serilog.ILogger _log, IScope scope) _services;
        public (global::Serilog.ILogger _log, IScope scope) Services { get => _services; set => _services = value; }

        private IScope _scope;
        public IScope Scope { get => _scope; set => _scope = value; }
        public bool IsInjected { get; set; }

        public void Recieve(IScope scope, global::Serilog.ILogger request1) {
            throw new NotImplementedException();
        }

        public void Recieve(IScope scope, global::Serilog.ILogger request1, IScope request2) {
            throw new NotImplementedException();
        }
    }
}
