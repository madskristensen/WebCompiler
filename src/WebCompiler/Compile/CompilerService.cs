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
        private static readonly string[] _allowed = new[] { ".LESS", ".SCSS", ".COFFEE" };
        private static string _path = Path.Combine(Path.GetTempPath(), "WebCompiler");

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
                    compiler = new LessCompiler(_path);
                    break;

                case ".SCSS":
                    compiler = new SassCompiler();
                    break;

                case ".COFFEE":
                    compiler = new CoffeeScriptCompiler();
                    break;
            }

            return compiler;
        }

        /// <summary>
        /// Initializes the Node environment.
        /// </summary>
        /// <param name="version"></param>
        public static void Initialize(string version = Version)
        {
            _path = _path += version;

            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
                SaveResourceFile(_path, "WebCompiler.Node.node.zip", "node.zip");
                SaveResourceFile(_path, "WebCompiler.Node.node_modules.zip", "node_modules.zip");
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

                Process.Start(start);
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
    }
}
