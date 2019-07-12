using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebCompiler;

namespace WebCompilerTest
{
    [TestClass]
    public class LessTest
    {
        private ConfigFileProcessor _processor;

        [TestInitialize]
        public void Setup()
        {
            _processor = new ConfigFileProcessor();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            File.Delete("../../artifacts/less/test.css");
            File.Delete("../../artifacts/less/test.min.css");
            File.Delete("../../artifacts/less/error.css");
            File.Delete("../../artifacts/less/relative.css");
            File.Delete("../../artifacts/less/relative.min.css");
        }

        [TestMethod, TestCategory("LESS")]
        public void CompileLess()
        {
            var result = _processor.Process("../../artifacts/lessconfig.json");
            Assert.IsTrue(result.All(r => !r.HasErrors));
            Assert.IsTrue(File.Exists("../../artifacts/less/test.css"));
            Assert.IsTrue(File.Exists("../../artifacts/less/test.min.css"));
            Assert.IsTrue(result.ElementAt(1).CompiledContent.Contains("url(foo.png)"));
            Assert.IsTrue(result.ElementAt(1).CompiledContent.Contains("-webkit-animation"), "AutoPrefix");

            Assert.IsTrue(File.ReadAllText("../../artifacts/less/test.min.css").Contains("important comment"), "Default options");

            string sourceMap = ScssTest.DecodeSourceMap(result.ElementAt(1).CompiledContent);
            Assert.IsTrue(sourceMap.Contains("\"relative.less\""), "Source map paths");

            string compiled = result.First().CompiledContent;
            int top = compiled.IndexOf("top");
            int pos = compiled.IndexOf("position");
            Assert.IsTrue(pos < top, "CSS Comb ordering");
        }

        [TestMethod, TestCategory("LESS")]
        public void CompileLessWithError()
        {
            var result = _processor.Process("../../artifacts/lessconfigerror.json");
            Assert.IsTrue(result.Any(r => r.HasErrors));
            Assert.IsTrue(result.Count() == 1);
            Assert.IsTrue(result.ElementAt(0).HasErrors);
        }

        [TestMethod, TestCategory("LESS")]
        public void CompileLessWithParsingExceptionError()
        {
            var result = _processor.Process("../../artifacts/lessconfigParseerror.json");
            Assert.IsTrue(result.Any(r => r.HasErrors));
            Assert.IsTrue(result.Count() == 1);
            Assert.IsTrue(result.ElementAt(0).HasErrors);
            Assert.AreNotEqual(0, result.ElementAt(0).Errors.ElementAt(0).LineNumber, "LineNumber is set when engine.TransformToCss generate a ParsingException");
            Assert.AreNotEqual(0, result.ElementAt(0).Errors.ElementAt(0).ColumnNumber, "ColumnNumber is set when engine.TransformToCss generate a ParsingException");
        }

        [TestMethod, TestCategory("LESS")]
        public void CompileLessWithOptions()
        {
            var result = ConfigHandler.GetConfigs("../../artifacts/lessconfig.json");
            Assert.IsTrue(result.First().Options.Count == 2);
        }

        [TestMethod, TestCategory("LESS")]
        public void AssociateExtensionSourceFileChangedTest()
        {
            var result = _processor.SourceFileChanged("../../artifacts/lessconfig.json", "less/test.less", null);
            Assert.IsTrue(result.All(r => !r.HasErrors));
            Assert.AreEqual(2, result.Count<CompilerResult>());
        }

        [TestMethod, TestCategory("LESS")]
        public void OtherExtensionTypeSourceFileChangedTest()
        {
            var result = _processor.SourceFileChanged("../../artifacts/lessconfig.json", "scss/test.scss", null);
            Assert.IsTrue(result.All(r => !r.HasErrors));
            Assert.AreEqual(0, result.Count<CompilerResult>());
        }
    }
}
