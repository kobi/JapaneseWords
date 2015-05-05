using System;
using System.Linq;
using JapaneseWords;
using System.Globalization;
using Xunit;
using Xunit.Abstractions;

namespace JapaneseWordsTests
{
    public class Tests
    {
        private readonly ITestOutputHelper _log;

        public Tests(ITestOutputHelper log)
        {
            _log = log;
        }

        [Fact]
        public void KanjiListSkipPattern()
        {
            var kanjis = KanjiList.Instance;
            var skipPatternCount = kanjis.Kanjis.Count(k => k.SkipPatternType != null);
            _log.WriteLine("skipPatternCount: {0}", skipPatternCount);
            _log.WriteLine("skip Pattern 1: {0}", kanjis.Kanjis.Count(k => k.SkipPatternType == SkipPatternType.LeftRight));
            Assert.NotEqual(0, skipPatternCount);
        }

        [Fact]
        public void KanjiListNotEmpty()
        {
            var kanjis = KanjiList.Instance;
            var count = kanjis.Kanjis.Count();
            _log.WriteLine("kanji count: {0}", count);
            Assert.NotEmpty(kanjis.Kanjis);
        }

        [Fact]
        public void GetCharacters()
        {
            var s = "𠀋1𠀋";
            var len = s.Length;
            Assert.NotEqual(3, len);
            var chars = s.SplitBySurrogatePairs();
            Assert.Equal(3, chars.Count());
        }

        [Fact]
        public void WordListTest()
        {
            var words = WordList.Instance;
            var count = words.Words.Count();
            _log.WriteLine("word count: {0}", count);
            Assert.NotEmpty(words.Words);
        }

        [Fact]
        public void RadicalTest()
        {
            var radicals = RadicalList.Instance;
            Assert.True(radicals.IsRadical("鬥"));
            Assert.False(radicals.IsRadical("!"));
        }

        [Fact]
        public void QueryGetWords()
        {
            var q = new QueryWordsByRadicals();
            var words = q.FindWordsWithSameRadical().ToList();
            _log.WriteLine("Found {0} words. {1}", words.Count(), String.Join(" ", words.Select(w => w.Word)));
            Assert.NotEmpty(words);
        }

        [Fact]
        public void KanjiListGroupBySkipType()
        {
            var kanjis = KanjiList.Instance;
            var groups = kanjis.Kanjis.GroupBy(k => k.SkipPatternType);
            foreach (var group in groups)
            {
                _log.WriteLine("{0,20} - {1}", group.Key.ToString(), group.Count());
            }
        }
    }
}