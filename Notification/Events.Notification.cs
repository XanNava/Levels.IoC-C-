using Levels.Universal;

namespace Levels.Universal {
	using System;

	using Levels.Universal.Functional;
    using UnityEngine.Playables;

    using UnityEngine;

    public interface INotification : ISetup<INotification, (Playable origin, PropertyName filter, object context)>, UnityEngine.Playables.INotification {
        public Playable Origin { get; }

        public PropertyName Filter { get; }

        public Type ContextType { get; }

        public object Context { get; protected set; }

		public string Readout();
    }

	/// <summary>
	/// The default <see cref="INotification"/> inheritable type. Seal all end derived types or performance.
	/// </summary>
    public class Notification : INotification {
		private (Playable origin, PropertyName filter, object context) _args;

		public ISetup<INotification, (Playable origin, PropertyName filter, object context)> _Setup { get; set; }

		public INotification Setup((Playable origin, PropertyName filter, object context) args, INotification source = null) {
			_args.origin = args.origin;
			_args.filter = args.filter;
			_args.context = args.context;

			return this;
		}

		public string Readout() {
			return $"$[{this.GetType()}][Origin='{_args.origin}'][Filter='{_args.filter}'][Context='{_args.context}']#";
		}

		Playable INotification.Origin {
			get {
				return _args.origin;
			}
		}

		Type INotification.ContextType {
			get {
				return _args.context.GetType();
			}
		}

		object INotification.Context {
			get {
				return _args.context;
			}
			set {
                _args.context = value;
            }
		}

		public PropertyName id => _args.filter;

        public PropertyName Filter => _args.filter;
	}
}

public static class Notification_Extends {
	public static INotification Interface_INotification(this Notification notif) {
		return notif as INotification;
	}
}