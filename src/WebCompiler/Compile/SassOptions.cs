namespace WebCompiler
{
    /// <summary>
    /// Give all options for the Sass compiler
    /// </summary>
    public class SassOptions : BaseOptions
    {
        private const string trueStr = "true";

        /// <summary>
        /// Create an instance of the Class SassOptions
        /// </summary>
        /// <param name="config">The Scss configuration file.</param>
        public SassOptions(Config config)
        {
            if (config.Options.ContainsKey("outputStyle"))
            {
                OutputStyle = config.Options["outputStyle"].ToString();
            }
            
            int precision = 5;
            if (int.TryParse(GetValue(config, "precision"), out precision))
                Precision = precision;
        }

        /// <summary>
        /// Type of output style
        /// </summary>
        public string OutputStyle { get; set; }
        

        /// <summary>
        /// Precision
        /// </summary>
        public int Precision { get; set; } = 5;
    }
}
