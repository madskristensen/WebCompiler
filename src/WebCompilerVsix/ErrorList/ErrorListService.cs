using System.Collections.Generic;
using System.Linq;
using WebCompiler;

namespace WebCompilerVsix
{
    class ErrorListService
    {
        public static void ProcessCompilerResults(IEnumerable<CompilerResult> results)
        {
            var errors = results.Where(r => r.HasErrors).SelectMany(r => r.Errors);
            var clean = results.Where(r => !r.HasErrors).Select(r => r.FileName);

            if (errors.Any())
            {
                TableDataSource.Instance.AddErrors(errors);
            }

            if (results.Any(r => r.HasErrors))
            {
                if (results.Any(r => r.Errors.Any(e => !e.IsWarning)))
                {
                    WebCompilerPackage._dte.StatusBar.Text = "Error compiling. See Error List for details";
                    TableDataSource.Instance.BringToFront();
                }
                else
                {
                    WebCompilerInitPackage.StatusText($"Compiled with warnings");
                }
            }
            else
            {
                WebCompilerInitPackage.StatusText($"Compiled successfully");
            }

            TableDataSource.Instance.CleanErrors(clean);
        }
    }
}
