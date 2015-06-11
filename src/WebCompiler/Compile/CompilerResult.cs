using System.Collections.Generic;

namespace WebCompiler
{
    /// <summary>
    /// Contians the result of a compilation.
    /// </summary>
    public class CompilerResult
    {
        /// <summary>
        /// The file being compiled.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The content of the source file.
        /// </summary>
        public string OriginalContent { get; set; }

        /// <summary>
        /// The compile output string.
        /// </summary>
        public string CompiledContent { get; set; }

        /// <summary>
        /// The source map string produced by the compiler.
        /// </summary>
        public string SourceMap { get; set; }

        /// <summary>
        /// A collection of any errors reported by the compiler.
        /// </summary>
        public List<CompilerError> Errors { get; set; } = new List<CompilerError>();

        /// <summary>
        /// Checks if the compilation resulted in errors.
        /// </summary>
        public bool HasErrors
        {
            get { return Errors.Count > 0; }
        }
    }
}
