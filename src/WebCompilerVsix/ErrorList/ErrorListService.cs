using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Windows.Threading;
using WebCompiler;

namespace WebCompilerVsix
{
    class ErrorListService
    {
        public static void ProcessCompilerResults(IEnumerable<CompilerResult> results, string configFile)
        {
            WebCompilerPackage._dispatcher.BeginInvoke(new Action(() =>
            {
                bool hasError = false;

                foreach (CompilerResult result in results)
                {
                    if (result.HasErrors)
                    {
                        hasError = true;
                        ErrorList.AddErrors(result.FileName, result.Errors);
                        WebCompilerPackage._dte.StatusBar.Text = $"Error compiling \"{Path.GetFileName(result.FileName)}\". See Error List for details";
                    }
                    else
                    {
                        ErrorList.CleanErrors(result.FileName);

                        if (!hasError)
                            WebCompilerPackage._dte.StatusBar.Text = $"Done compiling \"{Path.GetFileName(result.FileName)}\"";
                    }
                }
            }), DispatcherPriority.ApplicationIdle, null);
        }
    }
}
