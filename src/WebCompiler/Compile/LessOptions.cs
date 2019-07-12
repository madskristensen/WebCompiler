using Newtonsoft.Json;

namespace WebCompiler
{
    /// <summary>
    /// Give all options for the LESS compiler
    /// </summary>
    public class LessOptions : BaseOptions<LessOptions>
    {
        private const string trueStr = "true";

        /// <summary> Creates a new instance of the class.</summary>
        public LessOptions()
        { }

        /// <summary>
        /// Load the settings from the config object
        /// </summary>
        protected override void LoadSettings(Config config)
        {
            base.LoadSettings(config);

            var autoPrefix = GetValue(config, "autoPrefix");
            if (autoPrefix != null)
                AutoPrefix = autoPrefix;

            var cssComb = GetValue(config, "cssComb");
            if (cssComb != null)
                CssComb = cssComb;

            var ieCompat = GetValue(config, "ieCompat");
            if (ieCompat != null)
                IECompat = ieCompat.ToLowerInvariant() == trueStr;

            var math = GetValue(config, "math");
            if (math != null)
                Math = math;

            var strictMath = GetValue(config, "strictMath");
            if (strictMath != null)
                StrictMath = strictMath.ToLowerInvariant() == trueStr;

            var strictUnits = GetValue(config, "strictUnits");
            if (strictUnits != null)
                StrictUnits = strictUnits.ToLowerInvariant() == trueStr;

            var rootPath = GetValue(config, "rootPath");
            if (rootPath != null)
                RootPath = rootPath;

            var relativeUrls = GetValue(config, "relativeUrls");
            if (relativeUrls != null)
                RelativeUrls = relativeUrls.ToLowerInvariant() == trueStr;

            var sourceMapRoot = GetValue(config, "sourceMapRoot");
            if (sourceMapRoot != null)
                SourceMapRoot = sourceMapRoot;

            var sourceMapBasePath = GetValue(config, "sourceMapBasePath");
            if (sourceMapBasePath != null)
                SourceMapBasePath = sourceMapBasePath;
        }

        /// <summary>
        /// The file name should match the compiler name
        /// </summary>
        protected override string CompilerFileName
        {
            get { return "less"; }
        }

        /// <summary>
        /// Autoprefixer will use the data based on current browser popularity and
        /// property support to apply prefixes for you.
        /// </summary>
        [JsonProperty("autoPrefix")]
        public string AutoPrefix { get; set; } = "";

        /// <summary>
        /// CssComb will order the properties in the compiled CSS file.
        /// </summary>
        [JsonProperty("cssComb")]
        public string CssComb { get; set; } = "none";

        /// <summary>
        /// Currently only used for the data-uri function to ensure that images aren't
        /// created that are too large for the browser to handle.
        /// </summary>
        [JsonProperty("ieCompat")]
        public bool IECompat { get; set; } = true;

        /// <summary>
        /// New option for math that replaces 'strictMath' option.
        /// </summary>
        [JsonProperty("math")]
        public string Math { get; set; } = null;

        /// <summary>
        /// Without this option on Less will try and process all maths in your CSS.
        /// </summary>
        [JsonProperty("strictMath")]
        public bool StrictMath { get; set; } = false;

        /// <summary>
        /// Without this option, less attempts to guess at the output unit when it does maths.
        /// </summary>
        [JsonProperty("strictUnits")]
        public bool StrictUnits { get; set; } = false;

        /// <summary>
        /// This option allows you to re-write URL's in imported files so that the URL is always
        /// relative to the base imported file.
        /// </summary>
        [JsonProperty("relativeUrls")]
        public bool RelativeUrls { get; set; } = true;

        /// <summary>
        /// Allows you to add a path to every generated import and url in your css.
        /// This does not affect less import statements that are processed,
        /// just ones that are left in the output css.
        /// </summary>
        [JsonProperty("rootPath")]
        public string RootPath { get; set; } = "";

        /// <summary>
        /// Base path, will be emitted in source-map as is
        /// </summary>
        [JsonProperty("sourceMapRoot")]
        public string SourceMapRoot { get; set; } = string.Empty;

        /// <summary>
        /// This is the opposite of the 'rootpath' option, it specifies a path which should be removed from the output paths.
        /// </summary>
        [JsonProperty("sourceMapBasePath")]
        public string SourceMapBasePath { get; set; } = string.Empty;
    }
}
