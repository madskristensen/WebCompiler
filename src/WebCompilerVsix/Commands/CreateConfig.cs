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
    internal sealed class CreateConfig
    {
        private readonly Package _package;

        private CreateConfig(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            _package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(GuidList.guidCompilerCmdSet, PackageCommands.CreateConfigFile);
                var menuItem = new OleMenuCommand(AddConfig, menuCommandID);
                menuItem.BeforeQueryStatus += BeforeQueryStatus;
                commandService.AddCommand(menuItem);
            }
        }

        private List<Config> _reCompileConfigs = new List<Config>();

        private void BeforeQueryStatus(object sender, EventArgs e)
        {
            _reCompileConfigs.Clear();
            var button = (OleMenuCommand)sender;
            var sourceFile = ProjectHelpers.GetSelectedItemPaths().FirstOrDefault();

            if (string.IsNullOrEmpty(sourceFile))
            {
                button.Visible = false;
                return;
            }

            button.Visible = WebCompiler.CompilerService.IsSupported(sourceFile);

            if (!button.Visible)
                return;

            var item = ProjectHelpers.GetSelectedItems().ElementAt(0);

            if (item == null || item.ContainingProject == null)
                return;

            string configFile = item.ContainingProject.GetConfigFile();

            var configs = ConfigFileProcessor.IsFileConfigured(configFile, sourceFile);

            if (configs != null && configs.Any())
            {
                button.Text = "Re-compile file";
                _reCompileConfigs.AddRange(configs);
            }
            else
            {
                button.Text = "Compile file...";
            }
        }

        public static CreateConfig Instance
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
            Instance = new CreateConfig(package);
        }

        private void AddConfig(object sender, EventArgs e)
        {
            var item = ProjectHelpers.GetSelectedItems().First();

            if (item.ContainingProject == null)
                return;

            string folder = item.ContainingProject.GetRootFolder();
            string configFile = item.ContainingProject.GetConfigFile();
            string relativeFile = MakeRelative(configFile, ProjectHelpers.GetSelectedItemPaths().First());

            // Recompile if already configured
            if (_reCompileConfigs.Any())
            {
                string absoluteFile = Path.Combine(folder, relativeFile).Replace("/", "\\");
                CompilerService.SourceFileChanged(configFile, absoluteFile);
                return;
            }

            // Create new config
            WebCompilerPackage._dte.StatusBar.Progress(true, "Compiling file", 0, 3);
            string inputFile = item.Properties.Item("FullPath").Value.ToString();
            string outputFile = GetOutputFileName(inputFile, Path.GetFileName(relativeFile));

            if (string.IsNullOrEmpty(outputFile))
                return;

            string relativeOutputFile = MakeRelative(configFile, outputFile);
            Config config = CreateConfigFile(relativeFile, relativeOutputFile);

            WebCompilerPackage._dte.StatusBar.Progress(true, "Compiling file", 1, 3);

            ConfigHandler handler = new ConfigHandler();
            handler.AddConfig(configFile, config);

            item.ContainingProject.AddFileToProject(configFile, "None");
            WebCompilerPackage._dte.StatusBar.Progress(true, "Compiling file", 2, 3);

            WebCompilerPackage._dte.ItemOperations.OpenFile(configFile);
            WebCompilerPackage._dte.StatusBar.Progress(true, "Compiling file", 3, 3);

            CompilerService.Process(configFile);
            WebCompilerPackage._dte.StatusBar.Progress(false, "Compiling file");
        }

        private static Config CreateConfigFile(string inputfile, string outputFile)
        {
            return new Config
            {
                OutputFile = outputFile,
                InputFile = inputfile
            };
        }

        private static string MakeRelative(string baseFile, string file)
        {
            Uri baseUri = new Uri(baseFile, UriKind.RelativeOrAbsolute);
            Uri fileUri = new Uri(file, UriKind.RelativeOrAbsolute);

            return Uri.UnescapeDataString(baseUri.MakeRelativeUri(fileUri).ToString());
        }

        private static string GetOutputFileName(string inputFile, string fileName)
        {
            string extension = Path.GetExtension(fileName);
            string ext = "css";

            if (extension == ".coffee" || extension == ".iced")
                ext = "js";

            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.InitialDirectory = Path.GetDirectoryName(inputFile);
                dialog.DefaultExt = ext;
                dialog.FileName = Path.GetFileNameWithoutExtension(fileName) + "." + ext;
                dialog.Filter = ext.ToUpperInvariant() + " File|*." + ext;

                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK)
                    return dialog.FileName;
            }

            return null;
        }
    }
}
