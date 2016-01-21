﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCompiler
{
    /// <summary>
    /// Base class for file dependency resolver
    /// </summary>
    public abstract class DependencyResolverBase
    {
        private Dictionary<string, WebCompiler.Dependencies> _dependencies;

        /// <summary>
        /// Stores all resolved dependencies
        /// </summary>
        protected Dictionary<string, WebCompiler.Dependencies> Dependencies
        {
            get
            {
                return _dependencies;
            }
        }

        /// <summary>
        /// The search patterns to use to determine what files should be used to build the dependency tree
        /// </summary>
        public abstract string[] SearchPatterns
        {
            get;
        }

        /// <summary>
        /// The file extension of files of this type
        /// </summary>
        public abstract string FileExtension
        {
            get;
        }

        /// <summary>
        /// Gets the dependency tree
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, Dependencies> GetDependencies(string projectRootPath)
        {
            if (_dependencies == null)
            {
                _dependencies = new Dictionary<string, WebCompiler.Dependencies>();

                List<string> files = new List<string>();
                foreach(var pattern in this.SearchPatterns)
                {
                    files.AddRange(System.IO.Directory.GetFiles(projectRootPath, pattern, System.IO.SearchOption.AllDirectories));
                }
                
                foreach (var path in (from p in files select p.ToLowerInvariant()))
                {
                    this.UpdateFileDependencies(path);
                }
            }

            return _dependencies;
        }

        /// <summary>
        /// Updates the dependencies for the given file
        /// </summary>
        public abstract void UpdateFileDependencies(string path);

    }
}
