using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCompiler
{
    public class MinificationResult
    {
        public MinificationResult(string content, string sourceMap)
        {
            MinifiedContent = content;
            SourceMap = sourceMap;
        }

        public string MinifiedContent { get; set; }
        public string SourceMap { get; set; }
    }
}
