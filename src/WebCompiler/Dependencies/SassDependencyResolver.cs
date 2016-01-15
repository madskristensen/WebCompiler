using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCompiler
{
    class SassDependencyResolver : DependencyResolverBase
    {
        
        public override string[] SearchPatterns
        {
            get
            {
                return new string[] { "*.scss","*.sass"};
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

                if (!this.Dependencies.ContainsKey(path))
                    this.Dependencies[path] = new WebCompiler.Dependencies();

                //remove the dependencies registration of this file
                this.Dependencies[path].DependentOn = new HashSet<string>();
                //remove the dependentfile registration of this file for all other files
                foreach (var dependenciesPath in this.Dependencies.Keys)
                {
                    var lowerDependenciesPath = path.ToLowerInvariant();
                    if (this.Dependencies[lowerDependenciesPath].DependentFiles.Contains(path))
                    {
                        this.Dependencies[lowerDependenciesPath].DependentFiles.Remove(path);
                    }
                }

                FileInfo info = new FileInfo(path);
                string content = File.ReadAllText(info.FullName);

                var matches = System.Text.RegularExpressions.Regex.Matches(content, "@import\\s+(['\"])(.*?)(\\1);");
                foreach (System.Text.RegularExpressions.Match match in matches)
                {
                    FileInfo importedfile = new FileInfo(System.IO.Path.Combine(info.DirectoryName, match.Groups[2].Value));
                    var dependencyFilePath = importedfile.FullName.ToLowerInvariant();

                    if (!this.Dependencies[path].DependentOn.Contains(dependencyFilePath))
                        this.Dependencies[path].DependentOn.Add(dependencyFilePath);

                    if (!this.Dependencies.ContainsKey(dependencyFilePath))
                        this.Dependencies[dependencyFilePath] = new WebCompiler.Dependencies();

                    if (!this.Dependencies[dependencyFilePath].DependentFiles.Contains(path))
                        this.Dependencies[dependencyFilePath].DependentFiles.Add(path);
                }
            }
        }
    }
}
