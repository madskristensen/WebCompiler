using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace WebCompiler
{
    /// <summary>
    /// Handles reading and writing config files to disk.
    /// </summary>
    public class ConfigHandler
    {
        /// <summary>
        /// Adds a config file if no one exist or adds the specified config to an existing config file.
        /// </summary>
        /// <param name="fileName">The file path of the configuration file.</param>
        /// <param name="config">The compiler config object to add to the configration file.</param>
        public void AddConfig(string fileName, Config config)
        {
            IEnumerable<Config> existing = GetConfigs(fileName);
            List<Config> configs = new List<Config>();
            configs.AddRange(existing);
            configs.Add(config);
            config.FileName = fileName;

            string content = JsonConvert.SerializeObject(configs, Formatting.Indented);
            File.WriteAllText(fileName, content, new UTF8Encoding(true));
        }

        /// <summary>
        /// Get all the config objects in the specified file.
        /// </summary>
        /// <param name="fileName">A relative or absolute file path to the configuration file.</param>
        /// <returns>A list of Config objects.</returns>
        public static IEnumerable<Config> GetConfigs(string fileName)
        {
            FileInfo file = new FileInfo(fileName);

            if (!file.Exists)
                return Enumerable.Empty<Config>();

            string content = File.ReadAllText(fileName);
            var configs = JsonConvert.DeserializeObject<IEnumerable<Config>>(content);
            string folder = Path.GetDirectoryName(file.FullName);

            foreach (Config config in configs)
            {
                config.FileName = fileName;
            }

            return configs;
        }
    }
}
