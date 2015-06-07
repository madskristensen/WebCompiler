using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace WebCompiler
{
    public class ConfigHandler
    {
        public void AddConfig(string fileName, Config config)
        {
            IEnumerable<Config> existing = GetConfigs(fileName);
            List<Config> configs = new List<Config>();
            configs.AddRange(existing);
            configs.Add(config);
            config.FileName = fileName;
            
            string content = JsonConvert.SerializeObject(configs, Formatting.Indented);
            File.WriteAllText(fileName, content);
        }

        public static IEnumerable<Config> GetConfigs(string fileName)
        {
            FileInfo file = new FileInfo(fileName);

            if (!file.Exists)
                return Enumerable.Empty<Config>();

            string content = File.ReadAllText(fileName);
            var configs = JsonConvert.DeserializeObject<IEnumerable<Config>>(content);
            string folder = Path.GetDirectoryName(file.FullName);

            // Make the output path absolute
            foreach (Config config in configs)
            {
                //config.OutputFileAboslute = Path.Combine(folder, config.OutputFile.Replace("/", "\\"));
                config.FileName = fileName;
            }

            return configs;
        }
    }
}
