using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using WebCompiler;

namespace WebCompilerVsix.Commands
{
    internal sealed class CleanOutputFiles
    {
        private readonly Package _package;

        private CleanOutputFiles(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            _package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(PackageGuids.guidCompilerCmdSet, PackageIds.CleanOutputFiles);
                var menuItem = new OleMenuCommand(AddConfig, menuCommandID);
                menuItem.BeforeQueryStatus += BeforeQueryStatus;
                commandService.AddCommand(menuItem);
            }
        }

        private void BeforeQueryStatus(object sender, EventArgs e)
        {
            var button = (OleMenuCommand)sender;
            var items = ProjectHelpers.GetSelectedItems();

            button.Visible = false;

            if (items.Count() != 1)
                return;

            var item = items.FirstOrDefault();

            button.Visible = item.IsConfigFile();
        }

        public static CleanOutputFiles Instance
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
            Instance = new CleanOutputFiles(package);
        }

        private void AddConfig(object sender, EventArgs e)
        {
            var question = MessageBox.Show($"This will delete all output files from the project.\r\rDo you want to continue?", Constants.VSIX_NAME, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if (question == DialogResult.No)
                return;

            var item = ProjectHelpers.GetSelectedItems().FirstOrDefault();

            if (item == null || item.Properties == null)
                return;

            string configFile = item.Properties.Item("FullPath").Value.ToString();

            var configs = ConfigHandler.GetConfigs(configFile);

            foreach (Config config in configs)
            {
                string outputFile = config.GetAbsoluteOutputFile().FullName;
                ProjectHelpers.DeleteFileFromProject(outputFile);
            }
        }
    }
}
