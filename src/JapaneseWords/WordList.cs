using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace JapaneseWords
{
    public class WordList
    {
        private readonly ConnectionFactory _connectionFactory;

        private readonly Lazy<ReadOnlyCollection<WordData>> _words;

        private WordList()
        {
            _connectionFactory = new ConnectionFactory();
            _words = new Lazy<ReadOnlyCollection<WordData>>(LoadWords);
        }

        private static Lazy<WordList> _instance = new Lazy<WordList>(() => new WordList());
        public static WordList Instance { get { return _instance.Value; } }


        private ReadOnlyCollection<WordData> LoadWords()
        {
            var words = new List<WordData>();
            _connectionFactory.GetOpenCommand(command =>
            {
                command.CommandText = "select kanji from dict;";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var wordString = reader[0].ToString();
                        var word = new WordData(wordString);
                        words.Add(word);
                    }
                }
            });
            return words.AsReadOnly();
        }

        public IEnumerable<WordData> Words { get { return _words.Value; } }
    }

    public class WordData
    {
        private readonly string _word;
        private readonly List<string> _characters;

        public WordData(string word)
        {
            _word = word;
            _characters = word.SplitBySurrogatePairs().ToList();
        }

        public string Word { get { return _word; } }
        public IList<string> Characters { get { return _characters; } }
    }

    public class WordDataEqualityComparer : IEqualityComparer<WordData>
    {
        public bool Equals(WordData x, WordData y)
        {
            return x.Word == y.Word;
        }

        public int GetHashCode(WordData obj)
        {
            return obj.Word.GetHashCode();
        }
    }
}
