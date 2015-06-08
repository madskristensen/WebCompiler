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
        public MinifyFileEventArgs(string originalFile, string resultFile)
        {
            OriginalFile = originalFile;
            ResultFile = resultFile;
        }

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
