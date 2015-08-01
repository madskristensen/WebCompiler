namespace WebCompiler
{
    class IcedCoffeeScriptOptions : BaseOptions
    {
		private const string trueStr = "true";

		public IcedCoffeeScriptOptions(Config config)
        {
            Bare = GetValue(config, "bare").ToLowerInvariant() == trueStr;
            RuntimeMode = GetValue(config, "runtimeMode").ToLowerInvariant();
        }

        public bool Bare { get; set; }

        public string RuntimeMode{ get; set; }
    }
}
