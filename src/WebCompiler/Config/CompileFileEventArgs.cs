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
        public CompileFileEventArgs(Config config, string baseFolder)
        {
            Config = config;
            BaseFolder = baseFolder;
        }

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
