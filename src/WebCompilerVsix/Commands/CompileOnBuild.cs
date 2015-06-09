using System;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using NuGet.VisualStudio;

namespace WebCompilerVsix.Commands
{
    internal sealed class CompileOnBuild
    {
        private readonly Package _package;

        private CompileOnBuild(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            _package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(GuidList.guidBundlerCmdSet, PackageCommands.CompileOnBuild);
                var menuItem = new OleMenuCommand(EnableCompileOnBuild, menuCommandID);
                menuItem.BeforeQueryStatus += BeforeQueryStatus;
                commandService.AddCommand(menuItem);
            }
        }

        private bool _isInstalled;

        private void BeforeQueryStatus(object sender, EventArgs e)
        {
            var button = (OleMenuCommand)sender;
            var sourceFile = ProjectHelpers.GetSelectedItemPaths().First();
            var item = ProjectHelpers.GetSelectedItems().First();

            if (item == null || item.ContainingProject == null)
                return;

            button.Visible = Path.GetFileName(sourceFile).Equals(FileHelpers.FILENAME, StringComparison.OrdinalIgnoreCase);

            if (button.Visible)
            {
                _isInstalled = IsPackageInstalled(item.ContainingProject);
                button.Checked = _isInstalled;
            }
        }

        public static CompileOnBuild Instance
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
            Instance = new CompileOnBuild(package);
        }

        private void EnableCompileOnBuild(object sender, EventArgs e)
        {
            var item = ProjectHelpers.GetSelectedItems().First();

            try
            {
                var componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));

                if (!_isInstalled)
                {
                    WebCompilerPackage._dte.StatusBar.Text = @"Installing BuildWebCompiler NuGet package, this may take a minute...";

                    var installer = componentModel.GetService<IVsPackageInstaller>();
                    installer.InstallPackage(null, item.ContainingProject, "BuildWebCompiler", (System.Version)null, false);

                    WebCompilerPackage._dte.StatusBar.Text = @"Finished installing the BuildWebCompiler NuGet package";
                }
                else
                {
                    WebCompilerPackage._dte.StatusBar.Text = @"Uninstalling BuildWebCompiler NuGet package, this may take a minute...";

                    var uninstaller = componentModel.GetService<IVsPackageUninstaller>();
                    uninstaller.UninstallPackage(item.ContainingProject, "BuildWebCompiler", false);

                    WebCompilerPackage._dte.StatusBar.Text = @"Finished uninstalling the BuildWebCompiler NuGet package";
                }
            }
            catch (Exception)
            {
                WebCompilerPackage._dte.StatusBar.Text = @"Unable to install the BuildWebCompiler NuGet package";
            }
        }

        private bool IsPackageInstalled(Project project)
        {
            var componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
            IVsPackageInstallerServices installerServices = componentModel.GetService<IVsPackageInstallerServices>();

            return installerServices.IsPackageInstalled(project, "BuildWebCompiler");
        }
    }
}
