namespace WebCompiler
{
    class CoffeeScriptOptions : BaseOptions
    {
		private const string trueStr = "true";

		public CoffeeScriptOptions(Config config)
        {
            Bare = GetValue(config, "bare").ToLowerInvariant() == trueStr;
            Globals = GetValue(config, "globals").ToLowerInvariant() == trueStr;
        }

        public bool Bare { get; set; }

        public bool Globals { get; set; }
    }
}
