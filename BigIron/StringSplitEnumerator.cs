using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigIron
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    static class StringSplitEnumerator
    {
        /// <summary>
        /// Returns a string array that contains the substrings in this instance that are delimited by elements of a specified Unicdoe character array.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static IEnumerable<string> SplitString(this string text, params char[] separator)
        {
            return StringSplitter.DoSplitString(text, separator, int.MaxValue, StringSplitOptions.None);
        }

        /// <summary>
        /// Returns a string array that contains the substrings in this instance that are delimited by elements of a specified Unicode character array. Parameter specifies max number of substrings to return.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="separator"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<string> SplitString(this string text, char[] separator, int count)
        {
            return StringSplitter.DoSplitString(text, separator, count, StringSplitOptions.None);
        }

        /// <summary>
        /// Returns a string array that contains the substrings in this instance that are delimited by elements of a specified Unicdoe character array.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="separator"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IEnumerable<string> SplitString(this string text, char[] separator, StringSplitOptions options)
        {
            return StringSplitter.DoSplitString(text, separator, int.MaxValue, options);
        }

        /// <summary>
        /// Returns a string array that contains the substrings in this instance that are delimited by elements of a specified Unicode character array. Parameter specifies max number of substrings to return.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="separator"></param>
        /// <param name="count"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IEnumerable<string> SplitString(this string text, char[] separator, int count, StringSplitOptions options)
        {
            return StringSplitter.DoSplitString(text, separator, count, options);
        }

        // ************* string separator overloads *************

        /// <summary>
        /// Returns a string array that contains the substrings in this instance that are delimited by elements of a specified string array.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static IEnumerable<string> SplitString(this string text, params string[] separator)
        {
            return StringSplitter.DoSplitString(text, separator, int.MaxValue, StringSplitOptions.None);
        }

        /// <summary>
        /// Returns a string array that contains the substrings in this instance that are delimited by elements of a specified string array. Parameter specifies max number of substrings to return.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="separator"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<string> SplitString(this string text, string[] separator, int count)
        {
            return StringSplitter.DoSplitString(text, separator, count, StringSplitOptions.None);
        }

        /// <summary>
        /// Returns a string array that contains the substrings in this instance that are delimited by elements of a specified string array.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="separator"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IEnumerable<string> SplitString(this string text, string[] separator, StringSplitOptions options)
        {
            return StringSplitter.DoSplitString(text, separator, int.MaxValue, options);
        }

        /// <summary>
        /// Returns a string array that contains the substrings in this instance that are delimited by elements of a specified string array. Parameter specifies max number of substrings to return.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="separator"></param>
        /// <param name="count"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IEnumerable<string> SplitString(this string text, string[] separator, int count, StringSplitOptions options)
        {
            return StringSplitter.DoSplitString(text, separator, count, options);
        }
    }

    static class StringSplitter
    {
        public static IEnumerable<string> DoSplitString(string text, char[] separator, int count, StringSplitOptions options)
        {
            Func<string, int, int> getMatchLength;
            if ((separator == null) || (separator.Length == 0))
                getMatchLength = (text1, index1) => GetWhiteSpaceMatchLength(text1, index1);
            else
                getMatchLength = (text1, index1) => GetCharMatchLength(text1, index1, separator);

            return DoSplitString(text, getMatchLength, count, options);
        }

        public static IEnumerable<string> DoSplitString(string text, string[] separator, int count, StringSplitOptions options)
        {
            Func<string, int, int> getMatchLength;
            if ((separator == null) || (separator.Length == 0))
                getMatchLength = (text1, index1) => GetWhiteSpaceMatchLength(text1, index1);
            else
                getMatchLength = (text1, index1) => GetStringMatchLength(text1, index1, separator);

            return DoSplitString(text, getMatchLength, count, options);
        }

        public static IEnumerable<string> DoSplitString(string text, Func<string, int, int> getMatchLength, int count, StringSplitOptions options)
        {
            bool removeEmpty = (options == StringSplitOptions.RemoveEmptyEntries);

            // special cases
            if (text == null)
                yield break;
            if (count < 1)
                yield break;
            if (text == "")
            {
                if (!removeEmpty)
                    yield return "";
                yield break;
            }
            if (count == 1)
            {
                yield return text;
                yield break;
            }

            // loop over input text
            int startIndex = 0;
            string part;
            int matchLength;
            int currentCount = 0;
            for (int textIndex = 0; textIndex < text.Length; ++textIndex)
            {
                // loop until separator is found
                matchLength = getMatchLength(text, textIndex);
                if (matchLength == 0)
                    continue;

                if (startIndex == textIndex)
                {
                    // two separators followed immediately
                    if (!removeEmpty)
                    {
                        yield return "";
                        ++currentCount;
                    }
                }
                else
                {
                    // return sub string
                    part = text.Substring(startIndex, textIndex - startIndex);
                    yield return part;
                    ++currentCount;
                }

                startIndex = textIndex + matchLength; // matchLength off to skip whitespace/delim
                textIndex += matchLength - 1; //skip separator 
                if (currentCount >= count - 1)
                    break;
            }

            // process remaining text
            if (startIndex == text.Length)
            {
                if (!removeEmpty)
                    yield return "";
            }
            if (startIndex < text.Length)
            {
                part = text.Substring(startIndex);
                yield return part;
            }
        }

        private static int GetWhiteSpaceMatchLength(string text, int index)
        {
            return char.IsWhiteSpace(text[index]) ? 1 : 0;
        }

        private static int GetCharMatchLength(string text, int index, char[] separator)
        {
            return separator.Contains(text[index]) ? 1 : 0;
        }

        private static int GetStringMatchLength(string text, int startIndex, string[] separator)
        {
            foreach (string sep in separator)
            {
                if (String.IsNullOrEmpty(sep))
                    continue;
                if (String.CompareOrdinal(text, startIndex, sep, 0, sep.Length) == 0)
                    return sep.Length;
            }
            return 0;
        }
    }
}
