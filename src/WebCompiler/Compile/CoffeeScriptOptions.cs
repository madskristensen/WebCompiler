namespace WebCompiler
{
    class CoffeeScriptOptions
    {
        public CoffeeScriptOptions(Config config)
        {
            Bare = LessOptions.GetValue(config, "bare") == "true";
            Globals = LessOptions.GetValue(config, "globals") == "true";
        }

        public bool Bare { get; set; }

        public bool Globals { get; set; }
    }
}
