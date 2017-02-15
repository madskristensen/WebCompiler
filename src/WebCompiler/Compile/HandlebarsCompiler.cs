using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace WebCompiler
{
    class HandlebarsCompiler : ICompiler
    {
        private static Regex _errorRx = new Regex("Error: (?<message>.+) on line (?<line>[0-9]+):", RegexOptions.Compiled);
        private string _mapPath;
        private string _path;
        private string _name = string.Empty;
        private string _extension = string.Empty;
        private string _output = string.Empty;
        private string _error = string.Empty;
        private bool _partial = false;

        public HandlebarsCompiler(string path)
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

            var extension = Path.GetExtension(inputFile);
            if (!string.IsNullOrWhiteSpace(extension))
            {
                _extension = extension.Substring(1);
            }
            
            var name = Path.GetFileNameWithoutExtension(inputFile);
            if (!string.IsNullOrWhiteSpace(name) && name.StartsWith("_"))
            {
                _name = name.Substring(1);
                _partial = true;

                // Temporarily Fix
                // TODO: Remove after actual fix
                var tempFilename = Path.Combine(Path.GetDirectoryName(inputFile), _name + ".handlebarstemp");
                info.CopyTo(tempFilename);
                info = new FileInfo(tempFilename);
                _extension = "handlebarstemp";
            }

            _mapPath = Path.ChangeExtension(inputFile, ".js.map.tmp");

            try
            {                
                RunCompilerProcess(config, info);

                result.CompiledContent = _output;

                var options = HandlebarsOptions.FromConfig(config);
                if (options.SourceMap || config.SourceMap)
                {
                    if (File.Exists(_mapPath))
                        result.SourceMap = File.ReadAllText(_mapPath);
                }

                if (_error.Length > 0)
                {
                    CompilerError ce = new CompilerError
                    {
                        FileName = inputFile,
                        Message = _error.Replace(baseFolder, string.Empty),
                        IsWarning = !string.IsNullOrEmpty(_output)
                    };

                    var match = _errorRx.Match(_error);

                    if (match.Success)
                    {
                        ce.Message = match.Groups["message"].Value.Replace(baseFolder, string.Empty);
                        ce.LineNumber = int.Parse(match.Groups["line"].Value);
                        ce.ColumnNumber = 0;
                    }

                    result.Errors.Add(ce);
                }
            }
            catch (Exception ex)
            {
                CompilerError error = new CompilerError
                {
                    FileName = inputFile,
                    Message = string.IsNullOrEmpty(_error) ? ex.Message : _error,
                    LineNumber = 0,
                    ColumnNumber = 0,
                };

                result.Errors.Add(error);
            }
            finally
            {
                if (File.Exists(_mapPath))
                {
                    File.Delete(_mapPath);
                }
                // Temporarily Fix
                // TODO: Remove after actual fix
                if (info.Extension == ".handlebarstemp")
                {
                    info.Delete();
                }
            }

            return result;
        }

        private void RunCompilerProcess(Config config, FileInfo info)
        {
            string arguments = ConstructArguments(config);

            ProcessStartInfo start = new ProcessStartInfo
            {
                WorkingDirectory = info.Directory.FullName,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = "cmd.exe",
                Arguments = $"/c \"\"{Path.Combine(_path, "node_modules\\.bin\\handlebars.cmd")}\" \"{info.FullName}\" {arguments}\"",
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            start.EnvironmentVariables["PATH"] = _path + ";" + start.EnvironmentVariables["PATH"];

            using (Process p = Process.Start(start))
            {
                var stdout = p.StandardOutput.ReadToEndAsync();
                var stderr = p.StandardError.ReadToEndAsync();
                p.WaitForExit();

                _output = stdout.Result.Trim();
                _error = stderr.Result.Trim();
            }
        }

        private string ConstructArguments(Config config)
        {
            string arguments = "";

            HandlebarsOptions options = HandlebarsOptions.FromConfig(config);

            if (options.AMD)
                arguments += " --amd";
            else if (!string.IsNullOrEmpty(options.CommonJS))
                arguments += $" --commonjs \"{options.CommonJS}\"";

            foreach (var knownHelper in options.KnownHelpers)
            {
                arguments += $" --known \"{knownHelper}\"";
            }

            if (options.KnownHelpersOnly)
                arguments += " --knownOnly";

            if (options.ForcePartial || _partial)
                arguments += " --partial";

            if (options.NoBOM)
                arguments += " --bom";

            if ((options.SourceMap || config.SourceMap) && !string.IsNullOrWhiteSpace(_mapPath))
                arguments += $" --map \"{_mapPath}\"";

            if (!string.IsNullOrEmpty(options.TemplateNameSpace))
                arguments += $" --namespace \"{options.TemplateNameSpace}\"";

            if (!string.IsNullOrEmpty(options.Root))
                arguments += $" --root \"{options.Root}\"";

            if (!string.IsNullOrEmpty(options.Name))
                arguments += $" --name \"{options.Name}\"";
            else if (!string.IsNullOrEmpty(_name))
                arguments += $" --name \"{_name}\"";

            if (!string.IsNullOrEmpty(_extension))
                arguments += $" --extension \"{_extension}\"";

            return arguments;
        }
    }
}
