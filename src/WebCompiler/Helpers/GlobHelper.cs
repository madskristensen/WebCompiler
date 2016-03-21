using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            int pos = 0;

            while (pattern.Length != pos)
            {
                switch (pattern[pos])
                {
                    case '?':
                        break;

                    case '*':
                        for (int i = text.Length; i >= pos; i--)
                        {
                            if (GlobHelper.Glob(text.Substring(i), pattern.Substring(pos + 1)))
                            {
                                return true;
                            }
                        }
                        return false;

                    default:
                        if (text.Length == pos || char.ToUpper(pattern[pos]) != char.ToUpper(text[pos]))
                        {
                            return false;
                        }
                        break;
                }

                pos++;
            }

            return text.Length == pos;
        }
    }
}