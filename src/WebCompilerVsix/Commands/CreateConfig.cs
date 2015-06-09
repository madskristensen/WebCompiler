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
                var menuCommandID = new CommandID(GuidList.guidBundlerCmdSet, PackageCommands.CreateConfigFile);
                var menuItem = new OleMenuCommand(AddBundle, menuCommandID);
                menuItem.BeforeQueryStatus += BeforeQueryStatus;
                commandService.AddCommand(menuItem);
            }
        }

        private List<Config> _reCompileConfigs = new List<Config>();

        private void BeforeQueryStatus(object sender, EventArgs e)
        {
            _reCompileConfigs.Clear();
            var button = (OleMenuCommand)sender;
            var sourceFile = ProjectHelpers.GetSelectedItemPaths().ElementAt(0);

            button.Visible = WebCompiler.CompilerService.IsSupported(sourceFile);

            var item = ProjectHelpers.GetSelectedItems().ElementAt(0);

            if (item == null || item.ContainingProject == null)
                return;

            string folder = ProjectHelpers.GetRootFolder(item.ContainingProject);
            string configFile = Path.Combine(folder, FileHelpers.FILENAME);

            var configs = ConfigFileProcessor.IsFileConfigured(configFile, sourceFile);

            if (configs.Any())
            {
                button.Text = "Re-compile file";
                _reCompileConfigs.AddRange(configs);
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

        private void AddBundle(object sender, EventArgs e)
        {
            var item = ProjectHelpers.GetSelectedItems().ElementAt(0);

            if (item.ContainingProject == null)
                return;

            string folder = ProjectHelpers.GetRootFolder(item.ContainingProject);
            string configFile = Path.Combine(folder, FileHelpers.FILENAME);
            string relativeFile = MakeRelative(configFile, ProjectHelpers.GetSelectedItemPaths().First());

            // Recompile if already configured
            if (_reCompileConfigs.Any())
            {
                string absoluteFile = Path.Combine(folder, relativeFile).Replace("/", "\\");
                CompilerService.SourceFileChanged(configFile, absoluteFile);
                return;
            }

            // Create new config
            string outputFile = GetOutputFileName(folder, Path.GetFileName(relativeFile));
            string relativeOutputFile = MakeRelative(configFile, outputFile);

            if (string.IsNullOrEmpty(outputFile))
                return;

            Config bundle = CreateBundleFile(relativeFile, relativeOutputFile);

            ConfigHandler handler = new ConfigHandler();
            handler.AddConfig(configFile, bundle);

            WebCompilerPackage._dte.ItemOperations.OpenFile(configFile);
            ProjectHelpers.AddFileToProject(item.ContainingProject, configFile, "None");

            CompilerService.Process(configFile);
        }

        private static Config CreateBundleFile(string inputfile, string outputFile)
        {
            return new Config
            {
                IncludeInProject = true,
                OutputFile = outputFile,
                InputFile = inputfile
            };
        }

        private static string MakeRelative(string baseFile, string file)
        {
            Uri baseUri = new Uri(baseFile, UriKind.RelativeOrAbsolute);
            Uri fileUri = new Uri(file, UriKind.RelativeOrAbsolute);

            return baseUri.MakeRelativeUri(fileUri).ToString();
        }

        private static string GetOutputFileName(string folder, string fileName)
        {
            string extension = Path.GetExtension(fileName);
            string ext = "css";

            if (extension == ".coffee" || extension == ".iced")
                ext = "js";

            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.InitialDirectory = folder;
                dialog.DefaultExt = ext;
                dialog.FileName = Path.GetFileNameWithoutExtension(fileName);

                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK)
                    return dialog.FileName;
            }

            return null;
        }
    }
}
