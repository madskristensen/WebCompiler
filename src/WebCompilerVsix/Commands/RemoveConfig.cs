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
    internal sealed class RemoveConfig
    {
        private readonly Package _package;

        private RemoveConfig(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            _package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(GuidList.guidCompilerCmdSet, PackageCommands.RemoveConfig);
                var menuItem = new OleMenuCommand(AddConfig, menuCommandID);
                menuItem.BeforeQueryStatus += BeforeQueryStatus;
                commandService.AddCommand(menuItem);
            }
        }

        private IEnumerable<Config> _configs;

        private void BeforeQueryStatus(object sender, EventArgs e)
        {
            var button = (OleMenuCommand)sender;
            var item = ProjectHelpers.GetSelectedItems().First();

            if (item == null || item.ContainingProject == null)
                return;

            var sourceFile = item.Properties.Item("FullPath").Value.ToString();

            if (!WebCompiler.CompilerService.IsSupported(sourceFile))
                return;

            string configFile = FileHelpers.GetConfigFile(item.ContainingProject);

            _configs = ConfigFileProcessor.IsFileConfigured(configFile, sourceFile);

            button.Visible = _configs != null && _configs.Any();
        }

        public static RemoveConfig Instance
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
            Instance = new RemoveConfig(package);
        }

        private void AddConfig(object sender, EventArgs e)
        {
            var question = MessageBox.Show($"This will remove the file from {FileHelpers.FILENAME}.\r\rDo you want to continue?", "WebCompiler", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if (question == DialogResult.Cancel)
                return;

            ConfigHandler handler = new ConfigHandler();

            try
            {
                foreach (Config config in _configs)
                {
                    handler.RemoveConfig(config);
                }
            }
            catch
            {
                WebCompilerPackage._dte.StatusBar.Text = $"Could not update {FileHelpers.FILENAME}. Make sure it's not write-protected or has syntax errors.";
            }
        }
    }
}
