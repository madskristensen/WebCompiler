using System.Collections.Generic;

namespace WebCompiler
{
    class LessOptions : BaseOptions
    {
        public LessOptions(Config config)
        {
            StrictMath = GetValue(config, "strictMath") == "true";
            KeepFirstSpecialComment = GetValue(config, "keepFirstSpecialComment") == "true";
            DisableVariableRedefines = GetValue(config, "disableVariableRedefines") == "true";
            DisableColorCompression = GetValue(config, "disableColorCompression") == "true";
        }

        public bool StrictMath { get; set; }

        public bool KeepFirstSpecialComment { get; set; }

        public bool DisableVariableRedefines { get; set; }

        public bool DisableColorCompression { get; set; }
    }
}
