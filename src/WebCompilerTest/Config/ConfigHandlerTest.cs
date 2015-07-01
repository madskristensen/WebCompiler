using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using WebCompiler;

namespace WebCompilerTest.Config
{
    [TestClass]
    public class ConfigHandlerTest
    {
        private ConfigHandler _handler;

        private const string originalConfigFile = "../../artifacts/config/originalcoffeeconfig.json";
        private const string processingConfigFile = "../../artifacts/config/coffeeconfig.json";

        [TestInitialize]
        public void Setup()
        {
            _handler = new ConfigHandler();

            File.Copy(originalConfigFile, processingConfigFile, true);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(processingConfigFile))
                File.Delete(processingConfigFile);
        }

        [TestMethod, TestCategory("Config")]
        public void AddConfig()
        {
            var newConfig = new WebCompiler.Config();
            const string newInputFileName = "newInputFile";
            newConfig.InputFile = newInputFileName;

            _handler.AddConfig(processingConfigFile, newConfig);

            var configs = ConfigHandler.GetConfigs(processingConfigFile);
            Assert.AreEqual(2, configs.Count());
            Assert.AreEqual(newInputFileName, configs.ElementAt(1).InputFile);
        }

        [TestMethod, TestCategory("Config")]
        public void NonExistingConfigFileShouldReturnEmptyList()
        {
            var expectedResult = Enumerable.Empty<WebCompiler.Config>();

            var result = ConfigHandler.GetConfigs("../NonExistingFile.config");

            Assert.AreEqual(expectedResult, result);
        }
    }
}
