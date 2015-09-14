using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using Microsoft.Ajax.Utilities;

namespace WebCompiler
{
    /// <summary>
    /// Used by the compilers to minify the output files.
    /// </summary>
    public class FileMinifier
    {
        internal static MinificationResult MinifyFile(Config config)
        {
            string file = config.GetAbsoluteOutputFile();
            string extension = Path.GetExtension(file).ToUpperInvariant();

            switch (extension)
            {
                case ".JS":
                    return MinifyJavaScript(config, file);

                case ".CSS":
                    return MinifyCss(config, file);
            }

            return null;
        }

        private static MinificationResult MinifyJavaScript(Config config, string file)
        {
            string content = File.ReadAllText(file);
            var settings = JavaScriptOptions.GetSettings(config);
            var minifier = new Minifier();

            string ext = Path.GetExtension(file);
            string minFile = file.Substring(0, file.LastIndexOf(ext)) + ".min" + ext;
            string mapFile = minFile + ".map";

            string result = minifier.MinifyJavaScript(content, settings);

            if (!string.IsNullOrEmpty(result) && FileHelpers.HasFileContentChanged(minFile, result))
            {
                OnBeforeWritingMinFile(file, minFile);
                File.WriteAllText(minFile, result, new UTF8Encoding(true));
                OnAfterWritingMinFile(file, minFile);

                GzipFile(config, minFile);
            }

            return new MinificationResult(result, null);
        }

        private static MinificationResult MinifyCss(Config config, string file)
        {
            string content = File.ReadAllText(file);
            var settings = CssOptions.GetSettings(config);
            var minifier = new Minifier();

            string result = minifier.MinifyStyleSheet(content, settings);

            if (!string.IsNullOrEmpty(result))
            {
                string minFile = GetMinFileName(file);

                if (FileHelpers.HasFileContentChanged(minFile, result))
                {
                    OnBeforeWritingMinFile(file, minFile);
                    File.WriteAllText(minFile, result, new UTF8Encoding(true));
                    OnAfterWritingMinFile(file, minFile);

                    GzipFile(config, minFile);
                }
            }

            return new MinificationResult(result, null);
        }

        private static string GetMinFileName(string file)
        {
            string ext = Path.GetExtension(file);
            return file.Substring(0, file.LastIndexOf(ext)) + ".min" + ext;
        }

        private static void GzipFile(Config config, string sourceFile)
        {
            if (!config.Minify.ContainsKey("gzip") || !config.Minify["gzip"].ToString().Equals("true", StringComparison.OrdinalIgnoreCase))
                return;

            var gzipFile = sourceFile + ".gz";
            OnBeforeWritingGzipFile(sourceFile, gzipFile);

            using (var sourceStream = File.OpenRead(sourceFile))
            using (var targetStream = File.OpenWrite(gzipFile))
            using (var gzipStream = new GZipStream(targetStream, CompressionMode.Compress))
                sourceStream.CopyTo(gzipStream);

            OnAfterWritingGzipFile(sourceFile, gzipFile);
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


        private static void OnBeforeWritingGzipFile(string minFile, string gzipFile)
        {
            if (BeforeWritingGzipFile != null)
            {
                BeforeWritingGzipFile(null, new MinifyFileEventArgs(minFile, gzipFile));
            }
        }

        private static void OnAfterWritingGzipFile(string minFile, string gzipFile)
        {
            if (AfterWritingGzipFile != null)
            {
                AfterWritingGzipFile(null, new MinifyFileEventArgs(minFile, gzipFile));
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
