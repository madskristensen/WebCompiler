using Newtonsoft.Json;

namespace WebCompiler
{
    /// <summary>
    /// Give all options for the Sass compiler
    /// </summary>
    public class SassOptions : BaseOptions<SassOptions>
    {
        private const string trueStr = "true";

        /// <summary> Creates a new instance of the class.</summary>
        public SassOptions()
        { }

        /// <summary>
        /// Loads the settings based on the config
        /// </summary>
        protected override void LoadSettings(Config config)
        {
            base.LoadSettings(config);

            var autoPrefix = GetValue(config, "autoPrefix");
            if (autoPrefix != null)
                AutoPrefix = autoPrefix;

            if (config.Options.ContainsKey("outputStyle"))
                OutputStyle = config.Options["outputStyle"].ToString();

            if (config.Options.ContainsKey("indentType"))
                IndentType = config.Options["indentType"].ToString();

            int precision = 5;
            if (int.TryParse(GetValue(config, "precision"), out precision))
                Precision = precision;

            int indentWidth = -1;
            if (int.TryParse(GetValue(config, "indentWidth"), out indentWidth))
                IndentWidth = indentWidth;

            var relativeUrls = GetValue(config, "relativeUrls");
            if (relativeUrls != null)
                RelativeUrls = relativeUrls.ToLowerInvariant() == trueStr;

            var includePath = GetValue(config, "includePath");
            if (includePath != null)
                IncludePath = includePath;

            var sourceMapRoot = GetValue(config, "sourceMapRoot");
            if (sourceMapRoot != null)
                SourceMapRoot = sourceMapRoot;

            var lineFeed = GetValue(config, "lineFeed");
            if (lineFeed != null)
                LineFeed = lineFeed;
        }

        /// <summary>
        /// The file name should match the compiler name
        /// </summary>
        protected override string CompilerFileName
        {
            get { return "sass"; }
        }

        /// <summary>
        /// Autoprefixer will use the data based on current browser popularity and
        /// property support to apply prefixes for you.
        /// </summary>
        [JsonProperty("autoPrefix")]
        public string AutoPrefix { get; set; } = "";

        /// <summary>
        /// Path to look for imported files
        /// </summary>
        [JsonProperty("includePath")]
        public string IncludePath { get; set; } = string.Empty;

        /// <summary>
        /// Indent type for output CSS.
        /// </summary>
        [JsonProperty("indentType")]
        public string IndentType { get; set; } = "space";

        /// <summary>
        /// Number of spaces or tabs (maximum value: 10)
        /// </summary>
        [JsonProperty("indentWidth")]
        public int IndentWidth { get; set; } = 2;

        /// <summary>
        /// Type of output style
        /// </summary>
        [JsonProperty("outputStyle")]
        public string OutputStyle { get; set; } = "nested";


        /// <summary>
        /// Precision
        /// </summary>
        public int Precision { get; set; } = 5;

        /// <summary>
        /// This option allows you to re-write URL's in imported files so that the URL is always
        /// relative to the base imported file.
        /// </summary>
        [JsonProperty("relativeUrls")]
        public bool RelativeUrls { get; set; } = true;

        /// <summary>
        /// Base path, will be emitted in source-map as is
        /// </summary>
        [JsonProperty("sourceMapRoot")]
        public string SourceMapRoot { get; set; } = string.Empty;

        /// <summary>
        /// Linefeed style (cr | crlf | lf | lfcr)
        /// </summary>
        [JsonProperty("lineFeed")]
        public string LineFeed { get; set; } = string.Empty;


    }
}
