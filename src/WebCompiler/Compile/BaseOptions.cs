namespace WebCompiler
{
    /// <summary>
    /// Base class containing methods to all extensions options
    /// </summary>
    public class BaseOptions
    {
        internal static string GetValue(Config config, string key)
        {
            if (config.Options.ContainsKey(key))
                return config.Options[key].ToString();

            return string.Empty;
        }
    }
}
