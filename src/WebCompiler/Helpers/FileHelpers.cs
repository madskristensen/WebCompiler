using System;
using System.IO;

namespace WebCompiler
{
    /// <summary>
    /// Helper class for file interactions
    /// </summary>
    public static class FileHelpers
    {
        /// <summary>
        /// Finds the relative path between two files.
        /// </summary>
        public static string MakeRelative(string baseFile, string file)
        {
            Uri baseUri = new Uri(baseFile, UriKind.RelativeOrAbsolute);
            Uri fileUri = new Uri(file, UriKind.RelativeOrAbsolute);

            return Uri.UnescapeDataString(baseUri.MakeRelativeUri(fileUri).ToString());
        }

        /// <summary>
        /// If a file has the read-only attribute, this method will remove it.
        /// </summary>
        /// <param name="fileName"></param>
        public static void RemoveReadonlyFlagFromFile(string fileName)
        {
            FileInfo file = new FileInfo(fileName);

            if (file.Exists && file.IsReadOnly)
                file.IsReadOnly = false;
        }
    }
}
