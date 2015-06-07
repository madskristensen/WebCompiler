using System.IO;
using dotless.Core;

namespace WebCompiler
{
    public class LessCompiler : ICompiler
    {
        public CompilerResult Compile(Config config)
        {
            string baseFolder = Path.GetDirectoryName(config.FileName);
            string inputFile = Path.Combine(baseFolder, config.InputFile);

            FileInfo info = new FileInfo(inputFile);
            string content = File.ReadAllText(info.FullName);

            var engine = new LessEngine();
            engine.CurrentDirectory = info.Directory.FullName;

            string css = engine.TransformToCss(content, info.FullName);

            CompilerResult result = new CompilerResult
            {
                FileName = info.FullName,
                OriginalContent = content,
                CompiledContent = css,
            };

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
            
            return result;
        }
    }
}
