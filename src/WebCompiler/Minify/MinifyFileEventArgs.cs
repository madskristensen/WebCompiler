using System;

namespace WebCompiler
{
    /// <summary>
    /// Represents the event arguments from file minification.
    /// </summary>
    public class MinifyFileEventArgs : EventArgs
    {
        /// <summary>
        /// Creates an instance and populates the originalFile and resultFile properties.
        /// </summary>
        /// <param name="originalFile">The file path to the original file.</param>
        /// <param name="resultFile">The file path to the minified file.</param>
        /// <param name="containsChanges">True if there were any changes.</param>
        public MinifyFileEventArgs(string originalFile, string resultFile, bool containsChanges)
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
        /// The file path to the original file.
        /// </summary>
        public string OriginalFile { get; set; }

        /// <summary>
        /// The file path to the minified file.
        /// </summary>
        public string ResultFile { get; set; }

    }
}
