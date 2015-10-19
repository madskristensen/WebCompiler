using Newtonsoft.Json;

namespace WebCompiler
{
    /// <summary>
    /// Give all options for the CoffeeScript compiler
    /// </summary>
    public class IcedCoffeeScriptOptions : BaseOptions<IcedCoffeeScriptOptions>
    {
        /// <summary> Creates a new instance of the class.</summary>
        public IcedCoffeeScriptOptions()
        { }

        private const string trueStr = "true";

        /// <summary>
        /// Loads the settings based on the config
        /// </summary>
        protected override void LoadSettings(Config config)
        {
            base.LoadSettings(config);

            var bare = GetValue(config, "bare");
            if (bare != null)
                Bare = bare.ToLowerInvariant() == trueStr;

            var runtimeMode = GetValue(config, "runtimeMode");
            if (runtimeMode != null)
                RuntimeMode = runtimeMode.ToLowerInvariant();
        }

        /// <summary>
        /// The file name should match the compiler name
        /// </summary>
        protected override string CompilerFileName
        {
            get { return "coffeescript"; }
        }

        /// <summary>
        /// Compile the JavaScript without the top-level function safety wrapper.
        /// </summary>
        [JsonProperty("bare")]
        public bool Bare { get; set; } = false;

        /// <summary>
        /// Specify how the Iced runtime is included in the output JavaScript file.
        /// </summary>
        [JsonProperty("runtimeMode")]
        public string RuntimeMode { get; set; } = "node";
    }
}
