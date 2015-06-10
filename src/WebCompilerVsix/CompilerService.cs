using System.Threading;
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
                    _processor.BeforeProcess += (s, e) => { ProjectHelpers.CheckFileOutOfSourceControl(e.Config.OutputFile); };
                    _processor.AfterWritingSourceMap += AfterWritingSourceMap;
                    _processor.BeforeWritingSourceMap += (s, e) => { ProjectHelpers.CheckFileOutOfSourceControl(e.ResultFile); };

                    FileMinifier.BeforeWritingMinFile += (s, e) => { ProjectHelpers.CheckFileOutOfSourceControl(e.ResultFile); };
                    FileMinifier.AfterWritingMinFile += (s, e) => { ProjectHelpers.AddNestedFile(e.OriginalFile, e.ResultFile); };
                }

                return _processor;
            }
        }

        public static void Process(string conigFile)
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                var result = Processor.Process(conigFile);
                ErrorListService.ProcessCompilerResults(result, conigFile);
            });
        }

        public static void SourceFileChanged(string configFile, string sourceFile)
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                var result = Processor.SourceFileChanged(configFile, sourceFile);
                ErrorListService.ProcessCompilerResults(result, configFile);
            });
        }

        private static void AfterProcess(object sender, CompileFileEventArgs e)
        {
            if (!e.Config.IncludeInProject)
                return;

            var item = _dte.Solution.FindProjectItem(e.Config.FileName);

            if (item == null || item.ContainingProject == null)
                return;

            ProjectHelpers.AddFileToProject(item.ContainingProject, e.Config.GetAbsoluteOutputFile());
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
