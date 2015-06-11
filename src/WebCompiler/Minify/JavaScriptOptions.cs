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
