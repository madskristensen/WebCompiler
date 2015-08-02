using System;
using System.Linq;
using System.ComponentModel.Design;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace WebCompilerVsix.Commands
{
    internal sealed class CompileAllFiles
    {
        private readonly Package _package;

        private CompileAllFiles(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            _package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var compileSolId = new CommandID(GuidList.guidCompilerCmdSet, PackageCommands.CompileSolution);
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
                string config = project.GetConfigFile();

                if (!string.IsNullOrEmpty(config))
                    CompilerService.Process(config);
            }
        }
    }
}
