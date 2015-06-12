using System.IO;
using EnvDTE;

namespace WebCompilerVsix
{
    public static class FileHelpers
    {
        public const string FILENAME = "compilerconfig.json";

        internal static string GetConfigFile(Project project)
        {
            return Path.Combine(project.GetRootFolder(), FileHelpers.FILENAME);
        }
    }
}
