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
        internal const string Version = "1.0.4";
        private static readonly string[] _allowed = new[] { ".LESS", ".SCSS", ".COFFEE", ".ICED" };
        private static readonly string _path = Path.Combine(Path.GetTempPath(), "WebCompiler" + Version);

        /// <summary>
        /// Test if a file type is supported by the compilers.
        /// </summary>
        /// <param name="inputFile"></param>
        /// <returns>True if the file type can be compiled.</returns>
        public static bool IsSupported(string inputFile)
        {
            string ext = Path.GetExtension(inputFile).ToUpperInvariant();

            return _allowed.Contains(ext);
        }

        internal static ICompiler GetCompiler(Config config)
        {
            string ext = Path.GetExtension(config.InputFile).ToUpperInvariant();
            ICompiler compiler = null;

            switch (ext)
            {
                case ".LESS":
                    Initialize();
                    compiler = new LessCompiler(_path);
                    break;

                case ".SCSS":
                    compiler = new SassCompiler();
                    break;

                case ".COFFEE":
                    compiler = new CoffeeScriptCompiler();
                    break;

                case ".ICED":
                    Initialize();
                    compiler = new IcedCoffeeScriptCompiler(_path);
                    break;
            }

            return compiler;
        }

        /// <summary>
        /// Initializes the Node environment.
        /// </summary>
        public static void Initialize()
        {
            if (!Directory.Exists(_path))
            {
                OnInitializing();

                Directory.CreateDirectory(_path);
                SaveResourceFile(_path, "WebCompiler.Node.node.7z", "node.7z");
                SaveResourceFile(_path, "WebCompiler.Node.node_modules.7z", "node_modules.7z");
                SaveResourceFile(_path, "WebCompiler.Node.7z.exe", "7z.exe");
                SaveResourceFile(_path, "WebCompiler.Node.7z.dll", "7z.dll");
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

                OnInitialized();
            }
        }

        private static void SaveResourceFile(string path, string resourceName, string fileName)
        {
            using (Stream stream = typeof(CompilerService).Assembly.GetManifestResourceStream(resourceName))
            using (FileStream fs = new FileStream(Path.Combine(path, fileName), FileMode.CreateNew))
            {
                for (int i = 0; i < stream.Length; i++)
                    fs.WriteByte((byte)stream.ReadByte());
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

        public static event EventHandler<EventArgs> Initializing;
        public static event EventHandler<EventArgs> Initialized;
    }
}
