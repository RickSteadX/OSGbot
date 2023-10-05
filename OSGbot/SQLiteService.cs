using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic;

namespace OSGbot
{
    public class SQLiteService

    {
        private readonly string _connectionString;
        public readonly string globalValueDBname = "GlobalValues";
        public readonly string DBname = "Users";
        public readonly string channelIDValueName = "channelID";

        public SQLiteService(string connectionString)
        {
            _connectionString = connectionString;

            ExecuteNonQuery($"CREATE TABLE IF NOT EXISTS {DBname} (" +
                        "Id INTEGER PRIMARY KEY, " +
                        "discordID INTEGER UNIQUE, " +
                        "notificationDate INTEGER)");

            ExecuteNonQuery($"CREATE TABLE IF NOT EXISTS {globalValueDBname} (" +
                        "Name TEXT UNIQUE, " +
                        "Value TEXT);");
        }

        public void ExecuteNonQuery(string stringSql)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(stringSql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public void ExecuteInsertOrReplace(string tableName, Dictionary<string, object> columnValues)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                // Build the SQL statement dynamically with placeholders for parameters.
                var sqlBuilder = new StringBuilder($"INSERT OR REPLACE INTO {tableName} (");
                var paramNames = new List<string>();
                foreach (var columnName in columnValues.Keys)
                {
                    paramNames.Add($"{columnName}");
                }
                sqlBuilder.Append(string.Join(", ", paramNames));
                sqlBuilder.Append(") VALUES (");
                sqlBuilder.Append(string.Join(", ", paramNames.Select(paramName => $"${paramName}")));
                sqlBuilder.Append(");");

                using (var command = new SqliteCommand(sqlBuilder.ToString(), connection))
                {
                    foreach (var columnName in columnValues.Keys)
                    {
                        command.Parameters.AddWithValue($"${columnName}", columnValues[columnName]);
                    }

                    command.ExecuteNonQuery();
                }
            }
        }

        public string ExecuteGlobalValueQuery(string valueName)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                string stringSql = $"SELECT Value from {globalValueDBname} WHERE Name='{valueName}'";
                using (var command = new SqliteCommand(stringSql, connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        if (!reader.HasRows) return string.Empty;
                        return reader.GetString(0);
                    }
                }
            }
        }
    }
}
