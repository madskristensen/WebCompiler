using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace WebCompiler
{
    class LessCompiler : ICompiler
    {
        private static Regex _errorRx = new Regex("(?<message>.+) on line (?<line>[0-9]+), column (?<column>[0-9]+)", RegexOptions.Compiled);
        private string _path;

        public LessCompiler(string path)
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

            try
            {
                //var path = Path.Combine(Path.GetTempPath(), "WebCompiler");

                Process p = RunCompilerProcess(config, info);

                StringBuilder output = new StringBuilder();
                StringBuilder error = new StringBuilder();

                while (!p.HasExited)
                {
                    output.Append(p.StandardOutput.ReadToEnd());
                    error.Append(p.StandardError.ReadToEnd());
                }

                result.CompiledContent = output.ToString();

                if (error.Length > 0)
                {
                    string message = error.ToString();
                    CompilerError ce = new CompilerError
                    {
                        FileName = info.FullName,
                        Message = message.Replace(baseFolder, string.Empty),
                    };

                    var match = _errorRx.Match(message);

                    if (match.Success)
                    {
                        ce.Message = match.Groups["message"].Value.Replace(baseFolder, string.Empty);
                        ce.LineNumber = int.Parse(match.Groups["line"].Value);
                        ce.ColumnNumber = int.Parse(match.Groups["column"].Value);
                    }

                    result.Errors.Add(ce);
                }
            }
            catch (Exception ex)
            {
                CompilerError error = new CompilerError
                {
                    FileName = info.FullName,
                    Message = ex.Message,
                    LineNumber = 0,
                    ColumnNumber = 0,
                };

                result.Errors.Add(error);
            }

            return result;
        }

        private Process RunCompilerProcess(Config config, FileInfo info)
        {
            string arguments = ConstructArguments(config);

            ProcessStartInfo start = new ProcessStartInfo
            {
                WorkingDirectory = Path.Combine(_path, "node_modules\\.bin"),
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = "cmd.exe",
                Arguments = "/c lessc.cmd " + arguments + " \"" + info.FullName + "\"",
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            start.EnvironmentVariables["PATH"] = _path + ";" + start.EnvironmentVariables["PATH"];

            return Process.Start(start);
        }

        private static string ConstructArguments(Config config)
        {
            string arguments = " --no-color --relative-urls";

            if (config.SourceMap)
                arguments += " --source-map-map-inline";

            LessOptions options = new LessOptions(config);

            if (options.StrictMath)
                arguments += " --strict-math=on";

            return arguments;
        }
    }
}
