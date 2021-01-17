

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ANPaX.IO.DBConnection.Db;
using ANPaX.IO.DTO;
using ANPaX.IO.interfaces;

using Dapper;

namespace ANPaX.IO.DBConnection.Data
{
    public class FilmFormationConfigurationDataSQLite : IFilmFormationConfigurationData
    {
        private readonly IDataAccess _dataAccess;
        private readonly ConnectionData _connectionData;
        private readonly string _tableName = "FilmFormationConfiguration";

        public FilmFormationConfigurationDataSQLite(IDataAccess dataAccess, ConnectionData connectionData)
        {
            _dataAccess = dataAccess;
            _connectionData = connectionData;

            var da = (SqliteDB)_dataAccess;
            da.CreateDatabaseIfNotExist(GetInitializationProcedure(), GetInitialDataProcedure(), _tableName, _connectionData);
        }

        public async Task<int> CreateFilmFormationConfiguration(FilmFormationConfigurationDTO filmFormationConfiguration)
        {
            var procedure = $@"INSERT INTO {_tableName}
            ( [Description],
		      XWidth, 
		      YWidth, 
		      ZWidth, 
		      MaxCPU, 
		      LargeNumber, 
		      Delta, 
		      DepositionMethod, 
		      WallCollisionMethod, 
		      NeighborslistMethod
            )
            VALUES
            (
              @Description,
		      @XWidth, 
		      @YWidth, 
		      @ZWidth, 
		      @MaxCPU, 
		      @LargeNumber, 
		      @Delta, 
		      @DepositionMethod, 
		      @WallCollisionMethod, 
		      @NeighborslistMethod
            );
            select last_insert_rowid()";

            var p = new DynamicParameters();

            p.Add("Description", filmFormationConfiguration.Description);
            p.Add("XWidth", filmFormationConfiguration.XWidth);
            p.Add("YWidth", filmFormationConfiguration.YWidth);
            p.Add("ZWidth", filmFormationConfiguration.ZWidth);
            p.Add("MaxCPU", filmFormationConfiguration.MaxCPU);
            p.Add("LargeNumber", filmFormationConfiguration.LargeNumber);
            p.Add("Delta", filmFormationConfiguration.Delta);
            p.Add("DepositionMethod", filmFormationConfiguration.DepositionMethod);
            p.Add("WallCollisionMethod", filmFormationConfiguration.WallCollisionMethod);
            p.Add("NeighborslistMethod", filmFormationConfiguration.NeighborslistMethod);

            var id = await _dataAccess.SaveData(procedure, p, _connectionData);

            return id;
        }

        public async Task<FilmFormationConfigurationDTO> GetFilmFormationConfigurationById(int filmFormationConfigurationId)
        {
            var procedure = $@"SELECT *
            FROM {_tableName}
            WHERE Id = {filmFormationConfigurationId}";

            var recs = await _dataAccess.LoadData<FilmFormationConfigurationDTO, dynamic>(procedure, new { }, _connectionData);

            return recs.FirstOrDefault();
        }

        public async Task<List<FilmFormationConfigurationDTO>> GetFilmFormationConfigurations()
        {
            var procedure = $@"SELECT *
            FROM {_tableName}";

            return await _dataAccess.LoadData<FilmFormationConfigurationDTO, dynamic>(procedure, new { }, _connectionData);
        }

        private string GetInitializationProcedure()
        {
            return $@"create table {_tableName}
                (
                    Id                                  INTEGER PRIMARY KEY AUTOINCREMENT,
                    Description                         text not null,
                    XWidth                              numeric not null,
                    YWidth                              numeric not null,
                    ZWidth                              numeric not null,
                    MaxCPU                              integer not null,
                    LargeNumber                         numeric not null,
                    Delta                               numeric not null,
                    DepositionMethod                    text not null,
                    WallCollisionMethod                 text not null,
                    NeighborslistMethod                 text not null
                )";
        }

        private string GetInitialDataProcedure()
        {
            return $@"INSERT INTO {_tableName}
            ( [Description], XWidth, YWidth, ZWidth, MaxCPU, LargeNumber, Delta, DepositionMethod, WallCollisionMethod, NeighborslistMethod ) VALUES
            ( 'Default', 2000.0, 2000.0, 2000.0, 4, 10000000, 1.01, 'Ballistic','Periodic','Accord' );
            select last_insert_rowid()";
        }
    }
}
