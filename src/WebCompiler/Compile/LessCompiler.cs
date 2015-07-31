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
        private StringBuilder _output = new StringBuilder();
        private StringBuilder _error = new StringBuilder();

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

                RunCompilerProcess(config, info);

                result.CompiledContent = _output.ToString();

                if (_error.Length > 0)
                {
                    string message = _error.ToString();
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

        private void RunCompilerProcess(Config config, FileInfo info)
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

            Process p = new Process();
            p.StartInfo = start;
            p.EnableRaisingEvents = true;
            p.OutputDataReceived += (s, e) => { if (e.Data != null) _output.Append(e.Data); };
            p.ErrorDataReceived += (s, e) => { if (e.Data != null) _error.Append(e.Data); };
            p.Start();
            p.BeginErrorReadLine();
            p.BeginOutputReadLine();
            p.WaitForExit();
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
