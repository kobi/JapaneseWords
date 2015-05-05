using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JapaneseWords
{
    public class QueryWordsByRadicals
    {
        private readonly RadicalList _radicals;
        private readonly KanjiList _kanji;
        private readonly WordList _words;

        public QueryWordsByRadicals()
        {
            _radicals = RadicalList.Instance;
            _kanji = KanjiList.Instance;
            _words = WordList.Instance;
        }

        public IEnumerable<WordData> FindWordsWithSameRadical()
        {
            var w = from word in _words.Words
                    let chars = word.Characters
                    where chars.Count >= 2
                    where chars.All(_kanji.IsKanji)
                    let kanjis = chars.Select(_kanji.GetKanji)
                    where kanjis.All(k => k.SkipPatternType == SkipPatternType.LeftRight)
                    where kanjis.All(k => !_radicals.IsRadical(k.Kanji))
                    let firstKanjiRadicalIndex = kanjis.First().MainRadicalIndex
                    where kanjis.All(k => k.MainRadicalIndex == firstKanjiRadicalIndex)
                    orderby kanjis.Count() descending
                    select word;
            return w.Distinct(new WordDataEqualityComparer());
        }
    }
}
