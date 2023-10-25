using System.Collections.Generic;

using Levels.Universal.General;

using UnityEngine.Playables;

namespace Levels.Universal.Experimental {
	public struct NotificationCheckers {
		public IScope Scope;
		public readonly List<(object, IAmChecker)> Checkers;

		public NotificationCheckers(
			IScope scope) {

			Scope = scope;
			Checkers = new List<(object, IAmChecker)>();
		}

		public void Check(
			Playable origin,
			UnityEngine.Playables.INotification notification,
			object context,
			out bool cancelToken) {
			bool cancelFlag = false;

			foreach (int i in Checkers.Count) {
				if (Checkers[i].Item2.Check(origin, notification, context).Value) {
					cancelFlag = true;
				}
			}

			cancelToken = cancelFlag;
		}

		public void SubChecker(
			object src,
			IAmChecker toSub) {

			// TODO: Check that this is comparing the checker correctly since it is a struct.
			if (Checkers.Contains((src, toSub)))
				return;

			Checkers.
				Add((src, toSub));
		}


		public void UnsubChecker(
			object src,
			IAmChecker toUnsub) {

			if (!Checkers.Contains((src, toUnsub)))
				return;

			Checkers.
				Remove((src, toUnsub));
		}
	}
}
