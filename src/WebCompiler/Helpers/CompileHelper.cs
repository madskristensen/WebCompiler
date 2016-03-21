using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCompiler.Helpers
{
    public static class CompileHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetCompiledExtension(string filename)
        {
            string extension = Path.GetExtension(filename).ToLowerInvariant();
            switch (extension)
            {
                case ".coffee":
                case ".iced":
                case ".litcoffee":
                case ".jsx":
                case ".es6":
                    return ".js";

                case ".js":
                    return ".es5.js";

                default:
                    return ".css";
            }
        }
    }
}
