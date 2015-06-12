using System;
using System.Linq;
using System.IO;

namespace WebCompiler
{
    /// <summary>
    /// A service for working with the compilers.
    /// </summary>
    public static class CompilerService
    {
        private static readonly string[] _allowed = new[] { ".LESS", ".SCSS", ".COFFEE" };

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
                    compiler = new LessCompiler();
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
    }
}
