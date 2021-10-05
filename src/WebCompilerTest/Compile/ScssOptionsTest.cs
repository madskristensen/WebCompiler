using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebCompiler;

namespace WebCompilerTest
{
    [TestClass]
    public class ScssOptionsTest
    {
        [TestMethod, TestCategory("SCSSOptions")]
        public void AutoPrefix()
        {
            var configs = ConfigHandler.GetConfigs("../../artifacts/options/scss/scssconfigautoprefix.json");
            var result = WebCompiler.SassOptions.FromConfig(configs.ElementAt(0));
            Assert.AreEqual("test", result.AutoPrefix);
        }

        [TestMethod, TestCategory("SCSSOptions")]
        public void LoadPaths()
        {
            var configs = ConfigHandler.GetConfigs("../../artifacts/options/scss/scssconfigloadpaths.json");
            var result = WebCompiler.SassOptions.FromConfig(configs.ElementAt(0));
            CollectionAssert.AreEqual(new string[] { "/test/test.scss", "/test/test2.scss" }, result.LoadPaths);
        }

        [TestMethod, TestCategory("SCSSOptions")]
        public void StyleExpanded()
        {
            var configs = ConfigHandler.GetConfigs("../../artifacts/options/scss/scssconfigexpanded.json");
            var result = WebCompiler.SassOptions.FromConfig(configs.ElementAt(0));
            Assert.AreEqual(SassStyle.Expanded, result.Style);
        }

        [TestMethod, TestCategory("SCSSOptions")]
        public void StyleCompressed()
        {
            var configs = ConfigHandler.GetConfigs("../../artifacts/options/scss/scssconfigcompressed.json");
            var result = WebCompiler.SassOptions.FromConfig(configs.ElementAt(0));
            Assert.AreEqual(SassStyle.Compressed, result.Style);
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
