using Microsoft.Ajax.Utilities;

namespace WebCompiler
{
    /// <summary>
    /// Handle all options for JavaScript Minification
    /// </summary>
    public class JavaScriptOptions
    {
        /// <summary>
        /// Get settings for the minification
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static CodeSettings GetSettings(Config config)
        {
            CodeSettings settings = new CodeSettings();

            settings.PreserveImportantComments = GetValue(config, "preserveImportantComments") == "True";
            settings.TermSemicolons = GetValue(config, "termSemicolons") == "True";

            string evalTreatment = GetValue(config, "evanTreatment").ToLowerInvariant();

            if (evalTreatment == "ignore")
                settings.EvalTreatment = EvalTreatment.Ignore;
            else if (evalTreatment == "makeallsafe")
                settings.EvalTreatment = EvalTreatment.MakeAllSafe;
            else if (evalTreatment == "makeimmediatesafe")
                settings.EvalTreatment = EvalTreatment.MakeImmediateSafe;

            string outputMode = GetValue(config, "outputMode").ToLowerInvariant();

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
