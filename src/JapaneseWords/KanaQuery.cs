using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JapaneseWords
{
    public class KanaQuery
    {
        private readonly ConnectionFactory _connectionFactory;

        public KanaQuery()
        {
            _connectionFactory = new ConnectionFactory();
        }

        private long CountByLikePattern(string pattern)
        {
            long result = 0;
            _connectionFactory.GetOpenCommand(command =>
            {
                command.CommandText = "select count(*) from dict where kana like @Pattern escape '>'";
                command.Parameters.AddWithValue("@Pattern", pattern);
                object count = command.ExecuteScalar();
                result = (long)count;
            });
            return result;
        }

        public long CountKanaStartEnd(char startWith, char endsWith)
        {
            string likePattern = String.Format(">{0}%>{1}", startWith, endsWith);
            return CountByLikePattern(likePattern);
        }
        public long CountKanaStart(char startWith)
        {
            string likePattern = String.Format(">{0}%", startWith);
            return CountByLikePattern(likePattern);
        }
    }
}
