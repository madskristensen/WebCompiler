using LibSassNet;

namespace WebCompiler
{
    class SassOptions
    {
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
                    OutputStyle = OutputStyle.Compact;
                else if (style == "echo")
                    OutputStyle = OutputStyle.Echo;
            }

            IncludeSourceComments = LessOptions.GetValue(config, "includeSourceComments") == "true";

            int precision = 5;
            if (int.TryParse(LessOptions.GetValue(config, "precision"), out precision))
                Precision = precision;
        }

        public OutputStyle OutputStyle { get; set; } = OutputStyle.Nested;

        public bool IncludeSourceComments { get; set; }

        public int Precision { get; set; } = 5;
    }
}
