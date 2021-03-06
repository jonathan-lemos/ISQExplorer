using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using ISQExplorer.Functional;

namespace ISQExplorer.Misc
{
    public static class Strings
    {
        /// <summary>
        /// Returns a capture group given by searching the <paramref name="input">input string</paramref> for the given regex <paramref name="pattern"/>
        /// </summary>
        /// <param name="input">The string to be searched.</param>
        /// <param name="pattern">A string containing a regex to search it with.</param>
        /// <param name="number">Return the nth capture group. By default this is 1 which gets the first capture group. Do not pass a number below 0.</param>
        /// <returns>The first capture group, or the number described by the <paramref name="number"/> parameter.</returns>
        public static Optional<string> Capture(this string input, string pattern, int number = 1)
        {
            var matches = Regex.Match(input, pattern).Groups.Values.ToList();
            if (matches.Count == 1 && number <= 1)
            {
                return matches[0].Value != "" ? matches[0].Value : new Optional<string>();
            }

            return matches.Count <= number ? new Optional<string>() : matches[number].Value;
        }

        /// <summary>
        /// Unescapes any HTML glyphs in the string.
        /// </summary>
        /// <param name="s">The input string.</param>
        /// <returns>The unescaped string.</returns>
        public static string HtmlDecode(this string s) => HttpUtility.HtmlDecode(s);

        /// <summary>
        /// Escapes any special HTML characters in the string.
        /// </summary>
        /// <param name="s">The input string.</param>
        /// <returns>The escaped string.</returns>
        public static string HtmlEncode(this string s) => HttpUtility.HtmlEncode(s);

        /// <summary>
        /// Gets the index of all matches of the given pattern.
        /// </summary>
        /// <param name="s">The input string.</param>
        /// <param name="pattern">A regular expression to match.</param>
        /// <param name="options">Regex options, if any.</param>
        /// <returns>The position of the first character of each match in the string.</returns>
        public static IEnumerable<int> IndexOfAll(this string s, string pattern, RegexOptions options = RegexOptions.None)
        {
            var res = Regex.Match(s, pattern, options);
            while (res.Success)
            {
                yield return res.Index;
                res = res.NextMatch();
            }
        }

        /// <summary>
        /// Returns true if a string is null, empty, or contains only whitespace.
        /// </summary>
        /// <param name="s">The input string.</param>
        /// <returns>True if the string is null or blank, false if not.</returns>
        public static bool IsBlank(this string s) => s == null || s.Trim() == "";

        /// <summary>
        /// Joins an enumerable of strings into one string, linking them with the given <paramref name="delimiter"/>.
        /// </summary>
        /// <param name="enumerable">The enumerable of strings.</param>
        /// <param name="delimiter">The delimiter to join on.</param>
        /// <returns>The joined string.</returns>
        public static string Join(this IEnumerable<string> enumerable, string delimiter) =>
            string.Join(delimiter, enumerable);

        /// <summary>
        /// Returns true if the string matches the given regex, false if not.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="pattern">The regex to match it against.</param>
        /// <param name="substring">True if the regex can match a substring of the full string, false if it must match the entire string.</param>
        /// <returns>True if it matches, false if not.</returns>
        public static bool Matches(this string input, string pattern, bool substring = false)
        {
            if (!substring)
            {
                if (!pattern.StartsWith("^"))
                {
                    pattern = $"^{pattern}";
                }

                if (!pattern.EndsWith("$"))
                {
                    pattern = $"{pattern}$";
                }
            }

            return Regex.IsMatch(input, pattern);
        }

        /// <summary>
        /// Converts a UTF-8 string into a byte array.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <returns>A byte array corresponding to the input string.</returns>
        public static byte[] ToBytes(this string s) => Encoding.UTF8.GetBytes(s);
    }
}