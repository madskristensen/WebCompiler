using Microsoft.Ajax.Utilities;

namespace WebCompiler
{
    /// <summary>
    /// Handle all options for Css Minification
    /// </summary>
    public class CssOptions
    {
        /// <summary>
        /// Get settings for the minification
        /// </summary>
        /// <param name="config"></param>
        /// <returns>CssSettings based on the config file.</returns>
        public static CssSettings GetSettings(Config config)
        {
            CssSettings settings = new CssSettings();
            settings.TermSemicolons = GetValue(config, "termSemicolons") == "true";

            string cssComment = GetValue(config, "commentMode").ToLowerInvariant();

            if (cssComment == "hacks")
                settings.CommentMode = CssComment.Hacks;
            else if (cssComment == "important")
                settings.CommentMode = CssComment.Important;
            else if (cssComment == "none")
                settings.CommentMode = CssComment.None;
            else if (cssComment == "all")
                settings.CommentMode = CssComment.All;

            string colorNames = GetValue(config, "colorNames").ToLowerInvariant();

            if (colorNames == "hex")
                settings.ColorNames = CssColor.Hex;
            else if (colorNames == "major")
                settings.ColorNames = CssColor.Major;
            else if (colorNames == "noswap")
                settings.ColorNames = CssColor.NoSwap;
            else if (colorNames == "strict")
                settings.ColorNames = CssColor.Strict;

            string outputMode = GetValue(config, "outputMode").ToLowerInvariant ();

            if (outputMode == "multiplelines")
                settings.OutputMode = OutputMode.MultipleLines;
            else if (outputMode == "singleline")
                settings.OutputMode = OutputMode.SingleLine;
            else if (outputMode == "none")
                settings.OutputMode = OutputMode.None;

            string indentSize = GetValue(config, "indentSize");
            int size;
            if (int.TryParse(indentSize, out size))
                settings.IndentSize = size;

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
