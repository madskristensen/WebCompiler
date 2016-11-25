using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;
using WebCompiler;

namespace WebCompilerVsix
{
    class TableDataSource : ITableDataSource
    {
        private static TableDataSource _instance;
        private readonly List<SinkManager> _managers = new List<SinkManager>();
        private static Dictionary<string, TableEntriesSnapshot> _snapshots = new Dictionary<string, TableEntriesSnapshot>();

        [Import]
        private ITableManagerProvider TableManagerProvider { get; set; } = null;

        private TableDataSource()
        {
            var compositionService = ServiceProvider.GlobalProvider.GetService(typeof(SComponentModel)) as IComponentModel;
            compositionService.DefaultCompositionService.SatisfyImportsOnce(this);

            var manager = TableManagerProvider.GetTableManager(StandardTables.ErrorsTable);
            manager.AddSource(this, StandardTableColumnDefinitions.DetailsExpander,
                                                   StandardTableColumnDefinitions.ErrorSeverity, StandardTableColumnDefinitions.ErrorCode,
                                                   StandardTableColumnDefinitions.ErrorSource, StandardTableColumnDefinitions.BuildTool,
                                                   StandardTableColumnDefinitions.ErrorSource, StandardTableColumnDefinitions.ErrorCategory,
                                                   StandardTableColumnDefinitions.Text, StandardTableColumnDefinitions.DocumentName, StandardTableColumnDefinitions.Line, StandardTableColumnDefinitions.Column);
        }

        public static TableDataSource Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TableDataSource();

                return _instance;
            }
        }

        #region ITableDataSource members
        public string SourceTypeIdentifier
        {
            get { return StandardTableDataSources.ErrorTableDataSource; }
        }

        public string Identifier
        {
            get { return PackageGuids.guidCompilerPackageString; }
        }

        public string DisplayName
        {
            get { return Constants.VSIX_NAME; }
        }

        public IDisposable Subscribe(ITableDataSink sink)
        {
            return new SinkManager(this, sink);
        }
        #endregion

        public void AddSinkManager(SinkManager manager)
        {
            // This call can, in theory, happen from any thread so be appropriately thread safe.
            // In practice, it will probably be called only once from the UI thread (by the error list tool window).
            lock (_managers)
            {
                _managers.Add(manager);
            }
        }

        public void RemoveSinkManager(SinkManager manager)
        {
            // This call can, in theory, happen from any thread so be appropriately thread safe.
            // In practice, it will probably be called only once from the UI thread (by the error list tool window).
            lock (_managers)
            {
                _managers.Remove(manager);
            }
        }

        public void UpdateAllSinks()
        {
            lock (_managers)
            {
                foreach (var manager in _managers)
                {
                    manager.UpdateSink(_snapshots.Values);
                }
            }
        }

        public void AddErrors(IEnumerable<CompilerError> errors)
        {
            if (errors == null || !errors.Any())
                return;

            var cleanErrors = errors.Where(e => e != null && !string.IsNullOrEmpty(e.FileName));

            lock (_snapshots)
            {
                foreach (var error in cleanErrors.GroupBy(t => t.FileName))
                {
                    var snapshot = new TableEntriesSnapshot(error.Key, error);
                    _snapshots[error.Key] = snapshot;
                }
            }

            UpdateAllSinks();
        }

        public void CleanErrors(IEnumerable<string> files)
        {
            lock (_snapshots)
            {
                foreach (string file in files)
                {
                    if (_snapshots.ContainsKey(file))
                    {
                        _snapshots[file].Dispose();
                        _snapshots.Remove(file);
                    }
                }
            }

            lock (_managers)
            {
                foreach (var manager in _managers)
                {
                    manager.RemoveSnapshots(files);
                }
            }

            UpdateAllSinks();
        }

        public void CleanAllErrors()
        {
            lock (_snapshots)
            {
                foreach (string file in _snapshots.Keys)
                {
                    var snapshot = _snapshots[file];
                    if (snapshot != null)
                    {
                        snapshot.Dispose();
                    }
                }

                _snapshots.Clear();
            }

            lock (_managers)
            {
                foreach (var manager in _managers)
                {
                    manager.Clear();
                }
            }
        }

        public void BringToFront()
        {
            WebCompilerPackage._dte.ExecuteCommand("View.ErrorList");
        }

        public bool HasErrors()
        {
            lock (_snapshots)
            {
                return _snapshots.Count > 0;
            }
        }

        public bool HasErrors(string fileName)
        {
            lock (_snapshots)
            {
                return _snapshots.ContainsKey(fileName);
            }
        }
    }
}
