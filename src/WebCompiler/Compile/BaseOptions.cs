using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebCompiler
{
    /// <summary>
    /// Base class containing methods to all extensions options
    /// </summary>
    public abstract class BaseOptions<T> where T : BaseOptions<T>, new()
    {
        /// <summary>
        /// Loads the options based on the config object
        /// </summary>
        public static T FromConfig(Config config)
        {
            string defaultFile = config.FileName + ".defaults";

            T options = new T();

            if (File.Exists(defaultFile))
            {
                JObject json = JObject.Parse(File.ReadAllText(defaultFile));
                var jsonOptions = json["compilers"][options.CompilerFileName];

                if (jsonOptions != null)
                    options = JsonConvert.DeserializeObject<T>(jsonOptions.ToString());
            }

            options.LoadSettings(config);

            return options;
        }

        /// <summary>
        /// The file name should match the compiler name
        /// </summary>
        protected abstract string CompilerFileName { get; }

        /// <summary>
        /// Load the settings from the config object
        /// </summary>
        protected virtual void LoadSettings(Config config)
        {
            var sourceMap = GetValue(config, "sourceMap");
            if (sourceMap != null)
                SourceMap = sourceMap.ToLowerInvariant() == "true";
        }

        /// <summary>
        /// Generate Source Map v3
        /// </summary>
        [JsonProperty("sourceMap")]
        public bool SourceMap { get; set; }

        /// <summary>
        /// Gets a value from a string keyed dictionary
        /// </summary>
        protected string GetValue(Config config, string key)
        {
            if (config.Options.ContainsKey(key))
                return config.Options[key].ToString();

            return null;
        }
    }
}
