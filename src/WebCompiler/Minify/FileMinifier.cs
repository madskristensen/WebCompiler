using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using NUglify;

namespace WebCompiler
{
    /// <summary>
    /// Used by the compilers to minify the output files.
    /// </summary>
    public class FileMinifier
    {
        internal static MinificationResult MinifyFile(Config config)
        {
            FileInfo file = config.GetAbsoluteOutputFile();
            string extension = file.Extension.ToUpperInvariant();

            switch (extension)
            {
                case ".JS":
                    return MinifyJavaScript(config, file.FullName);

                case ".CSS":
                    return MinifyCss(config, file.FullName);
            }

            return null;
        }

        private static MinificationResult MinifyJavaScript(Config config, string file)
        {
            string content = File.ReadAllText(file);
            var settings = JavaScriptOptions.GetSettings(config);

            if (config.Minify.ContainsKey("enabled") && config.Minify["enabled"].ToString().Equals("false", StringComparison.OrdinalIgnoreCase))
                return null;


            string minFile = GetMinFileName(file);

            var minifiedJs = Uglify.Js(content, settings);
            string result = minifiedJs.Code;

            bool containsChanges = FileHelpers.HasFileContentChanged(minFile, result);

            if (!string.IsNullOrEmpty(result))
            {
                OnBeforeWritingMinFile(file, minFile, containsChanges);

                if (containsChanges)
                {
                    File.WriteAllText(minFile, result, new UTF8Encoding(true));
                }

                OnAfterWritingMinFile(file, minFile, containsChanges);

                GzipFile(config, minFile, containsChanges);
            }

            return new MinificationResult(result, null);
        }

        private static MinificationResult MinifyCss(Config config, string file)
        {
            string content = File.ReadAllText(file);
            var settings = CssOptions.GetSettings(config);

            if (config.Minify.ContainsKey("enabled") && config.Minify["enabled"].ToString().Equals("false", StringComparison.OrdinalIgnoreCase))
                return null;


            // Remove control characters which AjaxMin can't handle
            content = Regex.Replace(content, @"[\u0000-\u0009\u000B-\u000C\u000E-\u001F]", string.Empty);
            var minifiedCss = Uglify.Css(content, settings);

            string result = minifiedCss.Code;
            string minFile = GetMinFileName(file);
            bool containsChanges = FileHelpers.HasFileContentChanged(minFile, result);

            OnBeforeWritingMinFile(file, minFile, containsChanges);

            if (containsChanges)
            {
                File.WriteAllText(minFile, result, new UTF8Encoding(true));
            }

            OnAfterWritingMinFile(file, minFile, containsChanges);

            GzipFile(config, minFile, containsChanges);

            return new MinificationResult(result, null);
        }

        private static string GetMinFileName(string file)
        {
            string ext = Path.GetExtension(file);

            string fileName = file.Substring(0, file.LastIndexOf(ext));
            if (!fileName.EndsWith(".min"))
            {
                fileName += ".min";
            }

            return fileName + ext;
        }

        private static void GzipFile(Config config, string sourceFile, bool containsChanges)
        {
            if (!config.Minify.ContainsKey("gzip") || !config.Minify["gzip"].ToString().Equals("true", StringComparison.OrdinalIgnoreCase))
                return;

            var gzipFile = sourceFile + ".gz";

            OnBeforeWritingGzipFile(sourceFile, gzipFile, containsChanges);

            if (containsChanges)
            {
                using (var sourceStream = File.OpenRead(sourceFile))
                using (var targetStream = File.OpenWrite(gzipFile))
                using (var gzipStream = new GZipStream(targetStream, CompressionMode.Compress))
                    sourceStream.CopyTo(gzipStream);
            }

            OnAfterWritingGzipFile(sourceFile, gzipFile, containsChanges);
        }

        private static void OnBeforeWritingMinFile(string file, string minFile, bool containsChanges)
        {
            if (BeforeWritingMinFile != null)
            {
                BeforeWritingMinFile(null, new MinifyFileEventArgs(file, minFile, containsChanges));
            }
        }

        private static void OnAfterWritingMinFile(string file, string minFile, bool containsChanges)
        {
            if (AfterWritingMinFile != null)
            {
                AfterWritingMinFile(null, new MinifyFileEventArgs(file, minFile, containsChanges));
            }
        }


        private static void OnBeforeWritingGzipFile(string minFile, string gzipFile, bool containsChanges)
        {
            if (BeforeWritingGzipFile != null)
            {
                BeforeWritingGzipFile(null, new MinifyFileEventArgs(minFile, gzipFile, containsChanges));
            }
        }

        private static void OnAfterWritingGzipFile(string minFile, string gzipFile, bool containsChanges)
        {
            if (AfterWritingGzipFile != null)
            {
                AfterWritingGzipFile(null, new MinifyFileEventArgs(minFile, gzipFile, containsChanges));
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

        /// <summary>
        /// Fires before the .gz file is written to disk
        /// </summary>
        public static event EventHandler<MinifyFileEventArgs> BeforeWritingGzipFile;

        /// <summary>
        /// Fires after the .gz file is written to disk
        /// </summary>
        public static event EventHandler<MinifyFileEventArgs> AfterWritingGzipFile;
    }
}
