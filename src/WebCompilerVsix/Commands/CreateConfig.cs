using System;
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

        private void BeforeQueryStatus(object sender, EventArgs e)
        {
            var button = (OleMenuCommand)sender;
            var file = ProjectHelpers.GetSelectedItemPaths().ElementAt(0);

            button.Visible = WebCompiler.CompilerService.IsSupported(file);
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
            string jsonFile = Path.Combine(folder, FileHelpers.FILENAME);
            string file = ProjectHelpers.GetSelectedItemPaths().Select(f => MakeRelative(jsonFile, f)).ElementAt(0);
            string outputFile = GetOutputFileName(folder, Path.GetFileName(file));
            string relativeOutputFile = MakeRelative(jsonFile, outputFile);

            if (string.IsNullOrEmpty(outputFile))
                return;

            Config bundle = CreateBundleFile(file, relativeOutputFile);
            
            ConfigHandler handler = new ConfigHandler();
            handler.AddConfig(jsonFile, bundle);

            WebCompilerPackage._dte.ItemOperations.OpenFile(jsonFile);
            ProjectHelpers.AddFileToProject(item.ContainingProject, jsonFile, "None");

            CompilerService.Process(jsonFile);            
        }
        
        private static Config CreateBundleFile(string inputfile,string outputFile)
        {
            return new Config
            {
                IncludeInProject = true,
                Minify = true,
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
