using System.IO;
using Newtonsoft.Json;

namespace WebCompiler
{
    public class Config
    {
        [JsonIgnore]
        public string FileName { get; set; }

        [JsonProperty("outputFile")]
        public string OutputFile { get; set; }

        [JsonProperty("inputFile")]
        public string InputFile { get; set; }

        [JsonProperty("minify")]
        public bool Minify { get; set; } = true;

        [JsonProperty("includeInProject")]
        public bool IncludeInProject { get; set; }

        [JsonProperty("sourceMap")]
        public bool SourceMap { get; set; }

        internal string Output { get; set; }

        public string GetAbsoluteOutputFile()
        {
            string folder = Path.GetDirectoryName(FileName);
            return Path.Combine(folder, OutputFile.Replace("/", "\\"));
        }
    }
}
