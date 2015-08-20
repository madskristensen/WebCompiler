using System;
using Newtonsoft.Json;

namespace WebCompiler
{
    public class LessOptions : BaseOptions<LessOptions>
    {
        private const string trueStr = "true";

        public LessOptions()
        { }

        protected override void LoadSettings(Config config)
        {
            var strictMath = GetValue(config, "strictMath");
            if (strictMath != null)
                StrictMath = strictMath.ToLowerInvariant() == trueStr;

            var strictUnits = GetValue(config, "strictUnits");
            if (strictUnits != null)
                StrictUnits = strictUnits.ToLowerInvariant() == trueStr;

            var rootPath = GetValue(config, "rootPath");
            if (RootPath != null)
                RootPath = RootPath;

            var relativeUrls = GetValue(config, "relativeUrls");
            if (relativeUrls != null)
                RelativeUrls = relativeUrls.ToLowerInvariant() == trueStr;
        }

        protected override string CompilerFileName
        {
            get { return "less"; }
        }

        [JsonProperty("strictMath")]
        public bool StrictMath { get; set; } = false;

        [JsonProperty("strictUnits")]
        public bool StrictUnits { get; set; } = false;

        [JsonProperty("relativeUrls")]
        public bool RelativeUrls { get; set; } = true;

        [JsonProperty("rootPath")]
        public string RootPath { get; set; } = "";
    }
}
