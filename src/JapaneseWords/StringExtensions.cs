using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace JapaneseWords
{
    public static class StringExtensions
    {
        /// <remarks> http://stackoverflow.com/a/28155130/7586 </remarks>
        public static int[] ToCodePoints(this string str)
        {
            if (str == null)
                throw new ArgumentNullException("str");

            var codePoints = new List<int>(str.Length);
            for (int i = 0; i < str.Length; i++)
            {
                codePoints.Add(Char.ConvertToUtf32(str, i));
                if (Char.IsHighSurrogate(str[i]))
                    i += 1;
            }

            return codePoints.ToArray();
        }

        public static IEnumerable<string> SplitBySurrogatePairs(this string str)
        {
            //var positions = StringInfo.ParseCombiningCharacters(str);
            //var pos = 0;
            var enumerator = StringInfo.GetTextElementEnumerator(str);
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current as string;
            }
        }
    }
}
