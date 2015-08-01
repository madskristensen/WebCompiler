using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebCompiler;

namespace WebCompilerTest
{
    [TestClass]
    public class IcedCoffeeScriptTest
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
            File.Delete("../../artifacts/iced/test.js");
            File.Delete("../../artifacts/iced/test.min.js");
        }

        [TestMethod, TestCategory("Iced CoffeeScript")]
        public void CompileCoffeeScript()
        {
            var result = _processor.Process("../../artifacts/icedcoffeeconfig.json");
            FileInfo info = new FileInfo("../../artifacts/iced/test.js");
            Assert.IsTrue(info.Exists);
            Assert.IsTrue(info.Length > 5);
        }

        [TestMethod, TestCategory("Iced CoffeeScript")]
        public void CompileCoffeeScriptWithError()
        {
            var result = _processor.Process("../../artifacts/icedcoffeeconfigerror.json");
            var error = result.First().Errors[0];
            Assert.AreEqual(1, error.LineNumber);
            Assert.AreEqual("unexpected ==", error.Message);
        }
    }
}
