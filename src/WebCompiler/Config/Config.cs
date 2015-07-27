using System.Collections.Generic;
using System.IO;
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
        /// Settings for the minification.
        /// </summary>
        [JsonProperty("minify")]
        public Dictionary<string, object> Minify { get; set; } = new Dictionary<string, object> { { "enabled", true } };

        /// <summary>
        /// If true it makes Visual Studio include the output file in the project.
        /// </summary>
        [JsonProperty("includeInProject")]
        public bool IncludeInProject { get; set; }

        /// <summary>
        /// If true a source map file is generated for the file types that support it.
        /// </summary>
        [JsonProperty("sourceMap")]
        public bool SourceMap { get; set; }

        /// <summary>
        /// Options specific to each compiler. Based on the inputFile property.
        /// </summary>
        [JsonProperty("options")]
        public Dictionary<string, string> Options { get; set; } = new Dictionary<string, string>();

        internal string Output { get; set; }

        /// <summary>
        /// Converts the relative input file to an absolute file path.
        /// </summary>
        public string GetAbsoluteInputFile()
        {
            string folder = Path.GetDirectoryName(FileName);
            return Path.Combine(folder, InputFile.Replace("/", "\\"));
        }

        /// <summary>
        /// Converts the relative output file to an absolute file path.
        /// </summary>
        public string GetAbsoluteOutputFile()
        {
            string folder = Path.GetDirectoryName(FileName);
            return Path.Combine(folder, OutputFile.Replace("/", "\\"));
        }

        /// <summary>
        /// Determins if the object is equals to the other object.
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
    }
}
