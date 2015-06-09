using System.Collections.Generic;

namespace WebCompiler
{
    class LessOptions
    {
        public LessOptions(Config config)
        {
            StrictMath = GetValue(config, "strictMath") == "true";
            KeepFirstSpecialComment = GetValue(config, "keepFirstSpecialComment") == "true";
            DisableVariableRedefines = GetValue(config, "disableVariableRedefines") == "true";
            DisableColorCompression = GetValue(config, "disableColorCompression") == "true";
        }

        internal static string GetValue(Config config, string key)
        {
            if (config.Options.ContainsKey(key))
                return config.Options[key];

            return string.Empty;
        }

        public bool StrictMath { get; set; }

        public bool KeepFirstSpecialComment { get; set; }

        public bool DisableVariableRedefines { get; set; }

        public bool DisableColorCompression { get; set; }
    }
}
