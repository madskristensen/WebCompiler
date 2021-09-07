using System;
using System.Linq;
using System.IO;
using System.Diagnostics;

namespace WebCompiler
{
    /// <summary>
    /// A service for working with the compilers.
    /// </summary>
    public static class CompilerService
    {
        internal const string Version = "1.12.0";
        private static readonly string _path = Path.Combine(Path.GetTempPath(), "WebCompiler" + Version);
        private static object _syncRoot = new object(); // Used to lock on the initialize step

        /// <summary>A list of allowed file extensions.</summary>
        public static readonly string[] AllowedExtensions = new[] { ".LESS", ".SCSS", ".SASS", ".STYL", ".COFFEE", ".ICED", ".JS", ".JSX", ".ES6", ".HBS", ".HANDLEBARS" };

        /// <summary>
        /// Test if a file type is supported by the compilers.
        /// </summary>
        /// <param name="inputFile"></param>
        /// <returns>True if the file type can be compiled.</returns>
        public static bool IsSupported(string inputFile)
        {
            string ext = Path.GetExtension(inputFile).ToUpperInvariant();

            return AllowedExtensions.Contains(ext);
        }

        internal static ICompiler GetCompiler(Config config)
        {
            string ext = Path.GetExtension(config.InputFile).ToUpperInvariant();
            ICompiler compiler = null;

            Initialize();

            switch (ext)
            {
                case ".LESS":
                    compiler = new LessCompiler(_path);
                    break;

                case ".HANDLEBARS":
                case ".HBS":
                    compiler = new HandlebarsCompiler(_path);
                    break;

                case ".SCSS":
                case ".SASS":
                    compiler = new SassCompiler(_path);
                    break;

                case ".STYL":
                case ".STYLUS":
                    compiler = new StylusCompiler(_path);
                    break;

                case ".COFFEE":
                case ".ICED":
                    compiler = new IcedCoffeeScriptCompiler(_path);
                    break;

                case ".JS":
                case ".JSX":
                case ".ES6":
                    compiler = new BabelCompiler(_path);
                    break;
            }

            return compiler;
        }

        /// <summary>
        /// Initializes the Node environment.
        /// </summary>
        public static void Initialize()
        {
            string node_modules = Path.Combine(_path, "node_modules");
            string node_exe = Path.Combine(_path, "node.exe");
            string log_file = Path.Combine(_path, "log.txt");

            lock (_syncRoot)
            {
                if (!Directory.Exists(node_modules) || !File.Exists(node_exe) || !File.Exists(log_file))
                {
                    OnInitializing();

                    if (Directory.Exists(_path))
                        Directory.Delete(_path, true);

                    Directory.CreateDirectory(_path);
                    SaveResourceFile(_path, "WebCompiler.Node.node.7z", "node.7z");
                    SaveResourceFile(_path, "WebCompiler.Node.node_modules.7z", "node_modules.7z");
                    SaveResourceFile(_path, "WebCompiler.Node.7za.exe", "7za.exe");
                    SaveResourceFile(_path, "WebCompiler.Node.prepare.cmd", "prepare.cmd");

                    ProcessStartInfo start = new ProcessStartInfo
                    {
                        WorkingDirectory = _path,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        FileName = "cmd.exe",
                        Arguments = "/c prepare.cmd"
                    };

                    Process p = Process.Start(start);
                    p.WaitForExit();

                    // If this file is written, then the initialization was successful.
                    File.WriteAllText(log_file, DateTime.Now.ToLongDateString());

                    OnInitialized();
                }
            }
        }

        private static void SaveResourceFile(string path, string resourceName, string fileName)
        {
            using (Stream stream = typeof(CompilerService).Assembly.GetManifestResourceStream(resourceName))
            using (FileStream fs = new FileStream(Path.Combine(path, fileName), FileMode.Create))
            {
                stream.CopyTo(fs);
            }
        }

        private static void OnInitializing()
        {
            if (Initializing != null)
            {
                Initializing(null, EventArgs.Empty);
            }
        }

        private static void OnInitialized()
        {
            if (Initialized != null)
            {
                Initialized(null, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Fires when the compilers are about to be initialized.
        /// </summary>
        public static event EventHandler<EventArgs> Initializing;

        /// <summary>
        /// Fires when the compilers have been initialized.
        /// </summary>
        public static event EventHandler<EventArgs> Initialized;
    }
}
