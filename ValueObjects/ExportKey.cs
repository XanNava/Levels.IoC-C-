using System;

namespace Levels.Universal {
    public struct ExportKey {
        public Type TypeOf;
        public string Tag;

        public static implicit operator ExportKey((Type, string) value) {
            return new ExportKey {
                TypeOf = value.Item1,
                Tag = value.Item2
            };
        }
    }
}