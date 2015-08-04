using System.Collections.Generic;

namespace WebCompiler
{
    class LessOptions : BaseOptions
    {
		private const string trueStr = "true";

        public LessOptions(Config config)
        {
            StrictMath = GetValue(config, "strictMath").ToLowerInvariant() == trueStr;
            RelativeUrls = GetValue(config, "relativeUrls").ToLowerInvariant() == trueStr;
        }

        public bool StrictMath { get; set; }

        public bool RelativeUrls { get; set; }
    }
}
