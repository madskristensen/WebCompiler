using System.Collections.Generic;

namespace WebCompiler
{
    class LessOptions : BaseOptions
    {
		private const string trueStr = "true";

        public LessOptions(Config config)
        {
            StrictMath = GetValue(config, "strictMath").ToLowerInvariant() == trueStr;
            KeepFirstSpecialComment = GetValue(config, "keepFirstSpecialComment").ToLowerInvariant() == trueStr;
            DisableVariableRedefines = GetValue(config, "disableVariableRedefines").ToLowerInvariant() == trueStr;
            DisableColorCompression = GetValue(config, "disableColorCompression").ToLowerInvariant() == trueStr;
        }

        public bool StrictMath { get; set; }

        public bool KeepFirstSpecialComment { get; set; }

        public bool DisableVariableRedefines { get; set; }

        public bool DisableColorCompression { get; set; }
    }
}
