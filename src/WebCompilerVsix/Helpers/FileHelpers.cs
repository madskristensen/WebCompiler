using System.IO;
using EnvDTE;

namespace WebCompilerVsix
{
    public static class FileHelpers
    {
        public const string FILENAME = "compilerconfig.json";

        internal static string GetConfigFile(Project project)
        {
            string folder = ProjectHelpers.GetRootFolder(project);
            return Path.Combine(folder, FileHelpers.FILENAME);
        }
    }
}
