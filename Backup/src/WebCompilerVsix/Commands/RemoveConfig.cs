﻿using System;
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
                throw new ArgumentNullException(nameof(package));
            }

            _package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(PackageGuids.guidCompilerCmdSet, PackageIds.RemoveConfig);
                var menuItem = new OleMenuCommand(AddConfig, menuCommandID);
                menuItem.BeforeQueryStatus += BeforeQueryStatus;
                commandService.AddCommand(menuItem);
            }
        }

        private IEnumerable<Config> _configs;

        private void BeforeQueryStatus(object sender, EventArgs e)
        {
            var button = (OleMenuCommand)sender;
            var items = ProjectHelpers.GetSelectedItems();

            button.Visible = false;

            if (items.Count() != 1)
                return;

            var item = items.FirstOrDefault();

            if (item == null || item.ContainingProject == null || item.Properties == null)
                return;

            var sourceFile = item.Properties.Item("FullPath").Value.ToString();

            if (!WebCompiler.CompilerService.IsSupported(sourceFile))
                return;

            string configFile = item.ContainingProject.GetConfigFile();

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
            var question = MessageBox.Show($"This will remove the file from {Constants.CONFIG_FILENAME}.\r\rDo you want to continue?", Constants.VSIX_NAME, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

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
            catch (Exception ex)
            {
                Logger.Log(ex);
                WebCompilerPackage._dte.StatusBar.Text = $"Could not update {Constants.CONFIG_FILENAME}. Make sure it's not write-protected or has syntax errors.";
            }
        }
    }
}
