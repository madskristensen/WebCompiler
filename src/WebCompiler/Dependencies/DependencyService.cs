using System.Collections.Generic;

namespace WebCompiler
{
    /// <summary>
    /// Takes care of all administration of dependencies
    /// </summary>
    class DependencyService
    {
        /// <summary>
        /// The different types of dependencies
        /// </summary>
        private enum DependencyType
        {
            None = 0,
            Sass = 1,
            Less = 2
        }

        /// <summary>
        /// Contains all dependency resolvers for all different kinds of dependencies
        /// </summary>
        private static Dictionary<DependencyType, DependencyResolverBase> _dependencies = new Dictionary<DependencyType, DependencyResolverBase>();

        /// <summary>
        /// Gets the dependency tree for the type of file of the given sourceFile
        /// </summary>
        /// <returns>the dependency tree</returns>
        public static Dictionary<string, Dependencies> GetDependencies(string projectRootPath,
                                                                       string sourceFile)
        {
            if (projectRootPath == null)
                return null;

            var dependencyType = GetDependencyType(sourceFile);

            if(!_dependencies.ContainsKey(dependencyType))
            {
                switch (dependencyType)
                {
                    case DependencyType.Sass:
                        _dependencies[dependencyType] = new SassDependencyResolver();
                        break;
                    case DependencyType.Less:
                        _dependencies[dependencyType] = new LessDependencyResolver();
                        break;
                }
            }

            if (_dependencies.ContainsKey(dependencyType))
            {
                _dependencies[dependencyType].UpdateFileDependencies(sourceFile);
                return _dependencies[dependencyType].GetDependencies(projectRootPath);
            }
            else
                return null;
        }

        /// <summary>
        /// Determines the dependencyType that goes with the given source file
        /// </summary>
        private static DependencyType GetDependencyType(string sourceFile)
        {
            string ext = System.IO.Path.GetExtension(sourceFile).ToUpperInvariant();
            switch (ext)
            {
                case ".LESS":
                    return DependencyType.Less;

                case ".SCSS":
                case ".SASS":
                    return DependencyType.Sass;

                case ".STYL":
                case ".STYLUS":
                    return DependencyType.None;

                case ".COFFEE":
                case ".ICED":
                    return DependencyType.None;

                case ".HBS":
                case ".HANDLEBARS":
                    return DependencyType.None;

                case ".JS":
                case ".JSX":
                case ".ES6":
                    return DependencyType.None;
            }

            return DependencyType.None;
        }
    }
}
