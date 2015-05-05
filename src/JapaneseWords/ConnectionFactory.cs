using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace JapaneseWords
{
    public class ConnectionFactory
    {
        private readonly string _dictionaryPath = @"Data\dict.sqlite";

        public ConnectionFactory()
        {

        }

        public void GetOpenConnection(Action<SQLiteConnection> action)
        {
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = _dictionaryPath;
            using (var connection = new SQLiteConnection(builder.ToString()))
            {
                connection.Open();
                action(connection);
            }
        }

        public void GetOpenCommand(Action<SQLiteCommand> action)
        {
            GetOpenConnectionCommand((connection, command) => action(command));
        }

        public void GetOpenConnectionCommand(Action<SQLiteConnection, SQLiteCommand> action)
        {
            GetOpenConnection(connection =>
            {
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    action(connection, command);
                }
            });
        }
    }
}