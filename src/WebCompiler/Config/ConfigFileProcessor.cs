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
        /// <param name="force">Forces compilation of all config items.</param>
        /// <returns>A list of compiler results.</returns>
        public IEnumerable<CompilerResult> Process(string configFile, IEnumerable<Config> configs = null, bool force = false)
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
                    if (force || config.CompilationRequired())
                    {
                        var result = ProcessConfig(info.Directory.FullName, config);
                        list.Add(result);
                        OnConfigProcessed(config, list.Count, configs.Count());
                    }
                }
            }
            finally
            {
                if (_processing.Contains(configFile))
                    _processing.Remove(configFile);
            }

            return list;
        }

        /// <summary>
        /// Parses a compiler config file and deletes all outputs including .min and .min.map files
        /// </summary>
        public void DeleteOutputFiles(string configFile)
        {
            var configs = ConfigHandler.GetConfigs(configFile);
            foreach (var item in configs)
            {
                var outputFile = item.GetAbsoluteOutputFile().FullName;
                var minFile = Path.ChangeExtension(outputFile, ".min" + Path.GetExtension(outputFile));
                var mapFile = minFile + ".map";
                var gzipFile = minFile + ".gz";

                DeleteFile(outputFile);
                DeleteFile(minFile);
                DeleteFile(mapFile);
                DeleteFile(gzipFile);
            }
        }

        static void DeleteFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                FileHelpers.RemoveReadonlyFlagFromFile(fileName);
                File.Delete(fileName);
            }
        }

        /// <summary>
        /// Compiles all configs with the same input file extension as the specified sourceFile
        /// </summary>
        public IEnumerable<CompilerResult> SourceFileChanged(string configFile,
                                                             string sourceFile,
                                                             string projectPath)
        {
            return SourceFileChanged(configFile, sourceFile, projectPath, new HashSet<string>());
        }

        /// <summary>
        /// Compiles all configs with the same input file extension as the specified sourceFile
        /// </summary>
        private IEnumerable<CompilerResult> SourceFileChanged(string configFile,
                                                              string sourceFile,
                                                              string projectPath,
                                                              HashSet<string> compiledFiles)
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
                    {
                        list.Add(ProcessConfig(folder, config));
                        compiledFiles.Add(input.ToLowerInvariant());
                    }
                }

                //compile files that are dependent on the current file
                var dependencies = DependencyService.GetDependencies(projectPath, sourceFile);
                if (dependencies != null)
                {
                    string key = sourceFile.ToLowerInvariant();

                    if (dependencies.ContainsKey(key))
                    {
                        //compile all files that have references to the compiled file
                        foreach (var file in dependencies[key].DependentFiles.ToArray())
                        {
                            if (!compiledFiles.Contains(file.ToLowerInvariant()))
                                list.AddRange(SourceFileChanged(configFile, file, projectPath, compiledFiles));
                        }
                    }
                }
                else
                {
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
                }

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
            catch (Exception)
            {
                return null;
            }
        }

        private CompilerResult ProcessConfig(string baseFolder, Config config)
        {
            ICompiler compiler = CompilerService.GetCompiler(config);

            var result = compiler.Compile(config);

            if (result.Errors.Any(e => !e.IsWarning))
                return result;

            if (Path.GetExtension(config.OutputFile).Equals(".css", StringComparison.OrdinalIgnoreCase) && AdjustRelativePaths(config))
            {
                result.CompiledContent = CssRelativePath.Adjust(result.CompiledContent, config);
            }

            config.Output = result.CompiledContent;

            FileInfo outputFile = config.GetAbsoluteOutputFile();
            bool containsChanges = FileHelpers.HasFileContentChanged(outputFile.FullName, config.Output);

            OnBeforeProcess(config, baseFolder, containsChanges);

            if (containsChanges)
            {
                string dir = outputFile.DirectoryName;

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                File.WriteAllText(outputFile.FullName, config.Output, new UTF8Encoding(true));
            }

            OnAfterProcess(config, baseFolder, containsChanges);

            //if (!config.Minify.ContainsKey("enabled") || config.Minify["enabled"].ToString().Equals("true", StringComparison.OrdinalIgnoreCase))
            //{
            FileMinifier.MinifyFile(config);
            //}

            if (!string.IsNullOrEmpty(result.SourceMap))
            {
                string absolute = config.GetAbsoluteOutputFile().FullName;
                string mapFile = absolute + ".map";
                bool smChanges = FileHelpers.HasFileContentChanged(mapFile, result.SourceMap);

                OnBeforeWritingSourceMap(absolute, mapFile, smChanges);

                if (smChanges)
                {
                    File.WriteAllText(mapFile, result.SourceMap, new UTF8Encoding(true));
                }

                OnAfterWritingSourceMap(absolute, mapFile, smChanges);
            }

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
