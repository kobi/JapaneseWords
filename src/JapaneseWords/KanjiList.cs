using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace JapaneseWords
{
    public class KanjiList
    {
        private readonly string _kanjiDataFilePath = @"Data\kanji.dat";
        private readonly Lazy<Dictionary<string, KanjiData>> _kanjis;

        private KanjiList()
        {
            _kanjis = new Lazy<Dictionary<string, KanjiData>>(LoadKanjis);
        }

        private static Lazy<KanjiList> _instance = new Lazy<KanjiList>(() => new KanjiList());
        public static KanjiList Instance { get { return _instance.Value; } }

        private Dictionary<string, KanjiData> LoadKanjis()
        {
            var kanjis = new Dictionary<string, KanjiData>();
            var regex = new Regex(
                @"^(?<Kanji>\w|[^|]{2})\|       # \w doesn't match 𠀋 - surrogate pair
                B(?<RadicalIndex>\d+)\b
                (?=[^|]*\bS(?<StrokeCount>\d+\b))?
                (?=[^|]*\bP
                    (?<SkipPattern>(?<SkipPatternType>[1-4])-(?<SkipPatternFirstGroupStrokeCount>\d+)-\d+\b)
                )?
                ",
                RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
            var fileContent = File.ReadAllLines(_kanjiDataFilePath);
            foreach (var line in fileContent)
            {
                if (String.IsNullOrWhiteSpace(line)) continue;
                var match = regex.Match(line);
                if (!match.Success)
                    throw new InvalidOperationException("All lines must match the pattern. Line doesn't match: " + line);
                SkipPatternType? skip = null;
                if (match.Groups["SkipPattern"].Success)
                    skip = (SkipPatternType)Int32.Parse(match.Groups["SkipPatternType"].Value, CultureInfo.InvariantCulture);
                var kanji = new KanjiData
                {
                    Kanji = match.Groups["Kanji"].Value,
                    MainRadicalIndex = Int32.Parse(match.Groups["RadicalIndex"].Value, CultureInfo.InvariantCulture),
                    SkipPatternType = skip,
                };
                kanjis.Add(kanji.Kanji, kanji);
            }
            return kanjis;
        }

        public IEnumerable<KanjiData> Kanjis { get { return _kanjis.Value.Values; } }

        public bool IsKanji(string str)
        {
            return _kanjis.Value.ContainsKey(str);
        }

        public KanjiData GetKanji(string kanji)
        {
            return _kanjis.Value[kanji];
        }
    }

    public enum SkipPatternType
    {
        LeftRight = 1,
        TopBottom = 2,
        Enclosing = 3,
        Other = 4,
    }

    public class KanjiData
    {
        public string Kanji { get; set; }
        public int MainRadicalIndex { get; set; }
        public SkipPatternType? SkipPatternType { get; set; }
    }
}