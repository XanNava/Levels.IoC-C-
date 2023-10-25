namespace Levels.Universal {
    public static partial class Attribute {
        [System.AttributeUsage(System.AttributeTargets.All)]
        public sealed class Inject : System.Attribute {
            public string Tag = "";
            public bool UseDefaultTags = true;

            public Inject(
                string tag,
                bool useDefaultTags = true) {

                Tag = tag;
                UseDefaultTags = useDefaultTags;
            }

            public Inject() {
            }

            public string GetTag() {
                return Tag;
            }
        }
    }
}