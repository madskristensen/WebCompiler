using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebCompiler;

namespace WebCompilerTest.Minify
{
    [TestClass]
    public class CssMinifierTests
    {
        private const string processingConfigFile = "../../Minify/artifacts/css/";

        [TestCleanup]
        public void Cleanup()
        {
            File.Delete("../../Minify/artifacts/css/site.css");
            File.Delete("../../Minify/artifacts/css/site.min.css");
        }

        /// <summary>
        /// Tests that '.min' is automatically appended to a minified file name.
        /// </summary>
        /// <remarks>
        /// For example, the file 'site.css' becomes 'site.min.css' when minified.
        /// </remarks>
        [TestMethod, TestCategory("CssMinifier")]
        public void ShouldAppendMinToOutputFile()
        {
            var configPath = Path.Combine(processingConfigFile, "outputfilenomin.json");
            var configs = ConfigHandler.GetConfigs(configPath);
            var outputFile = "site.min.css";

            // Capture the name of the resulting (minified) file.
            string resultFile = string.Empty;
            FileMinifier.BeforeWritingMinFile += (object sender, MinifyFileEventArgs e) => { resultFile = new FileInfo(e.ResultFile).Name; };

            ConfigFileProcessor processor = new ConfigFileProcessor();
            var results = processor.Process(configPath, configs, force:true);

            Assert.AreEqual(outputFile, resultFile);
        }

        /// <summary>
        /// Tests that '.min' is only appended to a minified file name once.
        /// </summary>
        /// <remarks>
        /// For example, the file 'site.min.css' remains unchanged when minified
        /// as it already contains the '.min' suffix.
        /// </remarks>
        [TestMethod, TestCategory("CssMinifier")]
        public void ShouldAppendMinOnlyOnce()
        {
            var configPath = Path.Combine(processingConfigFile, "outputfilemin.json");
            var configs = ConfigHandler.GetConfigs(configPath);
            var outputFile = configs.First().OutputFile;

            // Capture the name of the resulting (minified) file.
            string resultFile = string.Empty;
            FileMinifier.BeforeWritingMinFile += (object sender, MinifyFileEventArgs e) => { resultFile = new FileInfo(e.ResultFile).Name; };

            ConfigFileProcessor processor = new ConfigFileProcessor();
            var results = processor.Process(configPath, configs, force: true);

            Assert.AreEqual(outputFile, resultFile);
        }
    }
}
