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
            RelativeUrls = GetValue(config, "relativeUrls").ToLowerInvariant() == trueStr;
            RootPath = GetValue(config, "rootPath");
        }

        public bool StrictMath { get; set; }

        public bool StrictUnits { get; set; }

        public bool RelativeUrls { get; set; }

        public string RootPath { get; set; }
    }
}
