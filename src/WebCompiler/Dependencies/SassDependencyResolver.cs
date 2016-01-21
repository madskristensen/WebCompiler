using System;
using System.Collections.Generic;
using System.IO;

namespace WebCompiler
{
    class SassDependencyResolver : DependencyResolverBase
    {
        public override string[] SearchPatterns
        {
            get { return new[] { "*.scss", "*.sass" }; }
        }

        public override string FileExtension
        {
            get
            {
                return ".scss";
            }
        }


        /// <summary>
        /// Updates the dependencies of a single file
        /// </summary>
        /// <param name="path"></param>
        public override void UpdateFileDependencies(string path)
        {
            if (this.Dependencies != null)
            {
                path = path.ToLowerInvariant();

                if (!Dependencies.ContainsKey(path))
                    Dependencies[path] = new Dependencies();

                //remove the dependencies registration of this file
                this.Dependencies[path].DependentOn = new HashSet<string>();
                //remove the dependentfile registration of this file for all other files
                foreach (var dependenciesPath in Dependencies.Keys)
                {
                    var lowerDependenciesPath = dependenciesPath.ToLowerInvariant();
                    if (Dependencies[lowerDependenciesPath].DependentFiles.Contains(path))
                    {
                        Dependencies[lowerDependenciesPath].DependentFiles.Remove(path);
                    }
                }

                FileInfo info = new FileInfo(path);
                string content = File.ReadAllText(info.FullName);

                //match both <@import "myFile.scss";> and <@import url("myFile.scss");> syntax
                var matches = System.Text.RegularExpressions.Regex.Matches(content, "@import\\s+(url\\()?(['\"])(.*?)(\\2)\\)?;");
                foreach (System.Text.RegularExpressions.Match match in matches)
                {
                    FileInfo importedfile = new FileInfo(Path.Combine(info.DirectoryName, match.Groups[3].Value));
                    //if the file doesn't end with the correct extension, an import statement without extension is probably used, to re-add the extension (#175)
                    if (string.Compare(importedfile.Extension, FileExtension, StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        importedfile = new FileInfo(importedfile.FullName + this.FileExtension);
                    }

                    var dependencyFilePath = importedfile.FullName.ToLowerInvariant();

                    if (!Dependencies[path].DependentOn.Contains(dependencyFilePath))
                        Dependencies[path].DependentOn.Add(dependencyFilePath);

                    if (!Dependencies.ContainsKey(dependencyFilePath))
                        Dependencies[dependencyFilePath] = new Dependencies();

                    if (!Dependencies[dependencyFilePath].DependentFiles.Contains(path))
                        Dependencies[dependencyFilePath].DependentFiles.Add(path);
                }
            }
        }
    }
}
