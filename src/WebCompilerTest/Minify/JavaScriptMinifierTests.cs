using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebCompiler;

namespace WebCompilerTest.Minify
{
    [TestClass]
    public class JavaScriptMinifierTests
    {
        private const string processingConfigFile = "../../Minify/artifacts/javascript/";

        [TestCleanup]
        public void Cleanup()
        {
            File.Delete("../../Minify/artifacts/javascript/site.js");
            File.Delete("../../Minify/artifacts/javascript/site.min.js");
        }

        /// <summary>
        /// Tests that '.min' is automatically appended to a minified file name.
        /// </summary>
        /// <remarks>
        /// For example, the file 'site.js' becomes 'site.min.js' when minified.
        /// </remarks>
        [TestMethod, TestCategory("JavaScriptMinifier")]
        public void ShouldAppendMinToOutputFile()
        {
            var configPath = Path.Combine(processingConfigFile, "outputfilenomin.json");
            var configs = ConfigHandler.GetConfigs(configPath);
            var outputFile = "site.min.js";

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
        /// For example, the file 'site.min.js' remains unchanged when minified
        /// as it already contains the '.min' suffix.
        /// </remarks>
        [TestMethod, TestCategory("JavaScriptMinifier")]
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
