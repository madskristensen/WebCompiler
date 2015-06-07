using System;

namespace WebCompiler
{
    public class SourceMapEventArgs : EventArgs
    {
        public SourceMapEventArgs(string originalFile, string resultFile)
        {
            OriginalFile = originalFile;
            ResultFile = resultFile;
        }

        public string OriginalFile { get; set; }

        public string ResultFile { get; set; }

    }
}
