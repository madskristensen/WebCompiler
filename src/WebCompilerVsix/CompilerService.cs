using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using EnvDTE80;
using WebCompiler;

namespace WebCompilerVsix
{
    static class CompilerService
    {
        private static ConfigFileProcessor _processor;
        private static DTE2 _dte;

        static CompilerService()
        {
            _dte = WebCompilerPackage._dte;
        }

        private static ConfigFileProcessor Processor
        {
            get
            {
                if (_processor == null)
                {
                    _processor = new ConfigFileProcessor();
                    _processor.ConfigProcessed += ConfigProcessed;
                    _processor.BeforeProcess += (s, e) => { if (e.ContainsChanges) ProjectHelpers.CheckFileOutOfSourceControl(e.Config.GetAbsoluteOutputFile().FullName); };
                    _processor.AfterProcess += AfterProcess;
                    _processor.BeforeWritingSourceMap += (s, e) => { if (e.ContainsChanges) ProjectHelpers.CheckFileOutOfSourceControl(e.ResultFile); };
                    _processor.AfterWritingSourceMap += (s, e) => { if (e.ContainsChanges) ProjectHelpers.AddNestedFile(e.OriginalFile, e.ResultFile); };

                    FileMinifier.BeforeWritingMinFile += (s, e) => { if (e.ContainsChanges) ProjectHelpers.CheckFileOutOfSourceControl(e.ResultFile); };
                    FileMinifier.AfterWritingMinFile += (s, e) => { if (e.ContainsChanges) ProjectHelpers.AddNestedFile(e.OriginalFile, e.ResultFile); };

                    FileMinifier.BeforeWritingGzipFile += (s, e) => { if (e.ContainsChanges) ProjectHelpers.CheckFileOutOfSourceControl(e.ResultFile); };
                    FileMinifier.AfterWritingGzipFile += (s, e) => { if (e.ContainsChanges) ProjectHelpers.AddNestedFile(e.OriginalFile, e.ResultFile); };
                }

                return _processor;
            }
        }

        private static void ConfigProcessed(object sender, ConfigProcessedEventArgs e)
        {
            if (e.AmountProcessed > 0)
                _dte.StatusBar.Progress(true, $"Compiling \"{e.Config.InputFile}\"", e.AmountProcessed, e.Total);
            else
                _dte.StatusBar.Progress(true, "Compiling...", e.AmountProcessed, e.Total);
        }

        public static void Process(string configFile, IEnumerable<Config> configs = null, bool force = false)
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                try
                {
                    var result = Processor.Process(configFile, configs, force);
                    ErrorListService.ProcessCompilerResults(result);

                    if (!result.Any(c => c.HasErrors))
                    {
                        WebCompilerInitPackage.StatusText("Done compiling");
                    }
                }
                catch (Exception ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException)
                {
                    string message = $"{Constants.VSIX_NAME} found an error in {Constants.CONFIG_FILENAME}";
                    Logger.Log(message);
                    WebCompilerInitPackage.StatusText(message);
                    _dte.StatusBar.Progress(false);

                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                    ShowError(configFile);
                    _dte.StatusBar.Progress(false);
                    WebCompilerInitPackage.StatusText($"{Constants.VSIX_NAME} couldn't compile successfully");
                }
                finally
                {
                    _dte.StatusBar.Progress(false);
                }
            });
        }

        public static void SourceFileChanged(string configFile, string sourceFile)
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                try
                {
                    WebCompilerInitPackage.StatusText($"Compiling...");

                    var result = Processor.SourceFileChanged(configFile, sourceFile);
                    ErrorListService.ProcessCompilerResults(result);
                }
                catch (FileNotFoundException ex)
                {
                    Logger.Log($"{Constants.VSIX_NAME} could not find \"{ex.FileName}\"");
                    WebCompilerInitPackage.StatusText($"{Constants.VSIX_NAME} could not find \"{ex.FileName}\"");
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                    ShowError(configFile);
                }
            });
        }

        private static void ShowError(string configFile)
        {
            MessageBox.Show($"There is an error in the {Constants.CONFIG_FILENAME} file. This could be due to a change in the format after this extension was updated.", Constants.VSIX_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);

            if (File.Exists(configFile))
                WebCompilerPackage._dte.ItemOperations.OpenFile(configFile);
        }

        private static void AfterProcess(object sender, CompileFileEventArgs e)
        {
            if (!e.Config.IncludeInProject || !e.ContainsChanges)
                return;

            var item = _dte.Solution.FindProjectItem(e.Config.FileName);

            if (item == null || item.ContainingProject == null)
                return;

            FileInfo input = e.Config.GetAbsoluteInputFile();
            FileInfo output = e.Config.GetAbsoluteOutputFile();

            string inputWithOutputExtension = Path.ChangeExtension(input.FullName, output.Extension);

            if (output.Name.EndsWith(".es5.js"))
                inputWithOutputExtension = Path.ChangeExtension(inputWithOutputExtension, ".es5.js");

            if (inputWithOutputExtension.Equals(output.FullName, StringComparison.OrdinalIgnoreCase))
            {
                var inputItem = _dte.Solution.FindProjectItem(input.FullName);
                var outputItem = _dte.Solution.FindProjectItem(output.FullName);

                // Only add output file to project if it isn't already
                if (inputItem != null && outputItem == null)
                    ProjectHelpers.AddNestedFile(input.FullName, output.FullName);
            }
            else
            {
                item.ContainingProject.AddFileToProject(e.Config.GetAbsoluteOutputFile().FullName);
            }
        }
    }
}
