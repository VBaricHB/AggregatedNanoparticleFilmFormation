using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ANPaX.IO.DBConnection.Db
{
    public class SqlDb : IDataAccess
    {
        private readonly IConfiguration _config;

        public SqlDb(IConfiguration config)
        {
            _config = config;
        }

        public async Task<List<T>> LoadData<T, U>(string storedProcedure, U parameters, ConnectionData connectionData)
        {
            var connectionString = _config.GetConnectionString(connectionData.SQLConnectionString);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                var rows = await connection.QueryAsync<T>(storedProcedure,
                                                          parameters,
                                                          commandType: CommandType.StoredProcedure);

                return rows.ToList();
            }

        }

        public async Task<int> SaveData<U>(string storedProcedure, U parameters, ConnectionData connectionData)
        {
            var connectionString = _config.GetConnectionString(connectionData.SQLConnectionString);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                return await connection.ExecuteAsync(storedProcedure,
                                                     parameters,
                                                     commandType: CommandType.StoredProcedure);
            }
        }
    }
}
