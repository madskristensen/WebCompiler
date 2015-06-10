using Microsoft.Ajax.Utilities;

namespace WebCompiler
{
    class JavaScriptOptions
    {
        public static CodeSettings GetSettings(Config config)
        {
            CodeSettings settings = new CodeSettings();

            settings.PreserveImportantComments = GetValue(config, "preserveImportantComments") == "true";
            settings.TermSemicolons = GetValue(config, "termSemicolons") == "true";

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
            if (config.Options.ContainsKey(key))
                return config.Options[key];

            return string.Empty;
        }
    }
}
