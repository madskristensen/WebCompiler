using System;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using NuGet.VisualStudio;
using WebCompiler;

namespace WebCompilerVsix.Commands
{
    internal sealed class CompileOnBuild
    {
        private readonly Package _package;
        private bool _isInstalled;
        private Project _project;

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
                var menuCommandID = new CommandID(PackageGuids.guidCompilerCmdSet, PackageIds.CompileOnBuild);
                var menuItem = new OleMenuCommand(EnableCompileOnBuild, menuCommandID);
                menuItem.BeforeQueryStatus += BeforeQueryStatus;
                commandService.AddCommand(menuItem);
            }
        }

        private void BeforeQueryStatus(object sender, EventArgs e)
        {
            var button = (OleMenuCommand)sender;
            var item = ProjectHelpers.GetSelectedItems().FirstOrDefault();

            if (item == null) // Project
            {
                var project = ProjectHelpers.GetActiveProject();

                if (project != null)
                {
                    string config = project.GetConfigFile();

                    if (!string.IsNullOrEmpty(config) && File.Exists(config))
                    {
                        _isInstalled = IsPackageInstalled(project);
                        _project = project;
                        button.Checked = _isInstalled;
                        button.Visible = true;

                        DisableUnsupportProjectType(project, button);

                        return;
                    }
                }
            }

            // Config file
            if (item == null || item.ContainingProject == null || item.Properties == null)
            {
                button.Visible = false;
                return;
            }

            bool isConfigFile = item.IsConfigFile();

            if (!isConfigFile)
            {
                button.Visible = false;
                return;
            }

            if (!DisableUnsupportProjectType(item.ContainingProject, button))
                return;

            button.Visible = isConfigFile;

            if (button.Visible)
            {
                _isInstalled = IsPackageInstalled(item.ContainingProject);
                _project = item.ContainingProject;
                button.Checked = _isInstalled;
            }
        }

        private static bool DisableUnsupportProjectType(Project project, OleMenuCommand button)
        {
            if (project.IsKind(ProjectTypes.WEBSITE_PROJECT) || project.IsKind(ProjectTypes.ASPNET_5))
            {
                button.Enabled = false;
                return false;
            }

            return true;
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
            if (_project == null)
                return;

            var componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));

            if (!_isInstalled)
            {
                var question = MessageBox.Show("A NuGet package will be installed to augment the MSBuild process, but no files will be added to the project.\rThis may require an internet connection.\r\rDo you want to continue?", Constants.VSIX_NAME, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (question == DialogResult.No)
                    return;

                Telemetry.TrackEvent("VS add compile on build");
                Version version = new Version(WebCompilerPackage.Version);

#if DEBUG
                version = (Version)null;
#endif
                System.Threading.ThreadPool.QueueUserWorkItem((o) =>
                {
                    try
                    {
                        WebCompilerPackage._dte.StatusBar.Text = $"Installing {Constants.NUGET_ID} v{WebCompilerPackage.Version} NuGet package, this may take a minute...";
                        WebCompilerPackage._dte.StatusBar.Animate(true, vsStatusAnimation.vsStatusAnimationSync);

                        var installer = componentModel.GetService<IVsPackageInstaller>();
                        installer.InstallPackage(null, _project, Constants.NUGET_ID, version, false);

                        WebCompilerPackage._dte.StatusBar.Text = $"Finished installing the {Constants.NUGET_ID} v{WebCompilerPackage.Version} NuGet package";
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                        WebCompilerPackage._dte.StatusBar.Text = $"Unable to install the {Constants.NUGET_ID} v{WebCompilerPackage.Version} NuGet package";
                    }
                    finally
                    {
                        WebCompilerPackage._dte.StatusBar.Animate(false, vsStatusAnimation.vsStatusAnimationSync);
                    }
                });
            }
            else
            {
                Telemetry.TrackEvent("VS remove compile on build");

                System.Threading.ThreadPool.QueueUserWorkItem((o) =>
                {
                    try
                    {
                        WebCompilerPackage._dte.StatusBar.Text = $"Uninstalling {Constants.NUGET_ID} NuGet package, this may take a minute...";
                        WebCompilerPackage._dte.StatusBar.Animate(true, vsStatusAnimation.vsStatusAnimationSync);
                        var uninstaller = componentModel.GetService<IVsPackageUninstaller>();
                        uninstaller.UninstallPackage(_project, Constants.NUGET_ID, false);

                        WebCompilerPackage._dte.StatusBar.Text = $"Finished uninstalling the {Constants.NUGET_ID} NuGet package";
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                        WebCompilerPackage._dte.StatusBar.Text = $"Unable to ininstall the {Constants.NUGET_ID} NuGet package";
                    }
                    finally
                    {
                        WebCompilerPackage._dte.StatusBar.Animate(false, vsStatusAnimation.vsStatusAnimationSync);
                    }
                });
            }
        }

        private bool IsPackageInstalled(Project project)
        {
            var componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
            IVsPackageInstallerServices installerServices = componentModel.GetService<IVsPackageInstallerServices>();

            return installerServices.IsPackageInstalled(project, Constants.NUGET_ID);
        }
    }
}
