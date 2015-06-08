using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Ajax.Utilities;

namespace WebCompiler
{
    /// <summary>
    /// Used by the compilers to minify the output files.
    /// </summary>
    public class FileMinifier
    {
        internal static MinificationResult MinifyFile(string file, bool produceSourceMap)
        {
            string extension = Path.GetExtension(file).ToUpperInvariant();

            switch (extension)
            {
                case ".JS":
                    return MinifyJavaScript(file);

                case ".CSS":
                    return MinifyCss(file);
            }

            return null;
        }

        private static MinificationResult MinifyJavaScript(string file)
        {
            var settings = new CodeSettings()
            {
                EvalTreatment = EvalTreatment.Ignore,
                TermSemicolons = true,
                PreserveImportantComments = false,
            };

            var minifier = new Minifier();

            string ext = Path.GetExtension(file);
            string minFile = file.Substring(0, file.LastIndexOf(ext)) + ".min" + ext;
            string mapFile = minFile + ".map";

            string result = minifier.MinifyJavaScript(File.ReadAllText(file), settings);

            if (!string.IsNullOrEmpty(result))
            {
                OnBeforeWritingMinFile(file, minFile);
                File.WriteAllText(minFile, result, new UTF8Encoding(true));
                OnAfterWritingMinFile(file, minFile);
            }

            return new MinificationResult(result, null);
        }

        private static MinificationResult MinifyCss(string file)
        {
            string content = File.ReadAllText(file);

            var settings = new CssSettings()
            {
                CommentMode = CssComment.Hacks
            };

            var minifier = new Minifier();

            string result = minifier.MinifyStyleSheet(content, settings);

            if (!string.IsNullOrEmpty(result))
            {
                string minFile = GetMinFileName(file);

                OnBeforeWritingMinFile(file, minFile);
                File.WriteAllText(minFile, result, new UTF8Encoding(true));
                OnAfterWritingMinFile(file, minFile);
            }

            return new MinificationResult(result, null);
        }

        private static string GetMinFileName(string file)
        {
            string ext = Path.GetExtension(file);
            return file.Substring(0, file.LastIndexOf(ext)) + ".min" + ext;
        }

        private static void OnBeforeWritingMinFile(string file, string minFile)
        {
            if (BeforeWritingMinFile != null)
            {
                BeforeWritingMinFile(null, new MinifyFileEventArgs(file, minFile));
            }
        }

        private static void OnAfterWritingMinFile(string file, string minFile)
        {
            if (AfterWritingMinFile != null)
            {
                AfterWritingMinFile(null, new MinifyFileEventArgs(file, minFile));
            }
        }

        /// <summary>
        /// Fires before the minified file is written to disk.
        /// </summary>
        public static event EventHandler<MinifyFileEventArgs> BeforeWritingMinFile;

        /// <summary>
        /// /// Fires after the minified file is written to disk.
        /// </summary>
        public static event EventHandler<MinifyFileEventArgs> AfterWritingMinFile;
    }
}
