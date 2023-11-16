using System;
using System.Collections.Generic;
using System.Diagnostics;

using LanguageExt.Common;

using Levels.Universal.General;

namespace Levels.Universal {
	using ScopeID = String.ID<DI.Scope>;
	public partial class DI {
		public interface IScope {
			public ScopeID ID { get; set; }
			public string Name { get; }
			public DI.Settings DefaultDISettings { get; }

			public ScopeID Parent { get; }

			public void InjectAll(IsInjectable[] Injections) {
				foreach (int i in Injections.Length) {
					Injections[i]?.Inject(ID);
				}
			}

			public void Inject(IsInjectable injections) {
				injections.Inject(ID);
			}

			public void BroadcastNotification<T>(T notification) where T : IoC.INotification, IEquatable<T>;
		}

		public struct Scope : IScope {
			private static ScopeID _root;
			public static ScopeID Root {
				get {
					if (_root == default(ScopeID)) {
						_root = new Scope("root", default(ScopeID), new DI.Settings()).ID;
					}

					return _root;
				}
				set {
					_root = value;
				}
			}

			private ScopeID _id;
			public ScopeID ID { get => _id; set => _id = value; }

			private string _name;
			public string Name => _name;

			private DI.Settings _defaultDISettings;
			public DI.Settings DefaultDISettings { get => _defaultDISettings; }

			private ScopeID _parent;
			ScopeID IScope.Parent { get => _parent; }
			public ScopeID Parent { get => _parent; }

			public Scope(
				string name,
				ScopeID parent = default,
				DI.Settings defaultDISettings = default) {

				_name = name;
				_id = String.ID.Manager<DI.Scope>.RequestID(_name);
				_parent = parent;
				_defaultDISettings = defaultDISettings;

				if (!References.collection.ContainsKey(_id))
					References.collection.Add(_id, this);
				else
					Debug.WriteLine("Scope.Collection already contains the key {1}", _id.ToString());
			}

			public void BroadcastNotification<T>(T notification) where T : IoC.INotification, IEquatable<T> {
				IoC.Notification.BroadcastNotificationMediators(notification);
			}

			public void Notify<T>(T notification) where T : IoC.INotification, IEquatable<T> {
				IoC.Notification.NotifyScopeMediator(_id, notification);
			}

			public bool HasParent() {
				return !_parent.Equals(default);
			}

			public static class References {
				public readonly static Dictionary<ScopeID, Scope> collection = new();

				public static bool AddScope(Scope scope) {
					if (collection.ContainsKey(scope._id)) {
						collection.Add(scope.ID, scope);
						return true;
					}
					return false;
				}

				public static Scope GetScope(ScopeID scope) {
					return collection[scope];
				}
			}

			public static class Factory {
				public static ScopeID CreateRoot() {
					if (Scope.Root != default) {
						var root = new Scope("root", default(ScopeID), new DI.Settings());
						References.AddScope(root);
					}

					return Scope.Root;
				}

				public static Result<ScopeID> CreateChildScope(string name, ScopeID parent) {
					var root = new Scope(name, parent, new DI.Settings());
					if (References.AddScope(root)) {
						return Scope.Root;
					}
					throw new ArgumentException("Scoped named {0} already existed", name);
				}

				public static ScopeID CreateScope(string name) {
					var root = new Scope(name, default(ScopeID), new DI.Settings());
					References.AddScope(root);

					return Scope.Root;
				}
			}
		}
	}

	public static class IScope_Extends {
		public static DI.IScope Interface_IScope(this DI.IScope scope) {
			return scope;
		}
	}
}
