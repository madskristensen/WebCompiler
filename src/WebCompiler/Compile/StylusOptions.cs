//using Newtonsoft.Json;

//namespace WebCompiler
//{
//    /// <summary>
//    /// Give all options for the LESS compiler
//    /// </summary>
//    public class StylusOptions : BaseOptions<LessOptions>
//    {
//        private const string trueStr = "true";

//        /// <summary> Creates a new instance of the class.</summary>
//        public StylusOptions()
//        { }

//        /// <summary>
//        /// Load the settings from the config object
//        /// </summary>
//        protected override void LoadSettings(Config config)
//        {
//            var autoPrefix = GetValue(config, "autoPrefix");
//            if (autoPrefix != null)
//                AutoPrefix = autoPrefix;

//            var cssComb = GetValue(config, "cssComb");
//            if (cssComb != null)
//                CssComb = cssComb;

//            var ieCompat = GetValue(config, "ieCompat");
//            if (ieCompat != null)
//                IECompat = ieCompat.ToLowerInvariant() == trueStr;

//            var strictMath = GetValue(config, "strictMath");
//            if (strictMath != null)
//                StrictMath = strictMath.ToLowerInvariant() == trueStr;

//            var strictUnits = GetValue(config, "strictUnits");
//            if (strictUnits != null)
//                StrictUnits = strictUnits.ToLowerInvariant() == trueStr;

//            var rootPath = GetValue(config, "rootPath");
//            if (rootPath != null)
//                RootPath = rootPath;

//            var relativeUrls = GetValue(config, "relativeUrls");
//            if (relativeUrls != null)
//                RelativeUrls = relativeUrls.ToLowerInvariant() == trueStr;
//        }

//        /// <summary>
//        /// The file name should match the compiler name
//        /// </summary>
//        protected override string CompilerFileName
//        {
//            get { return "stylus"; }
//        }
//    }
//}
