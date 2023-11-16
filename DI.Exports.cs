using System;
using System.Collections.Generic;
using System.Text;

using LanguageExt.Common;

namespace Levels.Universal {
	using ScopeKey = String.ID<DI.Scope>;
	public partial class DI {
		public static class Exports<T> {
			public static DI.Settings DefaultSettings;

			private readonly static Dictionary<ScopeKey, IRegisteryTable<T>> _registeries = new();

			public static Dictionary<ScopeKey, IRegisteryTable<T>> Registeries {
				get => _registeries;
			}

			private static StringBuilder _readoutBuilder { get; } = new StringBuilder();

			public static Result<T> Retrieve(object source, ScopeKey scope, DI.Settings localSettings = null, string key = "", string tag = "") {
				return Retrieve<T>(source, scope, localSettings, key, tag);
			}

			public static Result<T> Retrieve<S>(object source, ScopeKey scope, DI.Settings localSettings = null, string key = "", string tag = "") {
				SetupLocalSettings(ref localSettings);

				bool doRetrieveFromParents = localSettings.DoRetrieveFromParents;

				T value = default;

				Console.WriteLine($">> RETRIEVING : (type={typeof(T).Name}, key='{key}')\n[scope='{scope.ToString()}']\n");

				bool isRetrieved = RetrieveFromRegistered<S>(ref value, source, scope, localSettings, key, tag)
								|| RetrieveFromParent<S>(ref value, source, scope, localSettings, key, tag)
								|| RetrieveDefault(ref value);

				if (isRetrieved) {
					Console.WriteLine($"<< RETRIEVED : (type={typeof(T).Name}, key='{key}')\n[fromScope='{scope.ToString()}']\n");
					return value;
				}

				throw new KeyNotFoundException($"The was no entry for scope='{scope.ToString()}' for Type='{typeof(T)}' and tag='{key}'");
			}

			private static void SetupLocalSettings(ref DI.Settings settings) {
				if (settings == null) {
					settings ??= Exports<T>.DefaultSettings;
					settings ??= new DI.Settings();
				}
			}

			private static bool RetrieveFromRegistered<S>(ref T value, object source, ScopeKey scope, DI.Settings settings, string key, string tag) {
				bool useKeyAsTag = settings.UseKeyAsTag;
				bool tryDefaultKey = settings.UseDefaultKeyOnMissing;
				bool isRetrieved = false;

				if (Registeries.ContainsKey(scope)) // Note: Key as in dictionary key.
				{
					var result = Registeries[scope].Retrieve(key, useKeyAsTag ? key : tag, source).Match<T>(
						(r) => {
							isRetrieved = true;
							return r;
						}
						, (e) => {
							Console.WriteLine(e.Message);
							isRetrieved = false;
							return default(T);
						}
					);

					if (isRetrieved) {
						value = result;
						return true;
					}

					if (!tryDefaultKey)
						return false;

					result = Registeries[scope].Retrieve(key, useKeyAsTag ? key : tag).Match<T>(
						(r) => {
							isRetrieved = true;
							return r;
						}
						, (e) => {
							Console.WriteLine(e.Message);
							isRetrieved = false;
							return default(T);
						}
					);

					if (isRetrieved) {
						value = result;
						return true;
					}

					return false;
				}

				return false;
			}

			private static bool RetrieveFromParent<S>(ref T value, object source, String.ID<DI.Scope> scope, DI.Settings settings, string key, string tag) {
				if (!settings.DoRetrieveFromParents)
					return false;

				bool isRetrieved = false;

				if (scope.GetParentID() != default) {
					Console.WriteLine($"RETRIEVING_PARENT : (type={typeof(T).Name}, key='{key}')\n[scope='{scope.GetParentID().ToString()}']\n");
					var retrieved = Retrieve<S>(source, scope.GetParentID(), settings, key, tag).Match<T>(
						(r) => {
							isRetrieved = true;
							return r;
						},
						(e) => {
							Console.WriteLine(e.Message);
							isRetrieved = false;
							return default;
						}
					);

					if (isRetrieved) {
						value = retrieved;
						return true;
					}
				}

				return false;
			}

			private static bool RetrieveDefault(ref T value) {
				if (typeof(T).IsClass && typeof(T).GetConstructor(Type.EmptyTypes) != null) {
					value = (T)Activator.CreateInstance(typeof(T));
					return true;
				}
				else if (typeof(T).IsValueType) {
					value = default(T);
					return true;
				}

				// Will return false if isClass and doesn't have empty constructor.
				return false;
			}

			public static void Register(ScopeKey scope, IEntry<T> reg) {
				if (!Registeries.ContainsKey(scope)) {
					Registeries.Add(scope, new Registeries<T>(scope, _readoutBuilder));
				}

				Registeries[scope].Register(reg);
			}

			public static void TryRegister(ScopeKey scope, object reg) {
				if (reg.GetType().GetInterface(nameof(IEntry<T>)) != null) {
					throw new InvalidCastException($"The reg object passed to Register needs to be of type IRegistery<T>, but was of type {reg.GetType()}");
				}

				var regInst = reg as IEntry<T>;

				if (!Registeries.ContainsKey(scope)) {
					Registeries.Add(scope, new Registeries<T>(scope, _readoutBuilder));
				}

				Registeries[scope].Register(regInst);
			}
		}
	}

	public static class StringIDScope_Extends {
		public static ScopeKey GetParentID(this ScopeKey id) {
			var scope = DI.Scope.References.GetScope(id);

			return scope.Parent;
		}

		public static DI.Scope GetScope(this ScopeKey id) {
			return DI.Scope.References.GetScope(id);
		}
	};
}
