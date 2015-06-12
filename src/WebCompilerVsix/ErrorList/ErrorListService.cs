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
                if (results == null)
                {
                    MessageBox.Show($"There is an error in the {Constants.CONFIG_FILENAME} file. This could be due to a change in the format after this extension was updated.", Constants.VSIX_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (File.Exists(configFile))
                        WebCompilerPackage._dte.ItemOperations.OpenFile(configFile);

                    return;
                }

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
