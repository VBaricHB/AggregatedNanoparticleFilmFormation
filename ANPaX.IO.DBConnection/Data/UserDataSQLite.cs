using System.Collections.Generic;
using System.Threading.Tasks;

using ANPaX.IO.DBConnection.Db;
using ANPaX.IO.DTO;
using ANPaX.IO.interfaces;

using Dapper;

namespace ANPaX.IO.DBConnection.Data
{
    public class UserDataSQLite : IUserData
    {
        private readonly IDataAccess _dataAccess;
        private readonly ConnectionData _connectionData;
        private readonly string _tableName = "User";
        public UserDataSQLite(IDataAccess dataAccess, ConnectionData connectionData)
        {
            _dataAccess = dataAccess;
            _connectionData = connectionData;

            var da = (SqliteDB)_dataAccess;
            da.CreateDatabaseIfNotExist(GetInitializationProcedure(), GetInitialDataProcedure(), _tableName, _connectionData);
        }

        public async Task<int> CreateUser(UserDTO user)
        {
            var procedure = $@"INSERT INTO {_tableName}
            (
              User,
		      EMail
            )
            VALUES
            (
              @User,
		      @EMail
            );
            select last_insert_rowid()";

            var p = new DynamicParameters();

            p.Add("User", user.User);
            p.Add("EMail", user.EMail);

            var id = await _dataAccess.SaveData(procedure, p, _connectionData);

            return id;
        }

        public async Task<List<UserDTO>> GetUsers()
        {
            var procedure = $@"SELECT *
            FROM {_tableName}";

            return await _dataAccess.LoadData<UserDTO, dynamic>(procedure, new { }, _connectionData);
        }


        private string GetInitializationProcedure()
        {
            return $@"create table {_tableName}
                (
                    Id                                  INTEGER PRIMARY KEY AUTOINCREMENT,
                    User                                text not null,
                    EMail                               text not null                    
                )";
        }

        private string GetInitialDataProcedure()
        {
            return $@"INSERT INTO {_tableName}
            ( User, EMail )
            VALUES
            ( 'Test', 'Test@Test.de' );
            select last_insert_rowid()";
        }

    }
}
