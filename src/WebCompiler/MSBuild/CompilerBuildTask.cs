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

            if (!configFile.Exists)
            {
                Log.LogWarning(configFile.FullName + " does not exist");
                return true;
            }

            Log.LogMessage(MessageImportance.High, Environment.NewLine + "WebCompiler: Begin compiling " + configFile.Name);

            ConfigFileProcessor processor = new ConfigFileProcessor();
            processor.BeforeProcess += (s, e) => { if (e.ContainsChanges) FileHelpers.RemoveReadonlyFlagFromFile(e.Config.GetAbsoluteOutputFile()); };
            processor.AfterProcess += Processor_AfterProcess;
            processor.BeforeWritingSourceMap += (s, e) => { FileHelpers.RemoveReadonlyFlagFromFile(e.ResultFile); };
            processor.AfterWritingSourceMap += Processor_AfterWritingSourceMap;

            FileMinifier.BeforeWritingMinFile += (s, e) => { if (e.ContainsChanges) FileHelpers.RemoveReadonlyFlagFromFile(e.ResultFile); };
            FileMinifier.AfterWritingMinFile += FileMinifier_AfterWritingMinFile;
            FileMinifier.BeforeWritingGzipFile += (s, e) => { if (e.ContainsChanges) FileHelpers.RemoveReadonlyFlagFromFile(e.ResultFile); };
            FileMinifier.AfterWritingGzipFile += FileMinifier_AfterWritingGzipFile;

            CompilerService.Initializing += (s, e) => { Log.LogMessage(MessageImportance.High, "WebCompiler installing updated versions of the compilers..."); };

            try
            {
                var results = processor.Process(configFile.FullName);
                bool isSuccessful = true;

                foreach (CompilerResult result in results)
                {
                    if (result.HasErrors)
                    {
                        isSuccessful = false;

                        foreach (var error in result.Errors)
                        {
                            Log.LogError("WebCompiler", "0", "", error.FileName, error.LineNumber, error.ColumnNumber, error.LineNumber, error.ColumnNumber, error.Message, null);
                        }
                    }
                }

                Log.LogMessage(MessageImportance.High, "WebCompiler: Done compiling " + configFile.Name);
                return isSuccessful;
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
                return false;
            }
        }

        private void FileMinifier_AfterWritingGzipFile(object sender, MinifyFileEventArgs e)
        {
            Log.LogMessage(MessageImportance.High, "\tGzipped  " + FileHelpers.MakeRelative(FileName, e.ResultFile));
        }

        private void Processor_AfterProcess(object sender, CompileFileEventArgs e)
        {
            Log.LogMessage(MessageImportance.High, "\tCompiled " + e.Config.OutputFile);
        }

        private void Processor_AfterWritingSourceMap(object sender, SourceMapEventArgs e)
        {
            Log.LogMessage(MessageImportance.High, "\tSourceMap " + FileHelpers.MakeRelative(FileName, e.ResultFile));
        }

        private void FileMinifier_AfterWritingMinFile(object sender, MinifyFileEventArgs e)
        {
            Log.LogMessage(MessageImportance.High, "\tMinified " + FileHelpers.MakeRelative(FileName, e.ResultFile));
        }
    }
}
