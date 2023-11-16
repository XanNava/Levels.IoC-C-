namespace Levels.Universal {
	using System;

	public static partial class DI {
		[System.AttributeUsage(System.AttributeTargets.Class | AttributeTargets.Interface)]
		public sealed class Settings
			: System.Attribute {
			//<todo> implement these </todo>
			public bool UseLocalSettings = false;

			public bool DoInjectMethods = false;
			public bool DoInjectConstructor = true;
			public bool DoInjectInterface = false;
			public bool DoInjectPublicFields = false;
			public bool RegisterNotificationReceiver = true;

			/// <summary>
			/// Will default to key="" if the Registery has no entry with provided key.
			/// </summary>
			public bool UseDefaultKeyOnMissing = false;

			/// <summary>
			/// Will pass the key to the type being injected as tags.
			/// </summary>
			public bool UseKeyAsTag = false;

			[Obsolete("TODO: setup reflection injection.")]
			public bool DoAutoInject = true;
			public bool IsInjected = false;

			public bool DoRetrieveFromRoot = false;
			public bool DoRetrieveFromParents = true;

			[Obsolete("Create Instance Entry Instead")]
			public bool DoCreateInstance = true;

			public bool GetDoInjectPublicFields() {
				return DoInjectPublicFields;
			}

			public string Readout() {
				return $"[InjectInterface:{DoInjectInterface}][InjectPublicFields:{DoInjectPublicFields}][DefaultTags:{UseDefaultKeyOnMissing}]";
			}
		}
	}
}