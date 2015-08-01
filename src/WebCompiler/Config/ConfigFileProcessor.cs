using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WebCompiler
{
    /// <summary>
    /// The main class for compiling based on configuration files.
    /// </summary>
    public class ConfigFileProcessor
    {
        /// <summary>
        /// Parses a compiler config file and runs the configured compilers.
        /// </summary>
        /// <param name="configFile">The absolute or relative file path to compilerconfig.json</param>
        /// <returns>A list of compiler results.</returns>
        public IEnumerable<CompilerResult> Process(string configFile)
        {
            FileInfo info = new FileInfo(configFile);
            var configs = ConfigHandler.GetConfigs(configFile);
            List<CompilerResult> list = new List<CompilerResult>();

            if (configs.Any())
                OnConfigProcessed(configs.First(), 0, configs.Count());

            foreach (Config config in configs)
            {
                var result = ProcessConfig(info.Directory.FullName, config);
                list.Add(result);
                OnConfigProcessed(config, list.Count, configs.Count());
            }

            return list;
        }

        /// <summary>
        /// Compiles all configs with the same input file extension as the specified sourceFile
        /// </summary>
        public IEnumerable<CompilerResult> SourceFileChanged(string configFile, string sourceFile)
        {
            string folder = Path.GetDirectoryName(configFile);
            List<CompilerResult> list = new List<CompilerResult>();
            var configs = ConfigHandler.GetConfigs(configFile);

            // Compile if the file if it's referenced directly in compilerconfig.json
            foreach (Config config in configs)
            {
                string input = Path.Combine(folder, config.InputFile.Replace("/", "\\"));

                if (input.Equals(sourceFile, StringComparison.OrdinalIgnoreCase))
                    list.Add(ProcessConfig(folder, config));
            }

            // If not referenced directly, compile all configs with same file extension
            if (list.Count == 0)
            {
                string sourceExtension = Path.GetExtension(sourceFile);

                foreach (Config config in configs)
                {
                    string inputExtension = Path.GetExtension(config.InputFile);

                    if (inputExtension.Equals(sourceExtension, StringComparison.OrdinalIgnoreCase))
                        list.Add(ProcessConfig(folder, config));
                }
            }

            return list;
        }

        /// <summary>
        /// Returns a collection of Config objects that all contain the specified sourceFile
        /// </summary>
        public static IEnumerable<Config> IsFileConfigured(string configFile, string sourceFile)
        {
            try
            {
                var configs = ConfigHandler.GetConfigs(configFile);
                string folder = Path.GetDirectoryName(configFile);
                List<Config> list = new List<Config>();

                foreach (Config config in configs)
                {
                    string input = Path.Combine(folder, config.InputFile.Replace("/", "\\"));

                    if (input.Equals(sourceFile, StringComparison.OrdinalIgnoreCase))
                        list.Add(config);
                }

                return list;
            }
            catch
            {
                return null;
            }
        }

        private CompilerResult ProcessConfig(string baseFolder, Config config)
        {
            ICompiler compiler = CompilerService.GetCompiler(config);

            var result = compiler.Compile(config);

            if (result.HasErrors)
                return result;

            config.Output = result.CompiledContent;
            string outputFile = Path.Combine(baseFolder, config.OutputFile);

            OnBeforeProcess(config, baseFolder);
            File.WriteAllText(outputFile, config.Output, new UTF8Encoding(true));
            OnAfterProcess(config, baseFolder);

            if (config.Minify.ContainsKey("enabled") && config.Minify["enabled"].ToString().Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                FileMinifier.MinifyFile(config);
            }

            if (config.SourceMap)
            {
                if (!string.IsNullOrEmpty(result.SourceMap))
                {
                    string aboslute = config.GetAbsoluteOutputFile();
                    string mapFile = aboslute + ".map";

                    OnBeforeWritingSourceMap(aboslute, mapFile);
                    File.WriteAllText(mapFile, result.SourceMap, new UTF8Encoding(true));
                    OnAfterWritingSourceMap(aboslute, mapFile);
                }
            }

            return result;
        }

        private void OnBeforeProcess(Config config, string baseFolder)
        {
            if (BeforeProcess != null)
            {
                BeforeProcess(this, new CompileFileEventArgs(config, baseFolder));
            }
        }

        private void OnConfigProcessed(Config config, int amountProcessed, int total)
        {
            if (ConfigProcessed != null)
            {
                ConfigProcessed(this, new ConfigProcessedEventArgs(config, amountProcessed, total));
            }
        }

        private void OnAfterProcess(Config config, string baseFolder)
        {
            if (AfterProcess != null)
            {
                AfterProcess(this, new CompileFileEventArgs(config, baseFolder));
            }
        }

        private void OnBeforeWritingSourceMap(string file, string mapFile)
        {
            if (BeforeWritingSourceMap != null)
            {
                BeforeWritingSourceMap(this, new SourceMapEventArgs(file, mapFile));
            }
        }

        private void OnAfterWritingSourceMap(string file, string mapFile)
        {
            if (AfterWritingSourceMap != null)
            {
                AfterWritingSourceMap(this, new SourceMapEventArgs(file, mapFile));
            }
        }

        /// <summary>
        /// Fires before the compiler writes the output to disk.
        /// </summary>
        public event EventHandler<CompileFileEventArgs> BeforeProcess;

        /// <summary>
        /// Fires when a config file has been processed.
        /// </summary>
        public event EventHandler<ConfigProcessedEventArgs> ConfigProcessed;

        /// <summary>
        /// Fires after the compiler writes the output to disk.
        /// </summary>
        public event EventHandler<CompileFileEventArgs> AfterProcess;

        /// <summary>
        /// Fires before the compiler writes a source map file to disk.
        /// </summary>
        public event EventHandler<SourceMapEventArgs> BeforeWritingSourceMap;

        /// <summary>
        /// Fires after the compiler writes a source map file to disk.
        /// </summary>
        public event EventHandler<SourceMapEventArgs> AfterWritingSourceMap;
    }
}
