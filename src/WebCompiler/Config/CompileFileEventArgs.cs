using System;

namespace WebCompiler
{
    public class CompileFileEventArgs : EventArgs
    {
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
