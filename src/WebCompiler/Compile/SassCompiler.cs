using System;
using System.IO;

namespace WebCompiler
{
    internal class SassCompiler : ICompiler
    {
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

            SassOptions options = new SassOptions(config);

            try
            {
                LibSassNet.SassCompiler compiler = new LibSassNet.SassCompiler();
                var compilerResult = compiler.CompileFile(inputFile, options.OutputStyle, sourceMap, options.IncludeSourceComments, options.Precision);
                result.CompiledContent = compilerResult.CSS;
                result.SourceMap = compilerResult.SourceMap;
            }
            catch (Exception ex)
            {
                CompilerError error = new CompilerError
                {
                    FileName = info.FullName,
                    Message = ex.Message.Replace("/", "\\").Replace(info.FullName, string.Empty).Trim()
                };

                if (error.Message.StartsWith(":"))
                {
                    int end = error.Message.IndexOf(':', 1);
                    int line = 0;
                    if (int.TryParse(error.Message.Substring(1, end - 1), out line))
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
