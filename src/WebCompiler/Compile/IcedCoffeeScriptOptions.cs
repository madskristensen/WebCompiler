using Newtonsoft.Json;

namespace WebCompiler
{
    public class IcedCoffeeScriptOptions : BaseOptions<IcedCoffeeScriptOptions>
    {
        public IcedCoffeeScriptOptions()
        { }

        private const string trueStr = "true";

        protected override void LoadSettings(Config config)
        {
            var bare = GetValue(config, "bare");
            if (bare != null)
                Bare = bare.ToLowerInvariant() == trueStr;

            var runtimeMode = GetValue(config, "runtimeMode");
            if (runtimeMode != null)
                RuntimeMode = runtimeMode.ToLowerInvariant();
        }

        protected override string CompilerFileName
        {
            get { return "coffeescript"; }
        }

        [JsonProperty("bare")]
        public bool Bare { get; set; } = false;

        [JsonProperty("runtimeMode")]
        public string RuntimeMode { get; set; } = "node";
    }
}
