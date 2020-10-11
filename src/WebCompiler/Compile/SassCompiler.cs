﻿using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace WebCompiler
{
    class SassCompiler : ICompiler
    {
        private static Regex _errorRx = new Regex("(?<message>.+) on line (?<line>[0-9]+), column (?<column>[0-9]+)", RegexOptions.Compiled);
        private string _path;
        private string _output = string.Empty;
        private string _error = string.Empty;

        public SassCompiler(string path)
        {
            _path = path;
        }

        public CompilerResult Compile(Config config)
        {
            string baseFolder = Path.GetDirectoryName(config.FileName);
            string inputFile = Path.Combine(baseFolder, config.InputFile);

            FileInfo info = new FileInfo(inputFile);
            string content = File.ReadAllText(info.FullName);

            CompilerResult result = new CompilerResult
            {
                FileName = info.FullName,
                OriginalContent = content,
            };

            if (config.GlobalMatch && string.IsNullOrWhiteSpace(content) || Path.GetFileName(config.InputFile).StartsWith("_"))
                return result;

            try
            {
                RunCompilerProcess(config, info);

                int sourceMapIndex = _output.LastIndexOf("*/");
                if (sourceMapIndex > -1 && _output.Contains("sourceMappingURL=data:"))
                {
                    _output = _output.Substring(0, sourceMapIndex + 2);
                }

                result.CompiledContent = _output;

                if (_error.Length > 0)
                {
                    JObject json = JObject.Parse(_error);

                    CompilerError ce = new CompilerError
                    {
                        FileName = info.FullName,
                        Message = json["message"].ToString(),
                        ColumnNumber = int.Parse(json["column"].ToString()),
                        LineNumber = int.Parse(json["line"].ToString()),
                        IsWarning = !string.IsNullOrEmpty(_output)
                    };

                    result.Errors.Add(ce);
                }
            }
            catch (Exception ex)
            {
                CompilerError error = new CompilerError
                {
                    FileName = info.FullName,
                    Message = string.IsNullOrEmpty(_error) ? ex.Message : _error,
                    LineNumber = 0,
                    ColumnNumber = 0,
                };

                result.Errors.Add(error);
            }

            return result;
        }

        private void RunCompilerProcess(Config config, FileInfo info)
        {
            string arguments = ConstructArguments(config);

            ProcessStartInfo start = new ProcessStartInfo
            {
                WorkingDirectory = new FileInfo(config.FileName).DirectoryName, // use config's directory to fix source map relative paths
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = "cmd.exe",
                Arguments = $"/c \"\"{Path.Combine(_path, "node_modules\\.bin\\node-sass.cmd")}\" {arguments} \"{info.FullName}\" \"",
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            // Pipe output from node-sass to postcss if autoprefix option is set
            SassOptions options = SassOptions.FromConfig(config);
            if (!string.IsNullOrEmpty(options.AutoPrefix))
            {
                string postCssArguments = "--use autoprefixer";

                if (!options.SourceMap && !config.SourceMap)
                    postCssArguments += " --no-map";

                start.Arguments = start.Arguments.TrimEnd('"') + $" | \"{Path.Combine(_path, "node_modules\\.bin\\postcss.cmd")}\" {postCssArguments}\"";
                start.EnvironmentVariables.Add("BROWSERSLIST", options.AutoPrefix);
            }

            start.EnvironmentVariables["PATH"] = _path + ";" + start.EnvironmentVariables["PATH"];

            using (Process p = Process.Start(start))
            {
                var stdout = p.StandardOutput.ReadToEndAsync();
                var stderr = p.StandardError.ReadToEndAsync();
                p.WaitForExit();

                _output = stdout.Result;
                // postcss outputs "√ Finished stdin (##ms)" to stderr for some reason
                if (!stderr.Result.StartsWith("√"))
                    _error = stderr.Result;
            }
        }

        private static string ConstructArguments(Config config)
        {
            string arguments = "";

            SassOptions options = SassOptions.FromConfig(config);

            if (options.SourceMap || config.SourceMap)
                arguments += " --source-map-embed=true";

            arguments += " --precision=" + options.Precision;

            if (!string.IsNullOrEmpty(options.OutputStyle))
                arguments += " --output-style=" + options.OutputStyle;

            if (!string.IsNullOrEmpty(options.IndentType))
                arguments += " --indent-type=" + options.IndentType;

            if (options.IndentWidth > -1)
                arguments += " --indent-width=" + options.IndentWidth;

            if (!string.IsNullOrEmpty(options.IncludePath))
                arguments += " --include-path=" + options.IncludePath;

            if (!string.IsNullOrEmpty(options.SourceMapRoot))
                arguments += " --source-map-root=" + options.SourceMapRoot;

            if (!string.IsNullOrEmpty(options.LineFeed))
                arguments += " --linefeed=" + options.LineFeed;

            return arguments;
        }
    }
}
