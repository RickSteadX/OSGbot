using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace OSGbot
{
    public class SQLiteService
    {
        private readonly string _connectionString;

        public SQLiteService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CreateTable(string createTableSql)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqliteCommand(createTableSql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public void InsertData(string tableName, string insertSql)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqliteCommand(insertSql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
