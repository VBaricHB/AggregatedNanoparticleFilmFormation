using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using Microsoft.Data.Sqlite;

namespace ANPaX.IO.DBConnection.Db
{
    public class SqliteDB : IDataAccess
    {
        public async Task<List<T>> LoadData<T, U>(string procedure, U parameters, ConnectionData connectionData)
        {
            using (IDbConnection connection = new SqliteConnection(connectionData.SQLiteConnectionString))
            {
                var result = await connection.QueryAsync<T>(procedure, parameters);
                return result.ToList();
            }
        }

        public async Task<int> SaveData<U>(string procedure, U parameters, ConnectionData connectionData)
        {
            using (IDbConnection connection = new SqliteConnection(connectionData.SQLiteConnectionString))
            {
                var rec = await connection.QueryAsync<int>(procedure,
                                                     parameters);
                return rec.FirstOrDefault();
            }
        }

        public void CreateDatabaseIfNotExist(string dbGenerationProcedure, string initialDataProcedure, string tableName, ConnectionData connectionData)
        {
            if (!File.Exists(connectionData.SQLiteFilePath) || !DoesTableExist(tableName, connectionData))
            {
                CreateInitialDatabase(dbGenerationProcedure, initialDataProcedure, connectionData);
            }
        }

        public static bool DoesTableExist(string tableName, ConnectionData connectionData)
        {
            using (IDbConnection connection = new SqliteConnection(connectionData.SQLiteConnectionString))
            {
                connection.Open();
                var result = connection.Query($"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}'");
                return result.Any();
            }
        }

        private static void CreateInitialDatabase(string dbGenerationProcedure, string initialData, ConnectionData connectionData)
        {
            using (IDbConnection connection = new SqliteConnection(connectionData.SQLiteConnectionString))
            {
                connection.Open();
                connection.Execute(dbGenerationProcedure);
            }

            CreateInitialContent(initialData, connectionData);
        }

        private static void CreateInitialContent(string initialData, ConnectionData connectionData)
        {
            using (IDbConnection connection = new SqliteConnection(connectionData.SQLiteConnectionString))
            {
                connection.Open();
                var id = connection.Query<int>(initialData).First();
            }
        }
    }
}
