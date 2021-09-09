using System;
using Newtonsoft.Json;

namespace WebCompiler
{
    /// <summary>
    /// Give all options for the Sass compiler
    /// </summary>
    public class SassOptions : BaseOptions<SassOptions>
    {
        private readonly char[] separators = new char[] { ';', ',' };

        /// <summary> Creates a new instance of the class.</summary>
        public SassOptions()
        { }

        /// <summary>
        /// Loads the settings based on the config
        /// </summary>
        protected override void LoadSettings(Config config)
        {
            base.LoadSettings(config);

            string autoPrefix = GetValue(config, "autoPrefix");
            if (autoPrefix != null)
                AutoPrefix = autoPrefix;

            string loadPaths = GetValue(config, "loadPaths");
            if (loadPaths != null)
                LoadPaths = loadPaths.Split(separators, System.StringSplitOptions.RemoveEmptyEntries);

            string style = GetValue(config, "style");
            if (style != null && Enum.TryParse(style.ToString(), true, out SassStyle styleValue))
                Style = styleValue;

            if (int.TryParse(GetValue(config, "precision"), out int precision))
                Precision = precision;

            //string sourceMapUrls = GetValue(config, "sourceMapUrls");
            //if (sourceMapUrls != null && Enum.TryParse(sourceMapUrls.ToString(), true, out SassSourceMapUrls sourceMapUrlsValue))
            //    SourceMapUrls = sourceMapUrlsValue;
        }

        /// <summary>
        /// The file name should match the compiler name
        /// </summary>
        protected override string CompilerFileName
        {
            get { return "sass"; }
        }

        /// <summary>
        /// Autoprefixer will use the data based on current browser popularity and
        /// property support to apply prefixes for you.
        /// </summary>
        [JsonProperty("autoPrefix")]
        public string AutoPrefix { get; set; } = string.Empty;

        /// <summary>
        /// Paths to look for imported files
        /// </summary>
        [JsonProperty("loadPaths")]
        public string[] LoadPaths { get; set; } = new string[0];

        /// <summary>
        /// Type of output style
        /// </summary>
        [JsonProperty("style")]
        public SassStyle Style { get; set; } = SassStyle.Expanded;

        /// <summary>
        /// Precision
        /// </summary>
        public int Precision { get; set; } = 5;

        ///// <summary>
        ///// This option allows you to re-write URL's in imported files so that the URL is always
        ///// relative to the base imported file.
        ///// </summary>
        //[JsonProperty("sourceMapUrls")]
        //public SassSourceMapUrls SourceMapUrls { get; set; } = SassSourceMapUrls.Relative;
    }

    public enum SassStyle
    {
        Expanded,
        Compressed
    }

    //public enum SassSourceMapUrls
    //{
    //    Relative,
    //    Absolute
    //}
}
