namespace WebCompiler
{
    /// <summary>
    /// Represents an error that occured in the compiler.
    /// </summary>
    public class CompilerError
    {
        /// <summary>
        /// The absolute file path of the file being compiled.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The error message from the compiler.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The line number in the source file where the error happened.
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// The column number in the source file where the error happened.
        /// </summary>
        public int ColumnNumber { get; set; }

        /// <summary>
        /// Specifies whether or not the error should be treated as a warning.
        /// </summary>
        public bool IsWarning { get; set; }
    }
}
