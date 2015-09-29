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
        private static List<string> _processing = new List<string>();
        private static object _syncRoot = new object(); // Used for source file changes so they don't try to write to the same file at the same time.

        /// <summary>
        /// Parses a compiler config file and runs the configured compilers.
        /// </summary>
        /// <param name="configFile">The absolute or relative file path to compilerconfig.json</param>
        /// <param name="configs">Optional configuration items in the config file</param>
        /// <returns>A list of compiler results.</returns>
        public IEnumerable<CompilerResult> Process(string configFile, IEnumerable<Config> configs = null)
        {
            if (_processing.Contains(configFile))
                return Enumerable.Empty<CompilerResult>();

            _processing.Add(configFile);
            List<CompilerResult> list = new List<CompilerResult>();

            try
            {
                FileInfo info = new FileInfo(configFile);
                configs = configs ?? ConfigHandler.GetConfigs(configFile);

                if (configs.Any())
                    OnConfigProcessed(configs.First(), 0, configs.Count());

                foreach (Config config in configs)
                {
                    var result = ProcessConfig(info.Directory.FullName, config);
                    list.Add(result);
                    OnConfigProcessed(config, list.Count, configs.Count());
                }
            }
            finally
            {
                if (_processing.Contains(configFile))
                    _processing.Remove(configFile);

                Telemetry.Flush();
            }

            return list;
        }

        /// <summary>
        /// Compiles all configs with the same input file extension as the specified sourceFile
        /// </summary>
        public IEnumerable<CompilerResult> SourceFileChanged(string configFile, string sourceFile)
        {
            lock (_syncRoot)
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

                Telemetry.Flush();

                return list;
            }
        }

        /// <summary>
        /// Returns a collection of config objects that all contain the specified sourceFile
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

            if (Path.GetExtension(config.OutputFile).Equals(".css", StringComparison.OrdinalIgnoreCase) && AdjustRelativePaths(config))
            {
                result.CompiledContent = CssRelativePath.Adjust(result.CompiledContent, config);
            }

            config.Output = result.CompiledContent;

            string outputFile = config.GetAbsoluteOutputFile();
            bool containsChanges = FileHelpers.HasFileContentChanged(outputFile, config.Output);

            OnBeforeProcess(config, baseFolder, containsChanges);

            if (containsChanges)
            {
                File.WriteAllText(outputFile, config.Output, new UTF8Encoding(true));
            }

            OnAfterProcess(config, baseFolder, containsChanges);

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
                    bool smChanges = FileHelpers.HasFileContentChanged(mapFile, result.SourceMap);

                    OnBeforeWritingSourceMap(aboslute, mapFile, smChanges);

                    if (smChanges)
                    {
                        File.WriteAllText(mapFile, result.SourceMap, new UTF8Encoding(true));
                    }

                    OnAfterWritingSourceMap(aboslute, mapFile, smChanges);
                }
            }

            Telemetry.TrackCompile(config);

            return result;
        }

        private static bool AdjustRelativePaths(Config config)
        {
            if (!config.Options.ContainsKey("relativeUrls"))
                return true;

            return config.Options["relativeUrls"].ToString() == "True";
        }

        private void OnBeforeProcess(Config config, string baseFolder, bool containsChanges)
        {
            if (BeforeProcess != null)
            {
                BeforeProcess(this, new CompileFileEventArgs(config, baseFolder, containsChanges));
            }
        }

        private void OnConfigProcessed(Config config, int amountProcessed, int total)
        {
            if (ConfigProcessed != null)
            {
                ConfigProcessed(this, new ConfigProcessedEventArgs(config, amountProcessed, total));
            }
        }

        private void OnAfterProcess(Config config, string baseFolder, bool containsChanges)
        {
            if (AfterProcess != null)
            {
                AfterProcess(this, new CompileFileEventArgs(config, baseFolder, containsChanges));
            }
        }

        private void OnBeforeWritingSourceMap(string file, string mapFile, bool containsChanges)
        {
            if (BeforeWritingSourceMap != null)
            {
                BeforeWritingSourceMap(this, new SourceMapEventArgs(file, mapFile, containsChanges));
            }
        }

        private void OnAfterWritingSourceMap(string file, string mapFile, bool containsChanges)
        {
            if (AfterWritingSourceMap != null)
            {
                AfterWritingSourceMap(this, new SourceMapEventArgs(file, mapFile, containsChanges));
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
