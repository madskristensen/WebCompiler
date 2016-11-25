using System;
using System.Linq;
using System.ComponentModel.Design;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System.IO;
using System.Collections.Generic;
using WebCompiler;

namespace WebCompilerVsix.Commands
{
    internal sealed class CompileAllFiles
    {
        private readonly Package _package;

        private CompileAllFiles(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            _package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var compileSolId = new CommandID(PackageGuids.guidCompilerCmdSet, PackageIds.CompileSolution);
                var compileSolMenu = new OleMenuCommand(CompileSolution, compileSolId);
                compileSolMenu.BeforeQueryStatus += SolutionQueryStatus;
                commandService.AddCommand(compileSolMenu);
            }
        }

        private void SolutionQueryStatus(object sender, EventArgs e)
        {
            var button = (OleMenuCommand)sender;

            button.Visible = ProjectHelpers.IsSolutionLoaded();
        }

        public static CompileAllFiles Instance { get; private set; }

        private IServiceProvider ServiceProvider
        {
            get { return _package; }
        }

        public static void Initialize(Package package)
        {
            Instance = new CompileAllFiles(package);
        }

        private void CompileSolution(object sender, EventArgs e)
        {
            var projects = ProjectHelpers.GetAllProjects();

            foreach (Project project in projects)
            {
                string folder = Path.GetDirectoryName(project.GetRootFolder());
                var configs = GetFiles(folder, Constants.CONFIG_FILENAME);

                foreach (string config in configs)
                {
                    if (!string.IsNullOrEmpty(config))
                        CompilerService.Process(config);
                }
            }
        }

        private static List<string> GetFiles(string path, string pattern)
        {
            var files = new List<string>();

            if (path.Contains("node_modules"))
                return files;

            try
            {
                files.AddRange(Directory.GetFiles(path, pattern, SearchOption.TopDirectoryOnly));
                foreach (var directory in Directory.GetDirectories(path))
                    files.AddRange(GetFiles(directory, pattern));
            }
            catch (UnauthorizedAccessException) { }

            return files;
        }
    }
}
