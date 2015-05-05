using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JapaneseWords
{
    public class RadicalList
    {
        private RadicalList()
        {
            _radicals = new Lazy<HashSet<string>>(LoadRadicals);
        }

        private readonly string _radicalDataFilePath = @"Data\radicals.dat";
        private Lazy<HashSet<string>> _radicals;

        private static Lazy<RadicalList> _instance = new Lazy<RadicalList>(() => new RadicalList());
        public static RadicalList Instance { get { return _instance.Value; } }


        private HashSet<string> LoadRadicals()
        {
            var radicals = from line in File.ReadLines(_radicalDataFilePath)
                           select line.Split('\t') into tokens
                           let chars = tokens[0].SplitBySurrogatePairs().Concat(tokens[1].SplitBySurrogatePairs())
                           from c in chars
                           //blcklist . - it is there before and after small village.
                           where c != "."
                           select c;
            return new HashSet<string>(radicals);
        }

        public bool IsRadical(string character)
        {
            return _radicals.Value.Contains(character);
        }
    }
}
