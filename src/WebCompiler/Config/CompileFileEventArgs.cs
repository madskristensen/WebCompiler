using System;

namespace WebCompiler
{
    /// <summary>
    /// The event arguments from running the compiler on a Config object.
    /// </summary>
    public class CompileFileEventArgs : EventArgs
    {
        /// <summary>
        /// Creates an instance and populates the config and baseFolder properties.
        /// </summary>
        /// <param name="config">The configuration object being processed.</param>
        /// <param name="baseFolder">The base folder of the config file.</param>
        /// <param name="containsChanges">True if there were any changes.</param>
        public CompileFileEventArgs(Config config, string baseFolder, bool containsChanges)
        {
            ContainsChanges = containsChanges;
            Config = config;
            BaseFolder = baseFolder;
        }

        /// <summary>
        /// True if the output produced any changes to files on disk.
        /// </summary>
        public bool ContainsChanges { get; set; }

        /// <summary>
        /// The Config object used by the compiler.
        /// </summary>
        public Config Config { get; set; }

        /// <summary>
        /// The base folder of the current execution.
        /// </summary>
        public string BaseFolder { get; set; }
    }
}
