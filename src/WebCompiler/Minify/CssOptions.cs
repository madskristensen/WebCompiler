using Microsoft.Ajax.Utilities;

namespace WebCompiler
{
    class CssOptions
    {
        public static CssSettings GetSettings(Config config)
        {
            CssSettings settings = new CssSettings();
            settings.TermSemicolons = GetValue(config, "termSemicolons") == "True";

            string cssComment = GetValue(config, "cssComment");

            if (cssComment == "hacks")
                settings.CommentMode = CssComment.Hacks;
            else if (cssComment == "important")
                settings.CommentMode = CssComment.Important;
            else if (cssComment == "none")
                settings.CommentMode = CssComment.None;

            return settings;
        }

        internal static string GetValue(Config config, string key)
        {
            if (config.Minify.ContainsKey(key))
                return config.Minify[key].ToString();

            return string.Empty;
        }
    }
}
