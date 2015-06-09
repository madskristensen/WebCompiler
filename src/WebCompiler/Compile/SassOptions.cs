using LibSassNet;

namespace WebCompiler
{
    /// <summary>
    /// Give all options for the Sass compiler
    /// </summary>
    public class SassOptions : BaseOptions
    {
        /// <summary>
        /// Create an instance of the Class SassOptions
        /// </summary>
        /// <param name="config">The Scss configuration file.</param>
        public SassOptions(Config config)
        {
            if (config.Options.ContainsKey("outputStyle"))
            {
                string style = config.Options["outputStyle"];

                if (style == "nested")
                    OutputStyle = OutputStyle.Nested;
                else if (style == "expanded")
                    OutputStyle = OutputStyle.Expanded;
                else if (style == "compact")
                    OutputStyle = OutputStyle.Compact;
                else if (style == "compressed")
                    OutputStyle = OutputStyle.Compressed;
                else if (style == "echo")
                    OutputStyle = OutputStyle.Echo;
            }

            IncludeSourceComments = GetValue(config, "includeSourceComments") == "true";

            int precision = 5;
            if (int.TryParse(GetValue(config, "precision"), out precision))
                Precision = precision;
        }

        /// <summary>
        /// Type of output style
        /// </summary>
        public OutputStyle OutputStyle { get; set; } = OutputStyle.Nested;

        /// <summary>
        /// Flag indicating if comments should be included in the compiled version.
        /// </summary>
        public bool IncludeSourceComments { get; set; }

        /// <summary>
        /// Precision
        /// </summary>
        public int Precision { get; set; } = 5;
    }
}
