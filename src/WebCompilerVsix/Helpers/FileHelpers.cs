using System.IO;
using WebCompiler;

namespace WebCompilerVsix
{
    public static class FileHelpers
    {
        public const string FILENAME = "compilerconfig.json";
        
        public static bool HasSourceMap(string file, out string sourceMap)
        {
            if (File.Exists(file + ".map"))
            {
                sourceMap = file + ".map";
                return true;
            }

            sourceMap = null;

            return false;
        }
    }
}
