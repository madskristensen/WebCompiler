﻿using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace WebCompiler
{
    /// <summary>
    /// Represents a configuration object used by the compilers.
    /// </summary>
    public class Config
    {
        /// <summary>
        /// The file path to the configuration file.
        /// </summary>
        [JsonIgnore]
        public string FileName { get; set; }

        /// <summary>
        /// The relative file path to the output file.
        /// </summary>
        [JsonProperty("outputFile")]
        public string OutputFile { get; set; }

        /// <summary>
        /// The relative file path to the input file.
        /// </summary>
        [JsonProperty("inputFile")]
        public string InputFile { get; set; }

        /// <summary>
        /// The relative file path to the output files.
        /// </summary>
        [JsonProperty("outputFiles")]
        public string OutputFiles { get; set; }

        /// <summary>
        /// The relative file path to the input files.
        /// </summary>
        [JsonProperty("inputFiles")]
        public string InputFiles { get; set; }

        /// <summary>
        /// Settings for the minification.
        /// </summary>
        [JsonProperty("minify")]
        public Dictionary<string, object> Minify { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// If true it makes Visual Studio include the output file in the project.
        /// </summary>
        [JsonProperty("includeInProject")]
        public bool IncludeInProject { get; set; } = true;

        /// <summary>
        /// If true a source map file is generated for the file types that support it.
        /// </summary>
        [JsonProperty("sourceMap")]
        public bool SourceMap { get; set; }

        /// <summary>
        /// Options specific to each compiler. Based on the inputFile property.
        /// </summary>
        [JsonProperty("options")]
        public Dictionary<string, object> Options { get; set; } = new Dictionary<string, object>();

        internal string Output { get; set; }

        /// <summary>
        /// Converts the relative input file to an absolute file path.
        /// </summary>
        public FileInfo GetAbsoluteInputFile()
        {
            return new FileInfo(GetAbsolutePath(InputFile));
        }

        /// <summary>
        /// Converts the relative output file to an absolute file path.
        /// </summary>
        public FileInfo GetAbsoluteOutputFile()
        {
            return new FileInfo(GetAbsolutePath(OutputFile));
        }

        private string GetAbsolutePath(string relativePath)
        {
            string folder = new FileInfo(FileName).DirectoryName;
            return Path.Combine(folder, relativePath.Replace("/", "\\"));
        }

        private string GetRelativePath(string absolutePath)
        {
            if (absolutePath == null)
                return null;

            string folder = new FileInfo(FileName).DirectoryName;
            return absolutePath.Replace(folder, string.Empty).TrimStart('\\').Replace("\\", "/");
        }

        /// <summary>
        /// Checks to see if the input file needs compilation
        /// </summary>
        public bool CompilationRequired()
        {
            FileInfo input = GetAbsoluteInputFile();
            FileInfo output = GetAbsoluteOutputFile();

            if (!output.Exists)
                return true;

            if (input.LastWriteTimeUtc > output.LastWriteTimeUtc)
                return true;

            return HasDependenciesNewerThanOutput(input, output);
        }

        private bool HasDependenciesNewerThanOutput(FileInfo input, FileInfo output)
        {
            var projectRoot = new FileInfo(FileName).DirectoryName;
            var dependencies = DependencyService.GetDependencies(projectRoot, input.FullName);

            if (dependencies != null)
            {
                string key = input.FullName.ToLowerInvariant();
                return CheckForNewerDependenciesRecursively(key, dependencies, output);
            }

            return false;
        }

        private bool CheckForNewerDependenciesRecursively(string key, Dictionary<string, Dependencies> dependencies, FileInfo output)
        {
            if (!dependencies.ContainsKey(key))
                return false;

            foreach (var file in dependencies[key].DependentOn.ToArray())
            {
                var fileInfo = new FileInfo(file);

                if (!fileInfo.Exists)
                    continue;

                if (fileInfo.LastWriteTimeUtc > output.LastWriteTimeUtc)
                    return true;

                if (CheckForNewerDependenciesRecursively(file, dependencies, output))
                    return true;
            }

            return false;
        }

        internal IEnumerable<Config> ExpandConfig()
        {
            string inputFiles = InputFiles;
            string outputFiles = OutputFiles;

            InputFiles = OutputFiles = null;

            if (!string.IsNullOrEmpty(InputFile) && !string.IsNullOrEmpty(OutputFile))
            {
                yield return this;
            }

            if (string.IsNullOrEmpty(inputFiles) || string.IsNullOrEmpty(outputFiles))
            {
                yield break;
            }

            inputFiles = GetAbsolutePath(inputFiles);
            outputFiles = GetAbsolutePath(outputFiles);

            string inputFolder = Path.GetDirectoryName(inputFiles);
            string inputFilePattern = Path.GetFileName(inputFiles);
            string outputFolder = Path.GetDirectoryName(outputFiles);
            string outputFilePattern = Path.GetFileName(outputFiles);

            foreach (var filePath in Directory.GetFiles(inputFolder, inputFilePattern))
            {
                var fileConfig = Clone();
                fileConfig.InputFile = GetRelativePath(filePath);
                fileConfig.OutputFile = GetRelativePath(Path.Combine(outputFolder,
                                                                        Path.ChangeExtension(Path.GetFileName(filePath), Path.GetExtension(outputFilePattern))));
                yield return fileConfig;
            }
        }

        private Config Clone()
        {
            var clone = (Config)MemberwiseClone();
            clone.Minify = Minify.AsEnumerable().ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            clone.Options = Options.AsEnumerable().ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            return clone;
        }

        /// <summary>
        /// Determines if the object is equals to the other object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != GetType()) return false;
            if (obj == this) return true;

            Config other = (Config)obj;

            return GetHashCode() == other.GetHashCode();
        }

        /// <summary>
        /// Returns the hash code for this Config
        /// </summary>
        public override int GetHashCode()
        {
            return OutputFile.GetHashCode();
        }

        /// <summary>For the JSON.NET serializer</summary>
        public bool ShouldSerializeIncludeInProject()
        {
            Config config = new Config();
            return IncludeInProject != config.IncludeInProject;
        }

        /// <summary>For the JSON.NET serializer</summary>
        public bool ShouldSerializeMinify()
        {
            Config config = new Config();
            return !DictionaryEqual(Minify, config.Minify, null);
        }

        /// <summary>For the JSON.NET serializer</summary>
        public bool ShouldSerializeOptions()
        {
            Config config = new Config();
            return !DictionaryEqual(Options, config.Options, null);
        }

        private static bool DictionaryEqual<TKey, TValue>(
            IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second,
            IEqualityComparer<TValue> valueComparer)
        {
            if (first == second) return true;
            if ((first == null) || (second == null)) return false;
            if (first.Count != second.Count) return false;

            valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;

            foreach (var kvp in first)
            {
                TValue secondValue;
                if (!second.TryGetValue(kvp.Key, out secondValue)) return false;
                if (!valueComparer.Equals(kvp.Value, secondValue)) return false;
            }
            return true;
        }
    }
}
