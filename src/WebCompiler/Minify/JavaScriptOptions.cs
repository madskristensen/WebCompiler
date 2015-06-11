using Microsoft.Ajax.Utilities;

namespace WebCompiler
{
    class JavaScriptOptions
    {
        public static CodeSettings GetSettings(Config config)
        {
            CodeSettings settings = new CodeSettings();

            settings.PreserveImportantComments = GetValue(config, "preserveImportantComments") == "True";
            settings.TermSemicolons = GetValue(config, "termSemicolons") == "True";

            string evalTreatment = GetValue(config, "evanTreatment");

            if (evalTreatment == "ignore")
                settings.EvalTreatment = EvalTreatment.Ignore;
            else if (evalTreatment == "makeAllSafe")
                settings.EvalTreatment = EvalTreatment.MakeAllSafe;
            else if (evalTreatment == "makeImmediateSafe")
                settings.EvalTreatment = EvalTreatment.MakeImmediateSafe;

            string outputMode = GetValue(config, "outputMode");

            if (outputMode == "multipleLines")
                settings.OutputMode = OutputMode.MultipleLines;
            else if (outputMode == "singleLine")
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
