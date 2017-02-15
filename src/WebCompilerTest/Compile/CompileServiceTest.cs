using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebCompiler;

namespace WebCompilerTest
{
    [TestClass]
    public class CompileServiceTest
    {
        [TestMethod, TestCategory("CompileService")]
        public void LessIsSupported()
        {
            var result = CompilerService.IsSupported(".LESS");
            Assert.IsTrue(result);
        }

        [TestMethod, TestCategory("CompileService")]
        public void ScssIsSupported()
        {
            var result = CompilerService.IsSupported(".SCSS");
            Assert.IsTrue(result);
        }

        [TestMethod, TestCategory("CompileService")]
        public void CoffeeIsSupported()
        {
            var result = CompilerService.IsSupported(".COFFEE");
            Assert.IsTrue(result);
        }

        [TestMethod, TestCategory("CompileService")]
        public void HandleBarsIsSupported()
        {
            var result = CompilerService.IsSupported(".HANDLEBARS");
            Assert.IsTrue(result);

            result = CompilerService.IsSupported(".hbs");
            Assert.IsTrue(result);
        }

        [TestMethod, TestCategory("CompileService")]
        public void LowerCaseSupportedExtensionAlsoWorks()
        {
            var result = CompilerService.IsSupported(".less");
            Assert.IsTrue(result);
        }

        [TestMethod, TestCategory("CompileService")]
        public void OtherExtensionDoesntWorks()
        {
            var result = CompilerService.IsSupported(".cs");
            Assert.IsFalse(result);
        }

    }
}
