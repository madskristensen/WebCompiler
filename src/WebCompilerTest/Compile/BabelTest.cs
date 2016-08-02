using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebCompiler;

namespace WebCompilerTest
{
    [TestClass]
    public class BabelTest
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
            File.Delete("../../artifacts/babel/file1.js");
            File.Delete("../../artifacts/babel/file1.min.js");
        }

        [TestMethod, TestCategory("BABEL")]
        public void CompileBabel()
        {
            var result = _processor.Process("../../artifacts/babelconfig.json");
            Assert.IsTrue(File.Exists("../../artifacts/babel/file1.js"));

            string sourceMap = ScssTest.DecodeSourceMap(result.First().CompiledContent);
            Assert.IsTrue(sourceMap.Contains("/file1.jsx\""), "Source map paths");
        }

        [TestMethod, TestCategory("BABEL")]
        public void CompileBabelError()
        {
            var result = _processor.Process("../../artifacts/babelconfigError.json");
            Assert.IsTrue(result.Count() == 1);
            Assert.IsTrue(result.ElementAt(0).HasErrors);
        }

        [TestMethod, TestCategory("BABEL")]
        public void AssociateExtensionSourceFileChangedTest()
        {
            var result = _processor.SourceFileChanged("../../artifacts/babelconfig.json", "babel/file1.jsx", null);
            Assert.AreEqual(1, result.Count());
        }

        [TestMethod, TestCategory("BABEL")]
        public void OtherExtensionTypeSourceFileChangedTest()
        {
            var result = _processor.SourceFileChanged("../../artifacts/babelconfig.json", "babel/notjsx.css", null);
            Assert.AreEqual(0, result.Count<CompilerResult>());
        }
    }
}
