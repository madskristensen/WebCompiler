using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebCompiler
{
    public abstract class BaseMinifyOptions<T> where T : BaseMinifyOptions<T>, new()
    {
        /// <summary>
        /// Loads the options based on the config object
        /// </summary>
        protected static void LoadDefaultSettings(Config config, string minifierType)
        {
            string defaultFile = config.FileName + ".defaults";

            Dictionary<string, object> options = new Dictionary<string, object>();

            if (File.Exists(defaultFile))
            {
                JObject json = JObject.Parse(File.ReadAllText(defaultFile));
                var jsonOptions = json["minifiers"][minifierType];

                if (jsonOptions != null)
                    options = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonOptions.ToString());
            }

            foreach (string key in options.Keys)
            {
                if (!config.Minify.ContainsKey(key))
                    config.Minify[key] = options[key];
            }
        }
    }
}
