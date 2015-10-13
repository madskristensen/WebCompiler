using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebCompiler;

namespace WebCompilerTest
{
    [TestClass]
    public class AssemblyLoad
    {
        [AssemblyInitialize]
        public static void Initialize(TestContext context)
        {
            Telemetry.Enabled = false;
        }
    }
}
