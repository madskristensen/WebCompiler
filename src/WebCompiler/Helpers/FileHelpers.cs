using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCompiler
{
    public static class FileHelpers
    {
        public static string MakeRelative(string baseFile, string file)
        {
            Uri baseUri = new Uri(baseFile, UriKind.RelativeOrAbsolute);
            Uri fileUri = new Uri(file, UriKind.RelativeOrAbsolute);

            return Uri.UnescapeDataString(baseUri.MakeRelativeUri(fileUri).ToString());
        }

        public static void RemoveReadonlyFlagFromFile(string fileName)
        {
            FileInfo file = new FileInfo(fileName);

            if (file.Exists && file.IsReadOnly)
                file.IsReadOnly = false;
        }
    }
}
