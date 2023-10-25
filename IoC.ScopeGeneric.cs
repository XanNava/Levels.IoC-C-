namespace Levels.Universal {
	using System;
	using System.Collections.Generic;

	public sealed partial class IoC {
		/// <summary>
		/// A more performant, but less flexible, version of <see cref="Scope"/>.
		/// </summary>
		/// <typeparam name="T">The type this scope handles.</typeparam>
		/// <use>
		/// I default value should be implemented for each type that is used.
		/// Note I can't do this be hand as I would have to constraint T which would
		/// Limit the usefulness of this class
		/// </use>
		/// <note>
		/// This really will only be needed if performance is being hit to hard, which I could
		/// see happening if like 10k objects are being injected at once.
		/// </note>
		/// <todo>Implement this further.</todo>
		public class Scope<T> {
			public readonly Dictionary<string, Func<T>> Entries = new Dictionary<string, Func<T>>();

			public T TryRetrieve(string tag, bool defaultTag = true) {
				if (Entries.ContainsKey(tag)) {
					return Entries[tag].Invoke();
				}

				return Entries[""].Invoke();
			}

			public Scope<T> TryRegister(string tag, Func<T> creator) {
				Entries.TryAdd(tag, creator);

				return this;
			}

			public Scope<T> Inject<V>(V target) {
				// TODO: Implement from Pool
				return this;
			}
		}
	}
}