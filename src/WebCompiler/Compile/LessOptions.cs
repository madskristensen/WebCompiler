using System.Collections.Generic;

namespace WebCompiler
{
    class LessOptions : BaseOptions
    {
        private const string trueStr = "true";

        public LessOptions(Config config)
        {
            StrictMath = GetValue(config, "strictMath").ToLowerInvariant() == trueStr;
            StrictUnits = GetValue(config, "strictUnits").ToLowerInvariant() == trueStr;
            RootPath = GetValue(config, "rootPath");

            string relativeUrls = GetValue(config, "relativeUrls").ToLowerInvariant();
            if (relativeUrls == "false")
                RelativeUrls = false;
        }

        public bool StrictMath { get; set; }

        public bool StrictUnits { get; set; }

        public bool RelativeUrls { get; set; } = true;

        public string RootPath { get; set; }
    }
}
