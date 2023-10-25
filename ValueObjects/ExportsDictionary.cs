namespace Levels.Universal{
    using System;
    using System.Collections.Generic;

    public class ExportsDictionary {
        public readonly Dictionary<ExportKey, Func<IoC.IScope, string, object>> Dictionary = new Dictionary<ExportKey, Func<IoC.IScope, string, object>>();
    }
}