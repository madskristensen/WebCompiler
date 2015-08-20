using Newtonsoft.Json;

namespace WebCompiler
{
    /// <summary>
    /// Give all options for the Sass compiler
    /// </summary>
    public class SassOptions : BaseOptions<SassOptions>
    {
        private const string trueStr = "true";

        public SassOptions()
        { }

        /// <summary>
        /// Create an instance of the Class SassOptions
        /// </summary>
        /// <param name="config">The Scss configuration file.</param>
        protected override void LoadSettings(Config config)
        {
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
        }

        protected override string CompilerFileName
        {
            get { return "sass"; }
        }

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
    }
}
