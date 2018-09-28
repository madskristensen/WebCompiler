using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using WebCompiler;
using WebCompilerVsix.Commands;
using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace WebCompilerVsix
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", Version, IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.guidCompilerPackageString)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class WebCompilerPackage : AsyncPackage
    {
        public const string Version = "1.4.167";
        public static DTE2 _dte;
        public static Package Package;
        private SolutionEvents _solutionEvents;
        private BuildEvents _buildEvents;

        protected override async System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            _dte = GetService(typeof(DTE)) as DTE2;
            Package = this;

            Logger.Initialize(this, Constants.VSIX_NAME);

            Events2 events = (Events2)_dte.Events;
            _solutionEvents = events.SolutionEvents;
            _solutionEvents.AfterClosing += () => { TableDataSource.Instance.CleanAllErrors(); };
            _solutionEvents.ProjectRemoved += (project) => { TableDataSource.Instance.CleanAllErrors(); };

            _buildEvents = events.BuildEvents;
            _buildEvents.OnBuildBegin += OnBuildBegin;

            CreateConfig.Initialize(this);
            Recompile.Initialize(this);
            CompileOnBuild.Initialize(this);
            RemoveConfig.Initialize(this);
            CompileAllFiles.Initialize(this);
            CleanOutputFiles.Initialize(this);
        }

        private void OnBuildBegin(vsBuildScope Scope, vsBuildAction Action)
        {
            //TableDataSource.Instance.CleanAllErrors();
        }

        public static bool IsDocumentDirty(string documentPath, out IVsPersistDocData persistDocData)
        {
            var serviceProvider = new ServiceProvider((IServiceProvider)_dte);

            IVsHierarchy vsHierarchy;
            uint itemId, docCookie;
            VsShellUtilities.GetRDTDocumentInfo(
                serviceProvider, documentPath, out vsHierarchy, out itemId, out persistDocData, out docCookie);
            if (persistDocData != null)
            {
                int isDirty;
                persistDocData.IsDocDataDirty(out isDirty);
                return isDirty == 1;
            }

            return false;
        }
    }

    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(UIContextGuids80.NoSolution, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class WebCompilerInitPackage : AsyncPackage
    {
        public static Dispatcher _dispatcher;
        public static DTE2 _dte;

        protected override async System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync();

            _dispatcher = Dispatcher.CurrentDispatcher;
            _dte = GetService(typeof(DTE)) as DTE2;

            WebCompiler.CompilerService.Initializing += (s, e) => { StatusText("Installing updated versions of the web compilers..."); };
            WebCompiler.CompilerService.Initialized += (s, e) => { StatusText("Done installing the web compilers"); };

            // Delay execution until VS is idle.
            _dispatcher.BeginInvoke(new Action(() =>
            {
                // Then execute in a background thread.
                System.Threading.ThreadPool.QueueUserWorkItem((o) =>
                {
                    try
                    {
                        WebCompiler.CompilerService.Initialize();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                });
            }), DispatcherPriority.ApplicationIdle, null);
        }

        public static void StatusText(string message)
        {
            WebCompilerInitPackage._dispatcher.BeginInvoke(new Action(() =>
            {
                _dte.StatusBar.Text = message;
            }), DispatcherPriority.ApplicationIdle, null);
        }
    }
}
