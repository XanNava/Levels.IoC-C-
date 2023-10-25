namespace Levels.Universal {
    using System;

    public static partial class Settings {
        [System.AttributeUsage(System.AttributeTargets.Class | AttributeTargets.Interface)]
        public sealed class DI : System.Attribute {
            //<todo> implement these </todo>
            public bool InjectMethods = false;
            public bool InjectConstructor = true;
            public bool InjectInterface = false;
            public bool InjectPublicFields = false;
            public bool RegisterNotificationReceiver = true;

            /// <summary>
            /// Will default to tag="" if this injection has no service with provided tag exists.
            /// </summary>
            public bool DefaultTags = false;

            /// <summary>
            /// Will pass the tag to the type being injected, and the default tag "" will be used for retrevial.
            /// TODO: Split the functionality, have the Default Tag work with this.
            /// </summary>
            public bool PassTags = false;

            public bool AutoInject = true;
            public bool IsInjected = false;

            public bool RetrieveFromRoot = false;
            public bool RetrieveFromParents = true;
            public bool CreateInstance = true;

            public bool UsePublicFields() {
                return InjectPublicFields;
            }

            public string Readout() {
                return $"[InjectInterface:{InjectInterface}][InjectPublicFields:{InjectPublicFields}][DefaultTags:{DefaultTags}]";
            }
        }
    }
}