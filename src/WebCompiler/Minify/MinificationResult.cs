namespace WebCompiler
{
    /// <summary>
    /// Represents the result of minification.
    /// </summary>
    public class MinificationResult
    {
        /// <summary>
        /// Creates a new instance and populates the content and sourceMap properties.
        /// </summary>
        /// <param name="content">The minified content.</param>
        /// <param name="sourceMap">The generated sourceMap</param>
        public MinificationResult(string content, string sourceMap)
        {
            MinifiedContent = content;
            SourceMap = sourceMap;
        }

        /// <summary>
        /// The content after it has been minified.
        /// </summary>
        public string MinifiedContent { get; set; }

        /// <summary>
        /// The content of the sourceMap after minification.
        /// </summary>
        public string SourceMap { get; set; }
    }
}
