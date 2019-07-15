using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebCompiler;

namespace WebCompilerTest
{
    [TestClass]
    public class LessOptionsTest
    {
        [TestMethod, TestCategory("LessOptions")]
        public void RelativeUrls()
        {
            var configs = ConfigHandler.GetConfigs("../../artifacts/lessconfig.json");
            var result =  LessOptions.FromConfig(configs.First());
            Assert.AreEqual(true, result.RelativeUrls);
        }

        [TestMethod, TestCategory("LessOptions")]
        public void RootPath()
        {
            var configs = ConfigHandler.GetConfigs("../../artifacts/lessconfig.json");
            var result = LessOptions.FromConfig(configs.First());
            Assert.AreEqual("./", result.RootPath);
        }

        [TestMethod, TestCategory("LessOptions")]
        public void StrictMath()
        {
            var configs = ConfigHandler.GetConfigs("../../artifacts/lessconfig.json");
            var result = LessOptions.FromConfig(configs.First());
            Assert.AreEqual("strict", result.Math);
        }
    }
}
