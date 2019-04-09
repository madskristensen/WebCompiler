using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUglify;
using NUglify.JavaScript;
using WebCompiler;

namespace WebCompilerTest.Minify
{
    [TestClass]
    public class JavaScriptOptionsTests
    {
        private const string processingConfigFile = "../../Minify/artifacts/javascript/";

        [TestMethod, TestCategory("JavaScriptOptions")]
        public void EvanTreatmentInUpperCaseShouldWork()
        {
            var configFile = Path.Combine(processingConfigFile, "evantreatmentmakeallsafeuppercase.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = JavaScriptOptions.GetSettings(configs.ElementAt(0));
            Assert.AreEqual(EvalTreatment.MakeAllSafe, cfg.EvalTreatment);
        }

        [TestMethod, TestCategory("JavaScriptOptions")]
        public void EvanTreatmentIgnore()
        {
            var configFile = Path.Combine(processingConfigFile, "evantreatmentignore.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = JavaScriptOptions.GetSettings(configs.ElementAt(0));
            Assert.AreEqual(EvalTreatment.Ignore, cfg.EvalTreatment);
        }

        [TestMethod, TestCategory("JavaScriptOptions")]
        public void EvanTreatmentMakeAllSafe()
        {
            var configFile = Path.Combine(processingConfigFile, "evantreatmentmakeallsafe.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = JavaScriptOptions.GetSettings(configs.ElementAt(0));
            Assert.AreEqual(EvalTreatment.MakeAllSafe, cfg.EvalTreatment);
        }

        [TestMethod, TestCategory("JavaScriptOptions")]
        public void EvanTreatmentMakeImmediateSafee()
        {
            var configFile = Path.Combine(processingConfigFile, "evantreatmentmakeimmediatesafe.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = JavaScriptOptions.GetSettings(configs.ElementAt(0));
            Assert.AreEqual(EvalTreatment.MakeImmediateSafe, cfg.EvalTreatment);
        }

        [TestMethod, TestCategory("JavaScriptOptions")]
        public void OutputModeInUpperCaseShouldWork()
        {
            var configFile = Path.Combine(processingConfigFile, "outputmodemultiplelinesuppercase.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = JavaScriptOptions.GetSettings(configs.ElementAt(0));
            Assert.AreEqual(OutputMode.MultipleLines, cfg.OutputMode);
        }

        [TestMethod, TestCategory("JavaScriptOptions")]
        public void OutputModeAllInLowerCaseShouldWork()
        {
            var configFile = Path.Combine(processingConfigFile, "outputmodemultiplelineslowercase.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = JavaScriptOptions.GetSettings(configs.ElementAt(0));
            Assert.AreEqual(OutputMode.MultipleLines, cfg.OutputMode);
        }

        [TestMethod, TestCategory("JavaScriptOptions")]
        public void OutputModeMultipleLines()
        {
            var configFile = Path.Combine(processingConfigFile, "outputmodemultiplelines.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = JavaScriptOptions.GetSettings(configs.ElementAt(0));
            Assert.AreEqual(OutputMode.MultipleLines, cfg.OutputMode);
        }

        [TestMethod, TestCategory("JavaScriptOptions")]
        public void OutputModeNone()
        {
            var configFile = Path.Combine(processingConfigFile, "outputmodenone.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = JavaScriptOptions.GetSettings(configs.ElementAt(0));
            Assert.AreEqual(OutputMode.None, cfg.OutputMode);
        }

        [TestMethod, TestCategory("JavaScriptOptions")]
        public void OutputModeSingleLine()
        {
            var configFile = Path.Combine(processingConfigFile, "outputmodesingleline.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = JavaScriptOptions.GetSettings(configs.ElementAt(0));
            Assert.AreEqual(OutputMode.SingleLine, cfg.OutputMode);
        }

        [TestMethod, TestCategory("JavaScriptOptions")]
        public void IndendSize()
        {
            var configFile = Path.Combine(processingConfigFile, "indentsize.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = JavaScriptOptions.GetSettings(configs.ElementAt(0));
            Assert.AreEqual(8, cfg.IndentSize);
        }
    }
}
