using System;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace WebCompiler
{
    /// <summary>
    /// An MSBuild task for cleaning web compiler output of a given config file.
    /// </summary>
    public class CompilerCleanTask : Task
    {
        /// <summary>
        /// The file path of the compilerconfig.json file
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Execute the Clean Task
        /// </summary>
        public override bool Execute()
        {
            var configFile = new FileInfo(FileName);

            if (!configFile.Exists)
            {
                Log.LogWarning(configFile.FullName + " does not exist");
                return true;
            }

            Log.LogMessage(MessageImportance.High, Environment.NewLine + "WebCompiler: Begin cleaning output of " + configFile.Name);

            try
            {
                var processor = new ConfigFileProcessor();
                processor.DeleteOutputFiles(configFile.FullName);

                Log.LogMessage(MessageImportance.High, "WebCompiler: Done cleaning output of " + configFile.Name);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
                return false;
            }
        }
    }
}
