using System;
using System.Collections.Generic;
using System.Text;

using LanguageExt.Common;

using Levels.Universal.General;

namespace Levels.Universal {
	using ScopeKey = String.ID<DI.Scope>;
	public partial class DI {
		public interface IRegisteryTable<T> {
			public ScopeKey Scope {
				get;
			}
			public List<IEntry<T>> Collection {
				get;
			}
			public DI.Settings DefaultDISettings {
				get; set;
			}
			StringBuilder LogBuilder {
				get;
			}

			public IRegisteryTable<T> Register<I>(I reg) where I : IEntry<T>, IEquatable<I>;

			public Result<T> Retrieve(string key, string tag = "", object source = null);

			public Result<T> Retrieve(EntryKey key, string tag = "");

			public bool Contains(string tag);

			public bool Contains(EntryKey key);

			public string Readout();
		}

		// TODO: make it so you can use Registery specific ID manager.
		public struct Registeries<T> : IRegisteryTable<T> {
			private readonly List<IEntry<T>> _collection;
			public readonly List<IEntry<T>> Collection {
				get => _collection;
			}

			private ScopeKey _scope;
			public ScopeKey Scope {
				get => _scope;
			}

			private DI.Settings _defaultDISettings;
			public DI.Settings DefaultDISettings {
				get => _defaultDISettings; set => _defaultDISettings = value;
			}

			private StringBuilder _logBuilder;
			public StringBuilder LogBuilder {
				get => _logBuilder;
			}

			public Registeries(ScopeKey scope, StringBuilder builder, DI.Settings defaultDISettings = null) {
				if (defaultDISettings == null)
					defaultDISettings = new DI.Settings();

				_collection = new List<IEntry<T>>();
				_logBuilder = builder;
				_scope = scope;
				_defaultDISettings = defaultDISettings;
			}

			public IRegisteryTable<T> Register<I>(I reg) where I : IEntry<T>, IEquatable<I> {
				Console.WriteLine($"++ REG : (type={typeof(T).Name}', key='{reg.Key.ID.ToString()}')\n[Scope='{Scope.ToString()}']\n|>{reg.Readout()}");
				if (!Contains(reg.Key)) {
					Collection.Add(reg);
				}

				return this;
			}

			public bool Contains(string tag) {
				var key = String.ID.Manager<EntryKey>.Reference.GetID(tag);

				// If the String.ID doesn't exist in the manager it will return the null stringID.
				if (key.Value == 0)
					return false;

				return Collection.FindIndex(r => r.Key.ID == key) >= 0;
			}

			public bool Contains(EntryKey key) {
				return Collection.FindIndex(r => r.Key.ID == key.ID) >= 0;
			}

			public Result<T> Retrieve(EntryKey key, string tag = "") {
				Console.WriteLine($"RETRIEVE : [RegisterTable] (key={key.ID.ToString()}, tag={tag})\n\n[{Readout()}]");

				if (Contains(key)) {
					return Collection.Find((t) => {

						return t.Key.Equals(key);
					}).Fufiller(this, Scope, tag);
				}

				throw new KeyNotFoundException(key.ID.ToString());
			}

			public Result<T> Retrieve(string key, string tag = "", object source = null) {
				return Retrieve(new EntryKey(key, ref source), tag);
			}

			public string Readout() {
				LogBuilder.Clear();

				LogBuilder.Append($"READOUT : [RegisteryTable]\n[scope='{Scope.ToString()}']\n|>[Type='{typeof(T).Name}'][Settings={DefaultDISettings.Readout()}]");
				foreach (int i in Collection.Count) {
					LogBuilder.AppendLine(Collection[i].Readout());
				}
				return LogBuilder.ToString();
			}
		}
	}
}
