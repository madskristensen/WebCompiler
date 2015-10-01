using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebCompiler;

namespace WebCompilerTest
{
    [TestClass]
    public class ScssOptionsTest
    {
        [TestInitialize]
        public void Setup()
        {
            Telemetry.Enabled = false;
        }

        [TestMethod, TestCategory("SCSSOptions")]
        public void OutputStyleNested()
        {
            var configs = ConfigHandler.GetConfigs("../../artifacts/options/scss/scssconfignested.json");
            var result =  WebCompiler.SassOptions.FromConfig(configs.ElementAt(0));
            Assert.AreEqual("nested", result.OutputStyle);
        }

        [TestMethod, TestCategory("SCSSOptions")]
        public void OutputStyleExpanded()
        {
            var configs = ConfigHandler.GetConfigs("../../artifacts/options/scss/scssconfigexpanded.json");
            var result = WebCompiler.SassOptions.FromConfig(configs.ElementAt(0));
            Assert.AreEqual("expanded", result.OutputStyle);
        }

        [TestMethod, TestCategory("SCSSOptions")]
        public void OutputStyleCompact()
        {
            var configs = ConfigHandler.GetConfigs("../../artifacts/options/scss/scssconfigcompact.json");
            var result = WebCompiler.SassOptions.FromConfig(configs.ElementAt(0));
            Assert.AreEqual("compact", result.OutputStyle);
        }

        [TestMethod, TestCategory("SCSSOptions")]
        public void OutputStyleCompressed()
        {
            var configs = ConfigHandler.GetConfigs("../../artifacts/options/scss/scssconfigcompressed.json");
            var result = WebCompiler.SassOptions.FromConfig(configs.ElementAt(0));
            Assert.AreEqual("compressed", result.OutputStyle);
        }

        [TestMethod, TestCategory("SCSSOptions")]
        public void Precision()
        {
            var configs = ConfigHandler.GetConfigs("../../artifacts/options/scss/scssconfigprecision.json");
            var result = WebCompiler.SassOptions.FromConfig(configs.ElementAt(0));
            Assert.AreEqual(3, result.Precision);
        }
    }
}
