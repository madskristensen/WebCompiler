using Microsoft.Ajax.Utilities;

namespace WebCompiler
{
    class CssOptions
    {
        public CssOptions(Config config)
        {
            TermSemicolons = GetValue(config, "termSemicolons") == "true";

            string cssComment = GetValue(config, "cssComment");

            if (cssComment == "hacks")
                CssComment = CssComment.Hacks;
            else if (cssComment == "important")
                CssComment = CssComment.Important;
            else if (cssComment == "none")
                CssComment = CssComment.None;
        }

        internal static string GetValue(Config config, string key)
        {
            if (config.Options.ContainsKey(key))
                return config.Options[key];

            return string.Empty;
        }

        public CssComment CssComment { get; set; }

        public bool TermSemicolons { get; set; }
    }
}
