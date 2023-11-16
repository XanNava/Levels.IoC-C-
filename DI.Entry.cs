using System;
using System.Security.Cryptography;
using System.Text;

// TODO: make a string ID like PropertyName, will also need to make an IEquatable for PropertyName.


namespace Levels.Universal {
	using ScopeKey = String.ID<DI.Scope>;
	public partial class DI {
		public struct EntryKey : IEquatable<EntryKey> {
			public String.ID<EntryKey> ID;
			public object Source;

			public EntryKey(string key, ref object source) {
				this.ID = String.ID.Manager<EntryKey>.Reference.GetID(key);
				this.Source = source;
			}

			public static Double HashString(string input) {
				using (MD5 md5Hash = MD5.Create()) {
					byte[] data = Encoding.UTF8.GetBytes(input);
					byte[] hash = md5Hash.ComputeHash(data);
					long longHash = BitConverter.ToInt64(hash, 0);

					return Convert.ToDouble(longHash) / Convert.ToDouble(long.MaxValue);
				}
			}

			public bool Equals(EntryKey other) {
				return ID == other.ID;
			}

			public string Readout() {
				return $"READOUT : [RegisterKey][TagID={ID}][Source={Source}]";
			}
		}
		public interface IEntry<T> : IEquatable<IEntry<T>> {
			EntryKey Key {
				get;
			}

			/// <summary>
			/// Func<source, scope, key, return_service>
			/// </summary>
			/// <todo>Not sure we need source. </todo>
			Func<object, ScopeKey, string, T> Fufiller {
				get;
			}
		}

		public struct Entry<T> : IEntry<T> {
			private readonly EntryKey _key;
			public EntryKey Key {
				get => _key;
			}

			private readonly Func<object, ScopeKey, string, T> _fufiller;
			/// <summary>
			/// Func<source, scope, key, return_service>
			/// </summary>
			/// <todo>Not sure we need source. </todo>
			public Func<object, ScopeKey, string, T> Fufiller {
				get => _fufiller;
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="key"></param>
			/// <param name="fufiller">source, scope, key, return_service</param>
			public Entry(EntryKey key, Func<object, ScopeKey, string, T> fufiller) : this() {
				_key = key;
				_fufiller = fufiller;
			}

			public bool Equals(IEntry<T> other) {
				return Key.Equals(other.Key);
			}
		}

		public struct ServiceEntry<T> : IEntry<T>, IEquatable<ServiceEntry<T>> {
			private readonly EntryKey _key;
			public EntryKey Key {
				get => _key;
			}

			private readonly Func<object, ScopeKey, string, T> _fufiller;
			public Func<object, ScopeKey, string, T> Fufiller {
				get => _fufiller;
			}

			public ServiceEntry(EntryKey key, T instance) : this() {
				_key = key;
				_fufiller = new Func<object, ScopeKey, string, T>((sr, sc, t) => { return instance; });
			}

			public ServiceEntry(string tag, object source, T instance) : this(new EntryKey(tag, ref source), instance) {
			}

			public bool Equals(IEntry<T> other) {
				return Key.Equals(other.Key);
			}

			public bool Equals(ServiceEntry<T> other) {
				return Key.Equals(other.Key);
			}
		}

		public partial class Entry {
			public static class Factory {
				public static IEntry<R> Service<R>(R reference, string key, object source) {

					return new ServiceEntry<R>(new EntryKey("", ref source), reference);
				}

				// For structs/DataTypes you want to not copy.
				public static Entry<R> Service<R>(Func<R> reference, string key, object source) {

					return new Entry<R>(new EntryKey(
						"", ref source), (sr, sc, t) => reference.Invoke());
				}

				// For generic factories?
				// TODO: Still gotta test to make sure works.
				public static Entry<R> Service<G, R>(IGenericMethod<R> reference, string key, object source) {

					return new Entry<R>(new EntryKey(
						"", ref source), (sr, sc, t) => reference.call<G>());
				}
			}
		}
	}

	public static class IEntry_Extends {
		// TODO: Why?
		public static string Readout<T>(this DI.IEntry<T> source) {
			return $"READOUT : [IRegistery]\n\n[key='{source.Key.Readout()}'][type='{typeof(T).Name}'][fulfiller='{source.Fufiller}']";
		}
	}
}

// TODO: Move
public interface IGenericMethod<R> {
	public R call<G>();
}