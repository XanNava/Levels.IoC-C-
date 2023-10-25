using System.Collections.Generic;
using System.Text;

using Levels.Universal.General;
using UnityEngine;

namespace Levels.Universal.Experimental {

	public interface IRegisteryTable<T> {
		public IScope Scope { get; }
		public List<IEntry<T>> Registeries { get; }
		public Settings.DI DefaultDISettings { get; set; }
		StringBuilder LogBuilder { get; }

		public IRegisteryTable<T> Register(IEntry<T> reg) {
			Debug.Log($"++ REG : (type={typeof(T).Name}', key='{reg.Key.key.ToString()}')\n[Scope='{Scope.Name}']\n|>{reg.Readout()}");
			if (!Contains(reg.Key)) {
				Registeries.Add(reg);
			}

			return this;
		}

		public T Retrieve(string key, string tag = "") {
			return Retrieve(new EntryKey(key, null), tag);
		}

		public T Retrieve(EntryKey key, string tag = "") {
			Debug.Log($"RETRIEVE : [RegisterTable] (key={key.key.ToString()}, tag={tag})\n\n[{Readout()}]");

			if (Contains(key)) {
				return Registeries.Find((t) => {

					return t.Key.Equals(key);
				}).Fufiller(Scope, tag);
			}

			throw new KeyNotFoundException(key.key.ToString());
		}

		public bool Contains(string tag) {
			var key = new PropertyName(tag);
			return Registeries.FindIndex(r => r.Key.key == key) >= 0;
		}

		public bool Contains(EntryKey key) {
			return Registeries.FindIndex(r => r.Key.key == key.key) >= 0;
		}

		public string Readout() {
			LogBuilder.Clear();

			LogBuilder.Append($"READOUT : [RegisteryTable]\n[scope='{Scope.Name}']\n|>[Type='{typeof(T).Name}'][Settings={DefaultDISettings.Readout()}]");
			foreach (int i in Registeries.Count) {
				LogBuilder.AppendLine(Registeries[i].Readout());
			}
			return LogBuilder.ToString();
		}
	}

	public struct RegisteryTable<T> : IRegisteryTable<T> {
		private readonly List<IEntry<T>> _registeries;
		public readonly List<IEntry<T>> Registeries { get => _registeries; }

		private IScope _scope;
		public IScope Scope { get => _scope; }

		private Settings.DI _defaultDISettings;
		public Settings.DI DefaultDISettings { get => _defaultDISettings; set => _defaultDISettings = value; }

		private StringBuilder _logBuilder;
		public StringBuilder LogBuilder { get => _logBuilder; }

		public RegisteryTable(IScope scope, StringBuilder builder, Settings.DI defaultDISettings = null) {
			if (defaultDISettings == null)
				defaultDISettings = new Settings.DI();

			_registeries = new List<IEntry<T>>();
			_logBuilder = builder;
			_scope = scope;
			_defaultDISettings = defaultDISettings;
		}
	}
}
