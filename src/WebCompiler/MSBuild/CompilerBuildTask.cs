using System;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace WebCompiler
{
    /// <summary>
    /// An MSBuild task for running web compilers on a given config file.
    /// </summary>
    public class CompilerBuildTask : Task
    {
        /// <summary>
        /// The file path of the compilerconfig.json file
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Execute the Task
        /// </summary>
        public override bool Execute()
        {
            FileInfo configFile = new FileInfo(FileName);

            Log.LogMessage(MessageImportance.High, "WebCompiler: Begin compiling " + configFile.Name);

            if (!configFile.Exists)
            {
                Log.LogWarning(configFile.FullName + " does not exist");
                return true;
            }

            ConfigFileProcessor processor = new ConfigFileProcessor();
            processor.AfterProcess += Processor_AfterProcess;
            processor.AfterWritingSourceMap += Processor_AfterWritingSourceMap;
            FileMinifier.AfterWritingMinFile += FileMinifier_AfterWritingMinFile;

            var results = processor.Process(configFile.FullName);
            bool isSuccessful = true;

            if (results == null)
            {
                Log.LogError("Syntax error in " + configFile.FullName);
                return false;
            }

            foreach (CompilerResult result in results)
            {
                if (result.HasErrors)
                {
                    isSuccessful = false;

                    foreach (var error in result.Errors)
                    {
                        Log.LogError("WebCompiler", "0", "", error.FileName, error.LineNumber, error.ColumnNumber, error.LineNumber, error.ColumnNumber, error.Message, null); ;
                    }
                }
            }

            Log.LogMessage(MessageImportance.High, "WebCompiler: Done compiling " + configFile.Name);

            return isSuccessful;
        }

        private void Processor_AfterProcess(object sender, CompileFileEventArgs e)
        {
            Log.LogMessage(MessageImportance.High, "\tCompiled " + e.Config.OutputFile);
        }

        private void Processor_AfterWritingSourceMap(object sender, SourceMapEventArgs e)
        {
            Log.LogMessage(MessageImportance.High, "\tSourceMap " + MakeRelative(FileName, e.ResultFile));
        }

        private void FileMinifier_AfterWritingMinFile(object sender, MinifyFileEventArgs e)
        {
            Log.LogMessage(MessageImportance.High, "\tMinified " + MakeRelative(FileName, e.ResultFile));
        }

        private static string MakeRelative(string baseFile, string file)
        {
            Uri baseUri = new Uri(baseFile, UriKind.RelativeOrAbsolute);
            Uri fileUri = new Uri(file, UriKind.RelativeOrAbsolute);

            return baseUri.MakeRelativeUri(fileUri).ToString();
        }
    }
}
