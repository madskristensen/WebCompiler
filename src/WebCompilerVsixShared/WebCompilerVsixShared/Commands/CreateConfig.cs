using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using WebCompiler;

namespace WebCompilerVsix
{
    internal sealed class CreateConfig
    {
        private readonly Package _package;

        private CreateConfig(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            _package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

            if (commandService != null)
            {
                CommandID menuCommandID = new CommandID(PackageGuids.guidCompilerCmdSet, PackageIds.CreateConfigFile);
                OleMenuCommand menuItem = new OleMenuCommand(AddConfig, menuCommandID);
                menuItem.BeforeQueryStatus += BeforeQueryStatus;
                commandService.AddCommand(menuItem);
            }
        }

        private List<Config> _reCompileConfigs = new List<Config>();
        private ProjectItem _item;

        private void BeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand button = (OleMenuCommand)sender;
            button.Visible = button.Enabled = false;

            _item = GetProjectItem(WebCompilerPackage._dte);
            _reCompileConfigs.Clear();

            if (_item == null || _item.ContainingProject == null || _item.Properties == null)
                return;

            string configFile = _item.ContainingProject.GetConfigFile();
            string inputFile = _item.Properties.Item("FullPath").Value.ToString();

            button.Visible = button.Enabled = WebCompiler.CompilerService.IsSupported(inputFile);

            if (!button.Visible)
                return;

            IEnumerable<Config> configs = ConfigFileProcessor.IsFileConfigured(configFile, inputFile);

            if (configs != null && configs.Any())
            {
                button.Text = "Re-compile file";
                _reCompileConfigs.AddRange(configs);
            }
            else
            {
                button.Text = "Compile file";
            }
        }

        public static ProjectItem GetProjectItem(DTE2 dte)
        {
            Window2 window = dte.ActiveWindow as Window2;

            if (window == null)
                return null;

            if (window.Type == vsWindowType.vsWindowTypeDocument)
            {
                Document doc = dte.ActiveDocument;

                if (doc != null && !string.IsNullOrEmpty(doc.FullName))
                {
                    return dte.Solution.FindProjectItem(doc.FullName);
                }
            }

            return ProjectHelpers.GetSelectedItems().FirstOrDefault();
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
            string folder = _item.ContainingProject.GetRootFolder();
            string configFile = _item.ContainingProject.GetConfigFile();
            string relativeFile = FileHelpers.MakeRelative(configFile, ProjectHelpers.GetSelectedItemPaths().First());

            // Recompile if already configured
            if (_reCompileConfigs.Any())
            {
                string absoluteFile = Path.Combine(folder, relativeFile).Replace("/", "\\");
                CompilerService.SourceFileChanged(configFile, absoluteFile);
                return;
            }

            // Create new config
            WebCompilerPackage._dte.StatusBar.Progress(true, "Compiling file", 0, 3);
            string inputFile = _item.Properties.Item("FullPath").Value.ToString();
            string outputFile = GetOutputFileName(inputFile);

            if (string.IsNullOrEmpty(outputFile))
                return;

            string relativeOutputFile = FileHelpers.MakeRelative(configFile, outputFile);
            Config config = CreateConfigFile(relativeFile, relativeOutputFile);

            WebCompilerPackage._dte.StatusBar.Progress(true, "Compiling file", 1, 3);

            ProjectHelpers.CheckFileOutOfSourceControl(configFile);
            ConfigHandler handler = new ConfigHandler();
            handler.AddConfig(configFile, config);

            _item.ContainingProject.AddFileToProject(configFile, "None");
            WebCompilerPackage._dte.StatusBar.Progress(true, "Compiling file", 2, 3);

            // Create defaults file
            string defaultsFile = Path.Combine(folder, Constants.DEFAULTS_FILENAME);
            ProjectHelpers.CheckFileOutOfSourceControl(defaultsFile);
            handler.CreateDefaultsFile(defaultsFile);
            ProjectHelpers.AddNestedFile(configFile, defaultsFile, "None");
            WebCompilerPackage._dte.StatusBar.Progress(true, "Creating defaults file", 3, 3);

            CompilerService.Process(configFile, new[] { config });
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

        private static string GetOutputFileName(string inputFile)
        {
            string extension = Path.GetExtension(inputFile).ToLowerInvariant();
            string ext = ".css";

            if (extension == ".coffee" || extension == ".iced" || extension == ".litcoffee" || extension == ".jsx" || extension == ".es6" || extension == ".hbs" || extension == ".handlebars")
                ext = ".js";

            if (extension == ".js")
                ext = ".es5.js";

            return Path.ChangeExtension(inputFile, ext);
        }
    }
}
