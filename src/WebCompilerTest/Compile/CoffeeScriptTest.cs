using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebCompiler;

namespace WebCompilerTest
{
    [TestClass]
    public class CoffeeScriptTest
    {
        private ConfigFileProcessor _processor;

        [TestInitialize]
        public void Setup()
        {
            _processor = new ConfigFileProcessor();
        }

        [TestCleanup]
        public void Cleanup()
        {
            File.Delete("../../artifacts/coffee/test.js");
            File.Delete("../../artifacts/coffee/test.min.js");
        }

        [TestMethod, TestCategory("CoffeeScript")]
        public void CompileCoffeeScript()
        {
            var result = _processor.Process("../../artifacts/coffeeconfig.json");
            FileInfo info = new FileInfo("../../artifacts/coffee/test.js");
            Assert.IsTrue(info.Exists);
            Assert.IsTrue(info.Length > 5);
        }

        [TestMethod, TestCategory("CoffeeScript")]
        public void CompileCoffeeScriptWithError()
        {
            var result = _processor.Process("../../artifacts/coffeeconfigerror.json");
            var error = result.First().Errors[0];
            Assert.AreEqual(1, error.LineNumber);
            Assert.AreEqual("error: unexpected ==", error.Message);
        }
    }
}
