using System;

namespace Levels.Universal {
	using ScopeKey = String.ID<DI.Scope>;
	public partial class DI {
		public interface IsInjectable {
			public void Inject(ScopeKey scope);

			public DI.Settings Settings { get; set; }
		}

		public interface IInject<T, RQ1> : IsInjectable {
			virtual string Tags {
				get => "";
			}
			virtual string Keys {
				get => "";
			}

			public void Recieve(ScopeKey scope, RQ1 request1);

			void IsInjectable.Inject(ScopeKey scope) {
				if (Settings.IsInjected)
					return;

				Console.WriteLine($"// INJECT : [into='{this.GetType().Name}']\n[fromScope='{scope.ToString()}']\n|>[request1=({typeof(RQ1).Name},{Tags})]");
				Recieve(scope, Request1(scope, Keys, Tags));

				Settings.IsInjected = true;
				Console.WriteLine($"\\\\ INJECT_FIN : [into='{this.GetType().Name}']");
			}

			public RQ1 Request1(ScopeKey scope, string key = "", string tag = "") {
				Console.WriteLine($"@@ REQUEST_1 :({typeof(RQ1)})\n[into='{this.GetType().Name}'][fromScope='{scope.ToString()}']\n|>[request1=({typeof(RQ1).Name},{Tags})]");

				var result = Exports<RQ1>.Retrieve<T>(this, scope, Settings.UseLocalSettings ? Settings : null, key, tag).Match<RQ1>((r) => {
					return r;
				},
				(r) => {
					Console.WriteLine($"XX INJECT : [FAILED][1]\n[{typeof(RQ1).Name}]\n[{r}]");

					return default(RQ1);
				});

				return result;
			}
		}

		public interface IInject<T, RQ1, RQ2> : IInject<T, RQ1> {
			virtual new (string one, string two) Keys {
				get => ("", "");
			}
			virtual new (string one, string two) Tags {
				get => ("", "");
			}

			void IInject<T, RQ1>.Recieve(ScopeKey scope, RQ1 request1) {
				throw new InvalidOperationException("Using level below inherited interface, please use method with matching generic arguments as interface.");
			}

			public void Recieve(ScopeKey scope, RQ1 request1, RQ2 request2);

			void IsInjectable.Inject(ScopeKey scope) {
				if (Settings.IsInjected)
					return;

				Console.WriteLine($"// INJECT : [into='{this.GetType().Name}']\n[fromScope='{scope.ToString()}']\n|>[request1=({typeof(RQ1).Name},{Tags.one})]\n|>[request2=({typeof(RQ2).Name},{Tags.two})]");
				Recieve(
					scope,
					Request1(scope, Keys.one, Tags.one),
					Request2(scope, Keys.two, Tags.two));

				Settings.IsInjected = true;
				Console.WriteLine($"\\\\ INJECT_FIN : [into='{this.GetType().Name}']");
			}

			public RQ2 Request2(ScopeKey scope, string key = "", string tag = "") {
				Console.WriteLine($"@@ REQUEST_2 : ({typeof(RQ2)})\n[into='{this.GetType().Name}'][fromScope='{scope.ToString()}']\n|>[request2=({typeof(RQ2).Name},{Tags})]");

				var result = Exports<RQ2>.Retrieve<T>(this, scope, Settings.UseLocalSettings ? Settings : null, key, tag).Match<RQ2>((r) => {
					return r;
				},
				(r) => {
					Console.WriteLine($"XX INJECT : [FAILED][2]\n[{typeof(RQ2).Name}]\n[{r}]");

					return default(RQ2);
				});

				return result;
			}
		}

		public interface IInject<T, RQ1, RQ2, RQ3> : IInject<T, RQ1, RQ2> {
			virtual new (string key1, string key2, string key3) Keys {
				get => ("", "", "");
			}
			virtual new (string tag1, string tag2, string tag3) Tags {
				get => ("", "", "");
			}

			void IInject<T, RQ1, RQ2>.Recieve(ScopeKey scope, RQ1 request1, RQ2 request2) {
				throw new InvalidOperationException("Using level below inherited interface, please use method with matching generic arguments as interface.");
			}

			public void Recieve(ScopeKey scope, RQ1 request1, RQ2 request2, RQ3 request3);

			void IsInjectable.Inject(ScopeKey scope) {
				if (Settings.IsInjected)
					return;

				Console.WriteLine($"// INJECT : [into='{this.GetType().Name}']\n[fromScope='{scope.ToString()}']\n|>[request1=({typeof(RQ1).Name},{Tags.tag1})]\n|>[request2=({typeof(RQ2).Name},{Tags.tag2})]\n|>[request3=({typeof(RQ3).Name},{Tags.tag3})]");
				Recieve(
					scope,
					Request1(scope, Keys.key1, Tags.tag1),
					Request2(scope, Keys.key2, Tags.tag2),
					Request3(scope, Keys.key3, Tags.tag3));

				Settings.IsInjected = true;
				Console.WriteLine($"\\\\ INJECT_FIN : [into='{this.GetType().Name}']");
			}

			public RQ3 Request3(ScopeKey scope, string key = "", string tag = "") {
				Console.WriteLine($"@@ REQUEST_3 : ({typeof(RQ3)})\n[into='{this.GetType().Name}'][fromScope='{scope.ToString()}']\n|>[request3=({typeof(RQ3).Name},{key},{Tags})]");

				var result = Exports<RQ3>.Retrieve<T>(this, scope, Settings.UseLocalSettings ? Settings : null, key, tag).Match<RQ3>((r) => {
					return r;
				},
				(r) => {
					Console.WriteLine($"XX INJECT : [FAILED][3]\n[{typeof(RQ3).Name}]\n[{r}]");

					return default(RQ3);
				});

				return result;
			}
		}

		public interface IInject<T, RQ1, RQ2, RQ3, RQ4> : IInject<T, RQ1, RQ2, RQ3> {
			virtual new (string key1, string key2, string key3, string key4) Keys {
				get => ("", "", "", "");
			}
			virtual new (string tag1, string tag2, string tag3, string tag4) Tags {
				get => ("", "", "", "");
			}

			void IInject<T, RQ1, RQ2, RQ3>.Recieve(ScopeKey scope, RQ1 request1, RQ2 request2, RQ3 request3) {
				throw new InvalidOperationException("Using level below inherited interface, please use method with matching generic arguments as interface.");
			}

			public void Recieve(ScopeKey scope, RQ1 request1, RQ2 request2, RQ3 request3, RQ4 request4);

			public new void Inject(ScopeKey scope) {
				if (Settings.IsInjected)
					return;

				var holder = Tags;
				Console.WriteLine($"// INJECT : [into='{this.GetType().Name}']\n[fromScope='{scope.ToString()}']\n|>[request1=({typeof(RQ1).Name},{Tags.tag1})]\n|>[request2=({typeof(RQ2).Name},{Tags.tag2})]\n|>[request3=({typeof(RQ3).Name},{Tags.tag3})]\n|>[request4=({typeof(RQ4).Name},{Tags.tag4})]");
				Recieve(
					scope,
					Request1(scope, Keys.key1, Tags.tag1),
					Request2(scope, Keys.key2, Tags.tag2),
					Request3(scope, Keys.key3, Tags.tag3),
					Request4(scope, Keys.key4, Tags.tag4));

				Settings.IsInjected = true;
				Console.WriteLine($"\\\\ INJECT_FIN : [into='{this.GetType().Name}']");
			}

			public RQ4 Request4(ScopeKey scope, string key = "", string tag = "") {
				Console.WriteLine($"@@ REQUEST_4 : ({typeof(RQ4)})\n[into='{this.GetType().Name}'][fromScope='{scope.ToString()}']\n|>[request4=({typeof(RQ4).Name},{Tags})]");

				var result = Exports<RQ4>.Retrieve<T>(this, scope, Settings.UseLocalSettings ? Settings : null, key, tag).Match<RQ4>((r) => {
					return r;
				},
				(r) => {
					Console.WriteLine($"XX INJECT : [FAILED][4]\n[{typeof(RQ4).Name}]\n[{r}]");

					return default(RQ4);
				});

				return result;
			}
		}

		public interface IInject<T, RQ1, RQ2, RQ3, RQ4, RQ5> : IInject<T, RQ1, RQ2, RQ3, RQ4> {
			virtual new (string key1, string key2, string key3, string key4, string key5) Keys {
				get => ("", "", "", "", "");
			}
			virtual new (string tag1, string tag2, string tag3, string tag4, string tag5) Tags {
				get => ("", "", "", "", "");
			}

			void IInject<T, RQ1, RQ2, RQ3, RQ4>.Recieve(ScopeKey scope, RQ1 request1, RQ2 request2, RQ3 request3, RQ4 request4) {
				throw new InvalidOperationException("Using level below inherited interface, please use method with matching generic arguments as interface.");
			}

			public void Recieve(ScopeKey scope, RQ1 request1, RQ2 request2, RQ3 request3, RQ4 request4, RQ5 request5);

			public new void Inject(ScopeKey scope) {
				if (Settings.IsInjected)
					return;

				Console.WriteLine($"// INJECT : [into='{this.GetType().Name}']\n[fromScope='{scope.ToString()}']\n|>[request1=({typeof(RQ1).Name},{Tags.tag1})]\n|>[request2=({typeof(RQ2).Name},{Tags.tag2})]\n|>[request3=({typeof(RQ3).Name},{Tags.tag3})]\n|>[request4=({typeof(RQ4).Name},{Tags.tag4})]\n|>[request5=({typeof(RQ5).Name},{Tags.tag5})]");
				Recieve(
					scope,
					Request1(scope, Keys.key1, Tags.tag1),
					Request2(scope, Keys.key2, Tags.tag2),
					Request3(scope, Keys.key3, Tags.tag3),
					Request4(scope, Keys.key4, Tags.tag4),
					Request5(scope, Keys.key5, Tags.tag5));

				Settings.IsInjected = true;
				Console.WriteLine($"\\\\ INJECT_FIN : [into='{this.GetType().Name}']");
			}

			public RQ5 Request5(ScopeKey scope, string key = "", string tag = "") {
				Console.WriteLine($"@@ REQUEST_5 : ({typeof(RQ5)})\n[into='{this.GetType().Name}'][fromScope='{scope.ToString()}']\n|>[request5=({typeof(RQ5).Name},{Tags})]");

				var result = Exports<RQ5>.Retrieve<T>(this, scope, Settings.UseLocalSettings ? Settings : null, key, tag).Match<RQ5>((r) => {
					return r;
				},
				(r) => {
					Console.WriteLine($"XX INJECT : [FAILED][5]\n[{typeof(RQ5).Name}]\n[{r}]");

					return default(RQ5);
				});

				return result;
			}
		}

		public class TestInjectable : IInject<TestInjectable, global::Serilog.ILogger> {
			string IInject<TestInjectable, global::Serilog.ILogger>.Tags {
				get => "";
			}
			private (global::Serilog.ILogger _log, ScopeKey scope) _services;
			public (global::Serilog.ILogger _log, ScopeKey scope) Services {
				get => _services; set => _services = value;
			}

			private ScopeKey _scope;
			public ScopeKey Scope {
				get => _scope; set => _scope = value;
			}
			public bool IsInjected {
				get; set;
			}

			public void Recieve(ScopeKey scope, global::Serilog.ILogger request1) {
				throw new NotImplementedException();
			}

			public Settings Settings { get; set; }
		}

		public class TestInjectable2 : IInject<TestInjectable2, global::Serilog.ILogger, ScopeKey> {
			(string, string) IInject<TestInjectable2, global::Serilog.ILogger, ScopeKey>.Tags {
				get => ("", "");
			}
			private (global::Serilog.ILogger _log, ScopeKey scope) _services;
			public (global::Serilog.ILogger _log, ScopeKey scope) Services {
				get => _services; set => _services = value;
			}

			private ScopeKey _scope;
			public ScopeKey Scope {
				get => _scope; set => _scope = value;
			}
			public bool IsInjected {
				get; set;
			}

			public void Recieve(ScopeKey scope, global::Serilog.ILogger request1) {
				throw new NotImplementedException();
			}

			public void Recieve(ScopeKey scope, global::Serilog.ILogger request1, ScopeKey request2) {
				throw new NotImplementedException();
			}

			public Settings Settings { get; set; }
		}
	}

	public static class IInject_Extended {
		public static DI.IInject<T, S1>
		Interface<T, S1>(
		this DI.IInject<T, S1> inst) {

			return inst;
		}

		public static DI.IInject<T, S1, S2>
			Interface<T, S1, S2>(
			this DI.IInject<T, S1, S2> inst) {

			return inst;
		}

		public static DI.IInject<T, S1, S2, S3>
			Interface<T, S1, S2, S3>(
			this DI.IInject<T, S1, S2, S3> inst) {

			return inst;
		}

		public static DI.IInject<T, S1, S2, S3, S4>
			Interface<T, S1, S2, S3, S4>(
			this DI.IInject<T, S1, S2, S3, S4> inst) {

			return inst;
		}

		public static DI.IInject<T, S1, S2, S3, S4, S5>
			Interface<T, S1, S2, S3, S4, S5>(
			this DI.IInject<T, S1, S2, S3, S4, S5> inst) {

			return inst;
		}
	}
}
