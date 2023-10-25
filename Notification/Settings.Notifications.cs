namespace Levels.Universal {
    using System;

    public static partial class Settings {
        [System.AttributeUsage(System.AttributeTargets.Method)]
        public class Notifications : System.Attribute {
            public bool SubscribeUp { get; set; } = false;
            public UnityEngine.PropertyName Filter = new UnityEngine.PropertyName("");
            public Type[] Hooks;

            public Notifications() {

            }

            public Notifications(
                bool subscribeUp,
                string filter,
                Type[] hooks) {
                SubscribeUp = subscribeUp;
                Filter = filter;
                Hooks = hooks;
            }
        }
    }
}
