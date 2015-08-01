using System;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Shell;

namespace WebCompilerVsix.Commands
{
    internal sealed class Recompile
    {
        private readonly Package _package;

        private Recompile(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            _package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(GuidList.guidCompilerCmdSet, PackageCommands.RecompileConfigFile);
                var menuItem = new OleMenuCommand(UpdateSelectedConfig, menuCommandID);
                menuItem.BeforeQueryStatus += BeforeQueryStatus;
                commandService.AddCommand(menuItem);
            }
        }

        private void BeforeQueryStatus(object sender, EventArgs e)
        {
            var button = (OleMenuCommand)sender;
            var files = ProjectHelpers.GetSelectedItemPaths();
            button.Visible = false;

            int count = files.Count();

            if (count == 0) // Project
            {
                var project = ProjectHelpers.GetActiveProject();

                if (project == null)
                    return;

                string config = project.GetConfigFile();

                if (!string.IsNullOrEmpty(config) && File.Exists(config))
                    button.Visible = true;
            }
            else // config file
            {
                button.Visible = count == 1 && Path.GetFileName(files.FirstOrDefault() ?? "") == Constants.CONFIG_FILENAME;
            }
        }

        public static Recompile Instance
        {
            get;
            private set;
        }

        private IServiceProvider ServiceProvider
        {
            get
            {
                return _package;
            }
        }

        public static void Initialize(Package package)
        {
            Instance = new Recompile(package);
        }

        private void UpdateSelectedConfig(object sender, EventArgs e)
        {
            var file = ProjectHelpers.GetSelectedItemPaths().FirstOrDefault();

            if (string.IsNullOrEmpty(file)) // Project
            {
                var project = ProjectHelpers.GetActiveProject();

                if (project != null)
                    file = project.GetConfigFile();
            }

            if (!string.IsNullOrEmpty(file))
                CompilerService.Process(file);
        }
    }
}
