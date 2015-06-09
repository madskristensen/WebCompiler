using Microsoft.Ajax.Utilities;

namespace WebCompiler
{
    class JavaScriptOptions
    {
        public JavaScriptOptions(Config config)
        {
            PreserveImportantComments = GetValue(config, "preserveImportantComments") == "true";
            TermSemicolons = GetValue(config, "termSemicolons") == "true";

            string evalTreatment = GetValue(config, "evanTreatment");

            if (evalTreatment == "ignore")
                EvalTreatment = EvalTreatment.Ignore;
            else if (evalTreatment == "makeAllSafe")
                EvalTreatment = EvalTreatment.MakeAllSafe;
            else if (evalTreatment == "makeImmediateSafe")
                EvalTreatment = EvalTreatment.MakeImmediateSafe;
        }

        internal static string GetValue(Config config, string key)
        {
            if (config.Options.ContainsKey(key))
                return config.Options[key];

            return string.Empty;
        }

        public bool PreserveImportantComments { get; set; }

        public bool TermSemicolons { get; set; }

        public EvalTreatment EvalTreatment { get; set; }
    }
}
