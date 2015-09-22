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
        /// <param name="containsChanges">True if there were any changes.</param>
        public SourceMapEventArgs(string originalFile, string resultFile, bool containsChanges)
        {
            ContainsChanges = containsChanges;
            OriginalFile = originalFile;
            ResultFile = resultFile;
        }

        /// <summary>
        /// True if the output produced any changes to files on disk.
        /// </summary>
        public bool ContainsChanges { get; set; }

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
