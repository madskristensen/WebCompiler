using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.JSON.Core.Schema;

namespace WebCompilerVsix.JSON
{
    [Export(typeof(IJSONSchemaSelector))]
    class CompilerConfigSchemaSelector : IJSONSchemaSelector
    {
        public event EventHandler AvailableSchemasChanged { add { } remove { } }

        public Task<IEnumerable<string>> GetAvailableSchemasAsync()
        {
            return Task.FromResult(Enumerable.Empty<string>());
        }

        public string GetSchemaFor(string fileLocation)
        {
            string fileName = Path.GetFileName(fileLocation);

            if (fileName.Equals(Constants.CONFIG_FILENAME, StringComparison.OrdinalIgnoreCase))
                return GetSchemaFileName("json\\compilerconfig-schema.json");

            if (fileName.Equals(Constants.DEFAULTS_FILENAME, StringComparison.OrdinalIgnoreCase))
                return GetSchemaFileName("json\\compilerdefaults-schema.json");

            return null;
        }

        private static string GetSchemaFileName(string relativePath)
        {
            string assembly = Assembly.GetExecutingAssembly().Location;
            string folder = Path.GetDirectoryName(assembly);
            return Path.Combine(folder, relativePath);
        }
    }
}
