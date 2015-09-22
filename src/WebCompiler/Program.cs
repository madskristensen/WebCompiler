using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebCompiler
{
    class Program
    {
        static int Main(params string[] args)
        {
            string configPath = args[0];
            string file = args.Length > 1 ? args[1] : null;
            var configs = GetConfigs(configPath, file);

            if (configs == null)
            {
                Console.WriteLine("\x1B[33mNo configurations matched");
                return 0;
            }

            ConfigFileProcessor processor = new ConfigFileProcessor();
            EventHookups(processor, configPath);

            var results = processor.Process(configPath, configs);
            var errorResults = results.Where(r => r.HasErrors);

            foreach (var result in errorResults)
                foreach (var error in result.Errors)
                {
                    Console.Write("\x1B[31m" + error.Message);
                }

            return errorResults.Any() ? 1 : 0;
        }

        private static void EventHookups(ConfigFileProcessor processor, string configPath)
        {
            // For console colors, see http://stackoverflow.com/questions/23975735/what-is-this-u001b9-syntax-of-choosing-what-color-text-appears-on-console

            processor.BeforeProcess += (s, e) => { Console.WriteLine($"Processing \x1B[36m{e.Config.InputFile}"); if (e.ContainsChanges) FileHelpers.RemoveReadonlyFlagFromFile(e.Config.GetAbsoluteOutputFile()); };
            processor.AfterProcess += (s, e) => { Console.WriteLine($"  \x1B[32mCompiled"); };
            processor.BeforeWritingSourceMap += (s, e) => { if (e.ContainsChanges) FileHelpers.RemoveReadonlyFlagFromFile(e.ResultFile); };
            processor.AfterWritingSourceMap += (s, e) => { Console.WriteLine($"  \x1B[32mSourcemap"); };
            processor.ConfigProcessed += (s, e) => { Console.WriteLine("\t"); };

            FileMinifier.BeforeWritingMinFile += (s, e) => { FileHelpers.RemoveReadonlyFlagFromFile(e.ResultFile); };
            FileMinifier.AfterWritingMinFile += (s, e) => { Console.WriteLine($"  \x1B[32mMinified"); };
            FileMinifier.BeforeWritingGzipFile += (s, e) => { FileHelpers.RemoveReadonlyFlagFromFile(e.ResultFile); };
            FileMinifier.AfterWritingGzipFile += (s, e) => { Console.WriteLine($"  \x1B[32mGZipped"); };
        }

        private static IEnumerable<Config> GetConfigs(string configPath, string file)
        {
            var configs = ConfigHandler.GetConfigs(configPath);

            if (configs == null || !configs.Any())
                return null;

            if (file != null)
            {
                if (file.StartsWith("*"))
                    configs = configs.Where(c => Path.GetExtension(c.InputFile).Equals(file.Substring(1), StringComparison.OrdinalIgnoreCase));
                else
                    configs = configs.Where(c => c.InputFile.Equals(file, StringComparison.OrdinalIgnoreCase));
            }

            return configs;
        }
    }
}
