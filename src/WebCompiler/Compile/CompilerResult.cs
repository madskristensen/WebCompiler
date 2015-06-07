using System.Collections.Generic;

namespace WebCompiler
{
    public class CompilerResult
    {
        public string FileName { get; set; }

        public string OriginalContent { get; set; }

        public string CompiledContent { get; set; }

        public string SourceMap { get; set; }

        public bool HasErrors
        {
            get { return Errors.Count > 0; }

        }

        public List<CompilerError> Errors { get; set; } = new List<CompilerError>();
    }
}
