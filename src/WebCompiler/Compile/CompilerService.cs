using System;
using System.Linq;
using System.IO;

namespace WebCompiler
{
    public static class CompilerService
    {
        private static readonly string[] _allowed = new[] { ".LESS", ".SCSS" };

        public static bool IsSupported(string inputFile)
        {
            string ext = Path.GetExtension(inputFile).ToUpperInvariant();

            return _allowed.Contains(ext);
        }

        public static ICompiler GetCompiler(Config config)
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
            }
            
            return compiler;
        }

    }
}
