using System;
using NUglify;
using NUglify.JavaScript;

namespace WebCompiler
{
    /// <summary>
    /// Handle all options for JavaScript Minification
    /// </summary>
    public class JavaScriptOptions : BaseMinifyOptions
    {
        /// <summary>
        /// Get settings for the minification
        /// </summary>
        /// <param name="config"></param>
        /// <returns>CodeSettings based on the config file.</returns>
        public static CodeSettings GetSettings(Config config)
        {
            LoadDefaultSettings(config, "javascript");

            CodeSettings settings = new CodeSettings();

            settings.PreserveImportantComments = GetValue(config, "preserveImportantComments", true) == "True";
            settings.TermSemicolons = GetValue(config, "termSemicolons", true) == "True";

            if (GetValue(config, "renameLocals", true) == "False")
                settings.LocalRenaming = LocalRenaming.KeepAll;

            string evalTreatment = GetValue(config, "evalTreatment", "ignore");

            if (evalTreatment.Equals("ignore", StringComparison.OrdinalIgnoreCase))
                settings.EvalTreatment = EvalTreatment.Ignore;
            else if (evalTreatment.Equals("makeAllSafe", StringComparison.OrdinalIgnoreCase))
                settings.EvalTreatment = EvalTreatment.MakeAllSafe;
            else if (evalTreatment.Equals("makeImmediateSafe", StringComparison.OrdinalIgnoreCase))
                settings.EvalTreatment = EvalTreatment.MakeImmediateSafe;

            string outputMode = GetValue(config, "outputMode", "singleLine");

            if (outputMode.Equals("multipleLines", StringComparison.OrdinalIgnoreCase))
                settings.OutputMode = OutputMode.MultipleLines;
            else if (outputMode.Equals("singleLine", StringComparison.OrdinalIgnoreCase))
                settings.OutputMode = OutputMode.SingleLine;
            else if (outputMode.Equals("none", StringComparison.OrdinalIgnoreCase))
                settings.OutputMode = OutputMode.None;

            string indentSize = GetValue(config, "indentSize", 2);
            int size;
            if (int.TryParse(indentSize, out size))
                settings.IndentSize = size;

            return settings;
        }
    }
}
