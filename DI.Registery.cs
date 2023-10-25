using System;
using System.Security.Cryptography;
using System.Text;

using UnityEngine;

namespace Levels.Universal.Experimental {
    public struct EntryKey : IEquatable<EntryKey> {
        public PropertyName key;
        public object Source;

        public EntryKey(string key, ref object source) {
            this.key = new PropertyName(key);
            Source = source;
        }

        public EntryKey(PropertyName key, object source) {
            this.key = new PropertyName(key);
            Source = source;
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
            return key == other.key;
        }

        public string Readout() {
            return $"READOUT : [RegisterKey][TagID={key}][Source={Source}]";
        }
    }
    public interface IEntry<T> {
        EntryKey Key { get; }
        Func<IScope, string, T> Fufiller { get; }
    }

    public struct Entry<T> : IEntry<T> {
        private readonly EntryKey _key;
        public EntryKey Key { get => _key; }

        /// <summary>
        /// scope, key, return
        /// </summary>
        private readonly Func<IScope, string, T> _fufiller;
        public Func<IScope, string, T> Fufiller { get => _fufiller; }

        public Entry(EntryKey key, Func<IScope, string, T> fufiller) : this() {
            _key = key;
            _fufiller = fufiller;
        }
    }

    public struct ServiceEntry<T> : IEntry<T> {
        private readonly EntryKey _key;
        public EntryKey Key { get => _key; }

        private readonly Func<IScope, string, T> _fufiller;
        public Func<IScope, string, T> Fufiller { get => _fufiller; }

        public ServiceEntry(EntryKey key, T instance) : this() {
            _key = key;
            _fufiller = new Func<IScope, string, T>((s, t) => { return instance; });
        }

        public ServiceEntry(string tag, object source, T instance) : this(new EntryKey(tag, source), instance) {
        }
    }

    public static class IRegistery_Extends {
        public static string Readout<T>(this IEntry<T> source) {
            return $"READOUT : [IRegistery]\n\n[key='{source.Key.Readout()}'][type='{typeof(T).Name}'][fulfiller='{source.Fufiller}']";
        }
    }

    public partial class Entry {
        public static class Factory {
            public static IEntry<T> Service<T>(T reference, string key, object source) {

                return new ServiceEntry<T>(new EntryKey("", source), reference);
            }

            // For structs/DataTypes you want to not copy.
            public static IEntry<T> Service<T>(Func<T> reference, string key, object source) {

                return new Entry<T>(new EntryKey(
                    "", source), (s, t) => reference.Invoke());
            }
        }
    }
}
