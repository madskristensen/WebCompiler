using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebCompiler.Helpers
{
    /// <summary>
    /// Utils for glob matching
    /// </summary>
    public static class GlobHelper
    {
        /// <summary>
        /// Returns true if the input text contains a basic glob character: *?
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsGlobPattern(string text)
        {
            return text != null && text.LastIndexOfAny(new char[] { '*', '?' }) >= 0;
        }

        /// <summary>
        /// String matching including basic glob patterns: *?
        /// </summary>
        /// <param name="text">string to be matched</param>
        /// <param name="pattern">pattern to match against</param>
        /// <returns></returns>
        public static bool Glob(this string text, string pattern)
        {
            StringBuilder sb = new StringBuilder(pattern, pattern.Length + 10);
            sb.Replace('*', (char)1).Replace('?', (char)2).Replace('/', '\\');

            pattern = Regex.Escape(sb.ToString());

            sb.Clear().Append('^').Append(pattern).Replace("\u0001", ".*").Replace("\u0002", ".").Append('$');

            return Regex.IsMatch(text, sb.ToString(), RegexOptions.IgnoreCase);
        }
    }
}