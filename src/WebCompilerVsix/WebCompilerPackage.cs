using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
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
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", Version, IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.guidCompilerPackageString)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class WebCompilerPackage : Package
    {
        public const string Version = "1.4.166";
        public static DTE2 _dte;
        public static Package Package;
        public static Dispatcher _dispatcher;
        private SolutionEvents _events;

        protected override void Initialize()
        {
            _dte = GetService(typeof(DTE)) as DTE2;
            _dispatcher = Dispatcher.CurrentDispatcher;
            Package = this;

            Telemetry.SetDeviceName(_dte.Edition);
            Logger.Initialize(this, Constants.VSIX_NAME);

            Events2 events = _dte.Events as Events2;
            _events = events.SolutionEvents;
            _events.AfterClosing += () => { ErrorList.CleanAllErrors(); };
            _events.ProjectRemoved += (project) => { ErrorList.CleanAllErrors(); };

            CreateConfig.Initialize(this);
            Recompile.Initialize(this);
            CompileOnBuild.Initialize(this);
            RemoveConfig.Initialize(this);
            CompileAllFiles.Initialize(this);
            CleanOutputFiles.Initialize(this);

            base.Initialize();
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
}
