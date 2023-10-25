
using UnityEngine;
using UnityEngine.Playables;

namespace Levels.Universal {
    public interface INotificationChecker {
        public PropertyName Handle { set; get; }
        public INotificationReceiver Check(Events.INotificationHandler handler, INotification notice, out bool CancelToken);
    }

    // TODO: Move

}