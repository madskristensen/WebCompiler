using NUglify;
using NUglify.Css;

namespace WebCompiler
{
    /// <summary>
    /// Handle all options for Css Minification
    /// </summary>
    public class CssOptions : BaseMinifyOptions
    {
        /// <summary>
        /// Get settings for the minification
        /// </summary>
        /// <param name="config"></param>
        /// <returns>CssSettings based on the config file.</returns>
        public static CssSettings GetSettings(Config config)
        {
            LoadDefaultSettings(config, "css");

            CssSettings settings = new CssSettings();
            settings.TermSemicolons = GetValue(config, "termSemicolons") == "True";

            string cssComment = GetValue(config, "commentMode");

            if (cssComment == "hacks")
                settings.CommentMode = CssComment.Hacks;
            else if (cssComment == "important")
                settings.CommentMode = CssComment.Important;
            else if (cssComment == "none")
                settings.CommentMode = CssComment.None;
            else if (cssComment == "all")
                settings.CommentMode = CssComment.All;

            string colorNames = GetValue(config, "colorNames");

            if (colorNames == "hex")
                settings.ColorNames = CssColor.Hex;
            else if (colorNames == "major")
                settings.ColorNames = CssColor.Major;
            else if (colorNames == "noSwap")
                settings.ColorNames = CssColor.NoSwap;
            else if (colorNames == "strict")
                settings.ColorNames = CssColor.Strict;

            string outputMode = GetValue(config, "outputMode", "singleLine");

            if (outputMode == "multipleLines")
                settings.OutputMode = OutputMode.MultipleLines;
            else if (outputMode == "singleLine")
                settings.OutputMode = OutputMode.SingleLine;
            else if (outputMode == "none")
                settings.OutputMode = OutputMode.None;

            string indentSize = GetValue(config, "indentSize", 2);
            int size;
            if (int.TryParse(indentSize, out size))
                settings.IndentSize = size;

            return settings;
        }
    }
}
