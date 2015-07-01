using System;
using System.IO;
using System.Linq;
using WebCompiler;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebCompilerTest
{
    [TestClass]
    public class ScssOptionsTest
    {
        [TestMethod, TestCategory("SCSSOptions")]
        public void OutputStyleNested()
        {
            var configs = ConfigHandler.GetConfigs("../../artifacts/options/scss/scssconfignested.json");
            var result = new WebCompiler.SassOptions(configs.ElementAt(0));
            Assert.AreEqual(OutputStyle.Nested, result.OutputStyle);
        }

        [TestMethod, TestCategory("SCSSOptions")]
        public void OutputStyleExpanded()
        {
            var configs = ConfigHandler.GetConfigs("../../artifacts/options/scss/scssconfigexpanded.json");
            var result = new WebCompiler.SassOptions(configs.ElementAt(0));
            Assert.AreEqual(OutputStyle.Expanded, result.OutputStyle);
        }

        [TestMethod, TestCategory("SCSSOptions")]
        public void OutputStyleCompact()
        {
            var configs = ConfigHandler.GetConfigs("../../artifacts/options/scss/scssconfigcompact.json");
            var result = new WebCompiler.SassOptions(configs.ElementAt(0));
            Assert.AreEqual(OutputStyle.Compact, result.OutputStyle);
        }

        [TestMethod, TestCategory("SCSSOptions")]
        public void OutputStyleCompressed()
        {
            var configs = ConfigHandler.GetConfigs("../../artifacts/options/scss/scssconfigcompressed.json");
            var result = new WebCompiler.SassOptions(configs.ElementAt(0));
            Assert.AreEqual(OutputStyle.Compressed, result.OutputStyle);
        }

        [TestMethod, TestCategory("SCSSOptions")]
        public void OutputStyleEcho()
        {
            var configs = ConfigHandler.GetConfigs("../../artifacts/options/scss/scssconfigecho.json");
            var result = new WebCompiler.SassOptions(configs.ElementAt(0));
            Assert.AreEqual(OutputStyle.Echo, result.OutputStyle);
        }

        [TestMethod, TestCategory("SCSSOptions")]
        public void Precision()
        {
            var configs = ConfigHandler.GetConfigs("../../artifacts/options/scss/scssconfigprecision.json");
            var result = new WebCompiler.SassOptions(configs.ElementAt(0));
            Assert.AreEqual(3, result.Precision);
        }

        [TestMethod, TestCategory("SCSSOptions")]
        public void IncludeSourceComments()
        {
            var configs = ConfigHandler.GetConfigs("../../artifacts/options/scss/scssconfigincludesourcecomments.json");
            var result = new WebCompiler.SassOptions(configs.ElementAt(0));
            Assert.IsTrue(result.IncludeSourceComments);
        }

        [TestMethod, TestCategory("SCSSOptions")]
        public void IncludeSourceCommentsAsString()
        {
            var configs = ConfigHandler.GetConfigs("../../artifacts/options/scss/scssconfigincludesourcecommentsasstring.json");
            var result = new WebCompiler.SassOptions(configs.ElementAt(0));
            Assert.IsTrue(result.IncludeSourceComments);
        }
    }
}
