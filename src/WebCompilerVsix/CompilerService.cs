using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using EnvDTE;
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
                    _processor.AfterProcess += AfterProcess;
                    _processor.BeforeProcess += (s, e) => { ProjectHelpers.CheckFileOutOfSourceControl(e.Config.GetAbsoluteOutputFile()); };
                    _processor.AfterWritingSourceMap += AfterWritingSourceMap;
                    _processor.BeforeWritingSourceMap += (s, e) => { ProjectHelpers.CheckFileOutOfSourceControl(e.ResultFile); };

                    FileMinifier.BeforeWritingMinFile += (s, e) => { ProjectHelpers.CheckFileOutOfSourceControl(e.ResultFile); };
                    FileMinifier.AfterWritingMinFile += (s, e) => { ProjectHelpers.AddNestedFile(e.OriginalFile, e.ResultFile); };

                    FileMinifier.BeforeWritingGzipFile += (s, e) => { ProjectHelpers.CheckFileOutOfSourceControl(e.ResultFile); };
                    FileMinifier.AfterWritingGzipFile += (s, e) => { ProjectHelpers.AddNestedFile(e.OriginalFile, e.ResultFile); };

                    WebCompiler.CompilerService.Initializing += (s, e) => { _dte.StatusBar.Text = "Installing updated versions of the compilers..."; _dte.StatusBar.Animate(true, vsStatusAnimation.vsStatusAnimationGeneral); };
                    WebCompiler.CompilerService.Initialized += (s, e) => { _dte.StatusBar.Text = "Done installing the compiler"; _dte.StatusBar.Animate(false, vsStatusAnimation.vsStatusAnimationGeneral); };
                }

                return _processor;
            }
        }

        public static void Process(string configFile)
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                try
                {
                    var result = Processor.Process(configFile);
                    ErrorListService.ProcessCompilerResults(result, configFile);
                }
                catch (FileNotFoundException ex)
                {
                    Logger.Log($"{Constants.VSIX_NAME} could not find {ex.FileName}");
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                    ShowError(configFile);
                }
            });
        }

        public static void SourceFileChanged(string configFile, string sourceFile)
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                try
                {
                    var result = Processor.SourceFileChanged(configFile, sourceFile);
                    ErrorListService.ProcessCompilerResults(result, configFile);
                }
                catch (FileNotFoundException ex)
                {
                    Logger.Log($"{Constants.VSIX_NAME} could not find {ex.FileName}");
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
            MessageBox.Show($"There is an error in the {Constants.CONFIG_FILENAME} file. This could be due to a change in the format after this extension was updated.", Constants.VSIX_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (File.Exists(configFile))
                WebCompilerPackage._dte.ItemOperations.OpenFile(configFile);
        }

        private static void AfterProcess(object sender, CompileFileEventArgs e)
        {
            if (!e.Config.IncludeInProject)
                return;

            var item = _dte.Solution.FindProjectItem(e.Config.FileName);

            if (item == null || item.ContainingProject == null)
                return;

            string input = e.Config.GetAbsoluteInputFile();
            string output = e.Config.GetAbsoluteOutputFile();

            string inputWithOutputExtension = Path.ChangeExtension(input, Path.GetExtension(output));

            if (inputWithOutputExtension.Equals(output, StringComparison.OrdinalIgnoreCase))
            {
                var inputItem = _dte.Solution.FindProjectItem(input);

                if (inputItem != null)
                    ProjectHelpers.AddNestedFile(input, output);
            }
            else
            {
                item.ContainingProject.AddFileToProject(e.Config.GetAbsoluteOutputFile());
            }
        }

        private static void AfterWritingSourceMap(object sender, SourceMapEventArgs e)
        {
            var item = _dte.Solution.FindProjectItem(e.OriginalFile);

            if (item == null || item.ContainingProject == null)
                return;

            ProjectHelpers.AddNestedFile(e.OriginalFile, e.ResultFile);
        }
    }
}
