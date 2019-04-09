using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUglify;
using NUglify.Css;
using WebCompiler;

namespace WebCompilerTest.Minify
{
    [TestClass]
    public class CssOptionsTests
    {
        private const string processingConfigFile = "../../Minify/artifacts/css/";

        [TestMethod, TestCategory("CssOptions")]
        public void CssCommentInUpperCaseShouldWork()
        {
            var configFile = Path.Combine(processingConfigFile, "csscommenthacksuppercase.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = CssOptions.GetSettings(configs.ElementAt(0));
            Assert.AreEqual(CssComment.Hacks, cfg.CommentMode);
        }

        [TestMethod, TestCategory("CssOptions")]
        public void CssCommentHacks()
        {
            var configFile = Path.Combine(processingConfigFile, "csscommenthacks.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = CssOptions.GetSettings(configs.ElementAt(0));
            Assert.AreEqual(CssComment.Hacks, cfg.CommentMode);
        }

        [TestMethod, TestCategory("CssOptions")]
        public void CssCommentImportant()
        {
            var configFile = Path.Combine(processingConfigFile, "csscommentimportant.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = CssOptions.GetSettings(configs.ElementAt(0));
            Assert.AreEqual(CssComment.Important, cfg.CommentMode);
        }

        [TestMethod, TestCategory("CssOptions")]
        public void CssCommentNone()
        {
            var configFile = Path.Combine(processingConfigFile, "csscommentnone.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = CssOptions.GetSettings(configs.ElementAt(0));
            Assert.AreEqual(CssComment.None, cfg.CommentMode);
        }

        [TestMethod, TestCategory("CssOptions")]
        public void CssCommentAll()
        {
            var configFile = Path.Combine(processingConfigFile, "csscommentall.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = CssOptions.GetSettings(configs.ElementAt(0));
            Assert.AreEqual(CssComment.All, cfg.CommentMode);
        }

        [TestMethod, TestCategory("CssOptions")]
        public void ColorNamesInUpperCaseShouldWork()
        {
            var configFile = Path.Combine(processingConfigFile, "colornamesmajoruppercase.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = CssOptions.GetSettings(configs.ElementAt(0));
            Assert.AreEqual(CssColor.Major, cfg.ColorNames);
        }

        [TestMethod, TestCategory("CssOptions")]
        public void ColorNamesHex()
        {
            var configFile = Path.Combine(processingConfigFile, "colornameshex.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = CssOptions.GetSettings(configs.ElementAt(0));
            Assert.AreEqual(CssColor.Hex, cfg.ColorNames);
        }

        [TestMethod, TestCategory("CssOptions")]
        public void ColorNamesMajor()
        {
            var configFile = Path.Combine(processingConfigFile, "colornamesmajor.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = CssOptions.GetSettings(configs.ElementAt(0));
            Assert.AreEqual(CssColor.Major, cfg.ColorNames);
        }

        [TestMethod, TestCategory("CssOptions")]
        public void ColorNamesNoSwap()
        {
            var configFile = Path.Combine(processingConfigFile, "colornamesnoswap.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = CssOptions.GetSettings(configs.ElementAt(0));
            Assert.AreEqual(CssColor.NoSwap, cfg.ColorNames);
        }

        [TestMethod, TestCategory("CssOptions")]
        public void ColorNamesStrict()
        {
            var configFile = Path.Combine(processingConfigFile, "colornamesstrict.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = CssOptions.GetSettings(configs.ElementAt(0));
            Assert.AreEqual(CssColor.Strict, cfg.ColorNames);
        }

        [TestMethod, TestCategory("CssOptions")]
        public void OutputModeInUpperCaseShouldWork()
        {
            var configFile = Path.Combine(processingConfigFile, "outputmodemultiplelinesuppercase.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = CssOptions.GetSettings(configs.ElementAt(0));
            Assert.AreEqual(OutputMode.MultipleLines, cfg.OutputMode);
        }

        [TestMethod, TestCategory("CssOptions")]
        public void OutputModeAllInLowerCaseShouldWork()
        {
            var configFile = Path.Combine(processingConfigFile, "outputmodemultiplelineslowercase.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = CssOptions.GetSettings(configs.ElementAt(0));
            Assert.AreEqual(OutputMode.MultipleLines, cfg.OutputMode);
        }

        [TestMethod, TestCategory("CssOptions")]
        public void OutputModeMultipleLines()
        {
            var configFile = Path.Combine(processingConfigFile, "outputmodemultiplelines.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = CssOptions.GetSettings(configs.ElementAt(0));
            Assert.AreEqual(OutputMode.MultipleLines, cfg.OutputMode);
        }

        [TestMethod, TestCategory("CssOptions")]
        public void OutputModeNone()
        {
            var configFile = Path.Combine(processingConfigFile, "outputmodenone.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = CssOptions.GetSettings(configs.ElementAt(0));
            Assert.AreEqual(OutputMode.None, cfg.OutputMode);
        }

        [TestMethod, TestCategory("CssOptions")]
        public void OutputModeSingleLine()
        {
            var configFile = Path.Combine(processingConfigFile, "outputmodesingleline.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = CssOptions.GetSettings(configs.ElementAt(0));
            Assert.AreEqual(OutputMode.SingleLine, cfg.OutputMode);
        }

        [TestMethod, TestCategory("CssOptions")]
        public void IndendSize()
        {
            var configFile = Path.Combine(processingConfigFile, "indentsize.json");
            var configs = ConfigHandler.GetConfigs(configFile);
            var cfg = CssOptions.GetSettings(configs.ElementAt(0));
            Assert.AreEqual(8, cfg.IndentSize);
        }
    }
}
