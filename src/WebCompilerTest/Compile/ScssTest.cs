using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebCompiler;

namespace WebCompilerTest
{
    [TestClass]
    public class ScssTest
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
            File.Delete("../../artifacts/scss/test.css");
            File.Delete("../../artifacts/scss/test.min.css");
            File.Delete("../../artifacts/scss/test.css.map");
        }

        [TestMethod, TestCategory("SCSS")]
        public void CompileScss()
        {
            var result = _processor.Process("../../artifacts/scssconfig.json");
            var first = result.First();
            Assert.IsTrue(File.Exists("../../artifacts/scss/test.css"));
            //Assert.IsTrue(first.CompiledContent.Contains("/*# sourceMappingURL=test.css.map */"));
        }

        [TestMethod, TestCategory("SCSS")]
        public void CompileScssError()
        {
            var result = _processor.Process("../../artifacts/scssconfigError.json");
            Assert.IsTrue(result.Count() == 1);
            Assert.IsTrue(result.ElementAt(0).HasErrors);
        }

        [TestMethod, TestCategory("SCSS")]
        public void AssociateExtensionSourceFileChangedTest()
        {
            var result = _processor.SourceFileChanged("../../artifacts/scssconfig.json", "scss/test.scss");
            Assert.AreEqual(1, result.Count<CompilerResult>());
        }

        [TestMethod, TestCategory("SCSS")]
        public void OtherExtensionTypeSourceFileChangedTest()
        {
            var result = _processor.SourceFileChanged("../../artifacts/scssconfig.json", "scss/filewithinvalidextension.less");
            Assert.AreEqual(0, result.Count<CompilerResult>());
        }
    }
}
