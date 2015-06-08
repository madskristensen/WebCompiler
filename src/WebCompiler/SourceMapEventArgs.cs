using System;

namespace WebCompiler
{
    /// <summary>
    /// Event arguments for writing source map files to disk.
    /// </summary>
    public class SourceMapEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new instance of the event arguments.
        /// </summary>
        /// <param name="originalFile">The original file to where the source map points to.</param>
        /// <param name="resultFile">The .map file that was generated.</param>
        public SourceMapEventArgs(string originalFile, string resultFile)
        {
            OriginalFile = originalFile;
            ResultFile = resultFile;
        }

        /// <summary>
        /// The original file to where the source map points to.
        /// </summary>
        public string OriginalFile { get; set; }

        /// <summary>
        /// The .map file that was generated.
        /// </summary>
        public string ResultFile { get; set; }

    }
}
