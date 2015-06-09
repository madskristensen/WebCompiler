using System;
using System.IO;
using dotless.Core;

namespace WebCompiler
{
    class LessCompiler : ICompiler
    {
        public CompilerResult Compile(Config config)
        {
            string baseFolder = Path.GetDirectoryName(config.FileName);
            string inputFile = Path.Combine(baseFolder, config.InputFile);

            FileInfo info = new FileInfo(inputFile);
            string content = File.ReadAllText(info.FullName);

            var engine = new LessEngine();
            engine.CurrentDirectory = info.Directory.FullName;

            ApplyOptions(config, engine);

            CompilerResult result = new CompilerResult
            {
                FileName = info.FullName,
                OriginalContent = content,
            };

            try
            {
                string css = engine.TransformToCss(content, info.FullName);
                result.CompiledContent = css;

                if (engine.LastTransformationError != null)
                {
                    CompilerError error = new CompilerError
                    {
                        FileName = info.FullName,
                        Message = engine.LastTransformationError.Message.Trim(),
                        LineNumber = engine.LastTransformationError.ErrorLocation.LineNumber,
                        ColumnNumber = engine.LastTransformationError.ErrorLocation.Position
                    };

                    result.Errors.Add(error);
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

        private static void ApplyOptions(Config config, LessEngine engine)
        {
            LessOptions options = new LessOptions(config);
            engine.StrictMath = options.StrictMath;
            engine.KeepFirstSpecialComment = options.KeepFirstSpecialComment;
            engine.DisableVariableRedefines = options.DisableVariableRedefines;
            engine.DisableColorCompression = options.DisableColorCompression;
        }
    }
}
