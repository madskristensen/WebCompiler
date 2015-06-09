using System;
using System.IO;
using System.Text.RegularExpressions;
using CoffeeSharp;

namespace WebCompiler
{
    internal class CoffeeScriptCompiler : ICompiler
    {
        private static CoffeeScriptEngine _engine = new CoffeeScriptEngine();
        private static Regex _error = new Regex(":(?<line>[0-9]+):(?<column>[0-9]+):(?<message>.+)", RegexOptions.Compiled);

        public CompilerResult Compile(Config config)
        {
            string baseFolder = Path.GetDirectoryName(config.FileName);
            string inputFile = Path.Combine(baseFolder, config.InputFile);

            FileInfo info = new FileInfo(inputFile);
            string content = File.ReadAllText(info.FullName);

            string sourceMap = config.SourceMap ? inputFile + ".map" : null;

            CompilerResult result = new CompilerResult
            {
                FileName = info.FullName,
                OriginalContent = content,
            };

            CoffeeScriptOptions options = new CoffeeScriptOptions(config);

            try
            {
                string compilerResult = _engine.Eval(content, filename: info.FullName, bare: options.Bare, globals: options.Globals);

                result.CompiledContent = compilerResult;
            }
            catch (Exception ex)
            {
                CompilerError error = new CompilerError
                {
                    FileName = info.FullName,
                    Message = ex.Message.Replace(info.FullName, string.Empty).Trim()
                };

                Match match = _error.Match(ex.Message);

                if (match.Success)
                {
                    int line;
                    if (int.TryParse(match.Groups["line"].Value, out line))
                        error.LineNumber = line;

                    int column;
                    if (int.TryParse(match.Groups["column"].Value, out column))
                        error.ColumnNumber = column;

                    error.Message = match.Groups["message"].Value.Trim();
                }

                result.Errors.Add(error);
            }

            return result;
        }
    }
}
