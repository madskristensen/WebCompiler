using Newtonsoft.Json;
using System.Linq;

namespace WebCompiler
{
    /// <summary>
    /// Give all options for the Handlebars compiler
    /// </summary>
    public class HandlebarsOptions : BaseOptions<HandlebarsOptions>
    {
        private const string trueStr = "true";

        /// <summary> Creates a new instance of the class.</summary>
        public HandlebarsOptions()
        { }

        /// <summary>
        /// Load the settings from the config object
        /// </summary>
        protected override void LoadSettings(Config config)
        {
            base.LoadSettings(config);

            var name = GetValue(config, "name");
            if (name != null)
                Name = name;

            var @namespace = GetValue(config, "namespace");
            if (@namespace != null)
                TemplateNameSpace = @namespace;

            var root = GetValue(config, "root");
            if (root != null)
                Root = root;

            var commonjs = GetValue(config, "commonjs");
            if (commonjs != null)
                CommonJS = commonjs;

            var amd = GetValue(config, "amd");
            if (amd != null)
                AMD = amd.ToLowerInvariant() == trueStr;

            var forcePartial = GetValue(config, "forcePartial");
            if (forcePartial != null)
                ForcePartial = forcePartial.ToLowerInvariant() == trueStr;

            var noBOM = GetValue(config, "noBOM");
            if (noBOM != null)
                NoBOM = noBOM.ToLowerInvariant() == trueStr;

            var knownHelpersOnly = GetValue(config, "knownHelpersOnly");
            if (knownHelpersOnly != null)
                KnownHelpersOnly = knownHelpersOnly.ToLowerInvariant() == trueStr;

            var knownHelpers = GetValue(config, "knownHelpers");
            if (knownHelpers != null)            
                KnownHelpers = knownHelpers.Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
        }

        /// <summary>
        /// The file name should match the compiler name
        /// </summary>
        protected override string CompilerFileName
        {
            get { return "hbs"; }
        }

        /// <summary>
        /// Template root. Base value that will be stripped from template names.
        /// </summary>
        [JsonProperty("root")]
        public string Root { get; set; } = "";

        /// <summary>
        /// Removes the BOM (Byte Order Mark) from the beginning of the templates.
        /// </summary>
        [JsonProperty("noBOM")]
        public bool NoBOM { get; set; } = false;

        /// <summary>
        /// Name of passed string templates.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = "";

        /// <summary>
        /// Template namespace 
        /// </summary>
        [JsonProperty("namespace")]
        public string TemplateNameSpace { get; set; } = "";

        /// <summary>
        /// Compile with known helpers only
        /// </summary>
        [JsonProperty("knownHelpersOnly")]
        public bool KnownHelpersOnly { get; set; } = false;


        /// <summary>
        /// Forcing a partial template compilation
        /// </summary>
        [JsonProperty("forcePartial")]
        public bool ForcePartial { get; set; } = false;

        /// <summary>
        /// List of known helpers for a more optimized output
        /// </summary>
        [JsonProperty("knownHelpers")]
        public string[] KnownHelpers { get; set; } = new string[0];

        /// <summary>
        /// Path to the Handlebars module to export CommonJS style
        /// </summary>
        [JsonProperty("commonjs")]
        public string CommonJS { get; set; } = "";

        /// <summary>
        /// Exports amd style (require.js), this option has priority to commonjs.
        /// </summary>
        [JsonProperty("amd")]
        public bool AMD { get; set; } = false;
    }
}
