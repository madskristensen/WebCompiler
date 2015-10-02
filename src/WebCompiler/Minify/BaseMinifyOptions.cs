using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebCompiler
{
    /// <summary>
    /// Base class for minification options
    /// </summary>
    public abstract class BaseMinifyOptions
    {
        /// <summary>
        /// Loads the options based on the config object
        /// </summary>
        protected static void LoadDefaultSettings(Config config, string minifierType)
        {
            string defaultFile = config.FileName + ".defaults";

            if (!File.Exists(defaultFile))
                return;

            Dictionary<string, object> options = new Dictionary<string, object>();

            JObject json = JObject.Parse(File.ReadAllText(defaultFile));
            var jsonOptions = json["minifiers"]?[minifierType];

            if (jsonOptions != null)
                options = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonOptions.ToString());

            if (options != null)
            {
                foreach (string key in options.Keys)
                {
                    if (!config.Minify.ContainsKey(key))
                        config.Minify[key] = options[key];
                }
            }
        }

        /// <summary>
        /// Gets the string value of the minification settings.
        /// </summary>
        protected static string GetValue(Config config, string key, object defaultValue = null)
        {
            if (config.Minify.ContainsKey(key))
                return config.Minify[key].ToString();

            if (defaultValue != null)
                return defaultValue.ToString();

            return string.Empty;
        }
    }
}
