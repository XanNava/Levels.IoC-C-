using System.Collections.Generic;

using Levels.Universal.General;

using UnityEngine.Playables;

namespace Levels.Universal.Experimental {
	public interface IScope : INotificationReceiver {
		public static readonly List<(IScope scope, object source)> ScopeRegistery = new List<(IScope scope, object source)>();

		private static IScope _root;
		public static IScope Root {
			get {
				if (_root == default(IScope)) {
					_root = new Scope("root", default(IScope), new Settings.DI());
				}

				return _root;
			}
			set {
				_root = value;
			}
		}

		public IScope Parent { get; }

		public string Name { get; }

		public Settings.DI DefaultDISettings { get; }

		public void InjectAll(IsInjectable[] Injections) {
			foreach (int i in Injections.Length) {
				Injections[i]?.Inject(this);
			}
		}

		public void Inject(IsInjectable Injections) {
			Injections.Inject(this);
		}

		public void RegisterNotifiables(INotificationRecieverWrapper[] notifiables) {
			foreach (var notifiable in notifiables) {
				notifiable.RegisterToScope(this);
			}
		}

		public void CheckHandler<T>() where T : UnityEngine.Playables.INotification;

		public void PublishNotification<T>(Playable origin, INotification notification, object context) where T : UnityEngine.Playables.INotification;
	}

	public static class IScope_Extends {
		public static IScope Interface(this IScope scope) {
			return scope;
		}
	}

	public sealed class Scope : IScope {
		private IScope _parent;
		/// <summary>
		/// Will be default if there is no parent.
		/// </summary>
		public IScope Parent { get => _parent; }

		private Settings.DI _defaultDISettings;
		public Settings.DI DefaultDISettings { get => _defaultDISettings; }

		private string _name;
		public string Name => _name;

		public Scope(string name, IScope parent, Settings.DI defaultDISettings) {
			_defaultDISettings = defaultDISettings;

			_parent = parent;
			_name = name;
		}

		public void CheckHandler<T>() where T : UnityEngine.Playables.INotification {
			NotificationBoard<T>.Service.CheckHandler(this);
		}

		public void PublishNotification<T>(Playable origin, INotification notification, object context) where T : UnityEngine.Playables.INotification {
			NotificationBoard<T>.Service.OnNotifyScope(this, origin, notification, context);
		}

		public void OnNotify(Playable origin, UnityEngine.Playables.INotification notification, object context) {
			NotificationBoard<UnityEngine.Playables.INotification>.Service.OnNotifyScope(this, origin, notification, context);
		}

		public bool HasParent() {
			return _parent.Equals(default(IScope));
		}
	}
}
