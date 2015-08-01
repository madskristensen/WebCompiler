using System;

namespace WebCompiler
{
    /// <summary>
    /// The event arguments from running the compiler on a Config object.
    /// </summary>
    public class ConfigProcessedEventArgs : EventArgs
    {
        /// <summary>
        /// Creates an instance and populates the config and baseFolder properties.
        /// </summary>
        /// <param name="config">The configuration object being processed.</param>
        /// <param name="amountProcessed">The amount of completed configs in the .json file.</param>
        /// <param name="total">Total number of configs in the .json file</param>
        public ConfigProcessedEventArgs(Config config, int amountProcessed, int total)
        {
            Config = config;
            AmountProcessed = amountProcessed;
            Total = total;
        }

        /// <summary>
        /// The Config object used by the compiler.
        /// </summary>
        public Config Config { get; set; }

        /// <summary>
        /// The amount of completed configs in the .json file.
        /// </summary>
        public int AmountProcessed { get; set; }

        /// <summary>
        /// Total number of configs in the .json file
        /// </summary>
        public int Total { get; set; }
    }
}
