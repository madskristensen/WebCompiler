using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Threading;
using WebCompiler;

namespace WebCompilerVsix
{
    class ErrorListService
    {
        public static void ProcessCompilerResults(IEnumerable<CompilerResult> results)
        {
            WebCompilerPackage._dispatcher.BeginInvoke(new Action(() =>
            {
                foreach (CompilerResult result in results)
                {
                    if (result.HasErrors)
                    {
                        ErrorList.AddErrors(result.FileName, result.Errors);
                    }
                    else
                    {
                        ErrorList.CleanErrors(result.FileName);
                        WebCompilerPackage._dte.StatusBar.Text = $"{Path.GetFileName(result.FileName)} compiled";
                    }
                }
            }), DispatcherPriority.ApplicationIdle, null);
        }
    }
}
