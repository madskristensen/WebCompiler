using System;
using System.IO;
using CoffeeSharp;

namespace WebCompiler
{
    public class CoffeeScriptCompiler : ICompiler
    {
        private static CoffeeScriptEngine _engine = new CoffeeScriptEngine();

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

            try
            {   
                string compilerResult = _engine.Compile(content, filename: info.FullName);

                result.CompiledContent = compilerResult;
            }
            catch (Exception ex)
            {
                CompilerError error = new CompilerError
                {
                    FileName = info.FullName,
                    Message = ex.Message.Replace(info.FullName, string.Empty).Trim()
                };

                if (error.Message.Contains("error on line "))
                {
                    int index = error.Message.IndexOf("error on line ") + 14;
                    int end = error.Message.IndexOf(':', index);
                    int line = 0;

                    if (int.TryParse(error.Message.Substring(index, end - index), out line))
                    {
                        error.LineNumber = line;
                        error.Message = error.Message.Substring(end + 1).Trim();
                    }
                }

                result.Errors.Add(error);
            }

            return result;
        }
    }
}
