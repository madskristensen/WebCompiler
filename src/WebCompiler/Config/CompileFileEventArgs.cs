using System;

namespace WebCompiler
{
    public class CompileFileEventArgs : EventArgs
    {
        public CompileFileEventArgs(Config config, string baseFolder)
        {
            Config = config;
            BaseFolder = baseFolder;
        }

        public Config Config { get; set; }
        
        public string BaseFolder { get; set; }
    }
}
