using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WebCompiler
{
    public class ConfigFileProcessor
    {
        public IEnumerable<CompilerResult> Process(string fileName)
        {
            FileInfo info = new FileInfo(fileName);
            var configs = ConfigHandler.GetConfigs(fileName);
            List<CompilerResult> list = new List<CompilerResult>();

            foreach (Config config in configs)
            {
                var result = ProcessConfig(info.Directory.FullName, config);
                list.Add(result);
            }

            return list;
        }

        public IEnumerable<CompilerResult> SourceFileChanged(string configFile, string sourceFile)
        {
            string folder = Path.GetDirectoryName(configFile);
            List<CompilerResult> list = new List<CompilerResult>();

            var configs = IsFileConfigured(configFile, sourceFile);

            foreach (Config config in configs)
            {
                string input = Path.Combine(folder, config.InputFile.Replace("/", "\\"));
                list.Add(ProcessConfig(folder, config));
            }

            return list;
        }

        public static IEnumerable<Config> IsFileConfigured(string configFile, string sourceFile)
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

            if (config.Minify)
            {
                var minResult = FileMinifier.MinifyFile(config.GetAbsoluteOutputFile(), config.SourceMap);
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

        protected void OnBeforeProcess(Config bundle, string baseFolder)
        {
            if (BeforeProcess != null)
            {
                BeforeProcess(this, new CompileFileEventArgs(bundle, baseFolder));
            }
        }

        protected void OnAfterProcess(Config bundle, string baseFolder)
        {
            if (AfterProcess != null)
            {
                AfterProcess(this, new CompileFileEventArgs(bundle, baseFolder));
            }
        }

        protected void OnBeforeWritingSourceMap(string file, string mapFile)
        {
            if (BeforeWritingSourceMap != null)
            {
                BeforeWritingSourceMap(this, new SourceMapEventArgs(file, mapFile));
            }
        }

        protected void OnAfterWritingSourceMap(string file, string mapFile)
        {
            if (AfterWritingSourceMap != null)
            {
                AfterWritingSourceMap(this, new SourceMapEventArgs(file, mapFile));
            }
        }

        public event EventHandler<CompileFileEventArgs> BeforeProcess;
        public event EventHandler<CompileFileEventArgs> AfterProcess;

        public event EventHandler<SourceMapEventArgs> BeforeWritingSourceMap;
        public event EventHandler<SourceMapEventArgs> AfterWritingSourceMap;
    }
}
