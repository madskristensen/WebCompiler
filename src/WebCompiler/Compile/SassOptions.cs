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

        /// <summary>
        /// The file name should match the compiler name
        /// </summary>
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
