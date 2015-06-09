namespace WebCompiler
{
    class CoffeeScriptOptions : BaseOptions
    {
        public CoffeeScriptOptions(Config config)
        {
            Bare = GetValue(config, "bare") == "true";
            Globals = GetValue(config, "globals") == "true";
        }

        public bool Bare { get; set; }

        public bool Globals { get; set; }
    }
}
