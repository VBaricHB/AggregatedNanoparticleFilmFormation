using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ANPaX.IO.DBConnection.Db;
using ANPaX.IO.DTO;
using ANPaX.IO.interfaces;

using Dapper;

namespace ANPaX.IO.DBConnection.Data
{
    public class PrimaryParticleDataSQLite : IPrimaryParticleData
    {
        private readonly IDataAccess _dataAccess;
        private readonly ConnectionData _connectionData;
        private readonly string _tableName = "PrimaryParticleData";

        public PrimaryParticleDataSQLite(IDataAccess dataAccess, ConnectionData connectionData)
        {
            _dataAccess = dataAccess;
            _connectionData = connectionData;

            var da = (SqliteDB)_dataAccess;
            da.CreateDatabaseIfNotExist(GetInitializationProcedure(), GetInitialDataProcedure(), _tableName, _connectionData);
        }

        public async Task<int> CreateAggregateConfiguration(PrimaryParticleDTO primaryParticleDTO)
        {
            var procedure = $@"INSERT INTO {_tableName}
            (
              ParticleSimulationId,
		      AggregateId, 
		      ClusterId, 
		      AggregateConfigurationId, 
		      XCoord, 
		      YCoord, 
		      ZCoord, 
		      Radius
            )
            VALUES
            (
              @ParticleSimulationId,
		      @AggregateId, 
		      @ClusterId, 
		      @AggregateConfigurationId, 
		      @XCoord, 
		      @YCoord, 
		      @ZCoord, 
		      @Radius
            );
            select last_insert_rowid()";

            var p = new DynamicParameters();

            p.Add("ParticleSimulationId", primaryParticleDTO.ParticleSimulationId);
            p.Add("AggregateId", primaryParticleDTO.AggregateId);
            p.Add("ClusterId", primaryParticleDTO.ClusterId);
            p.Add("AggregateConfigurationId", primaryParticleDTO.AggregateConfigurationId);
            p.Add("XCoord", primaryParticleDTO.XCoord);
            p.Add("YCoord", primaryParticleDTO.YCoord);
            p.Add("ZCoord", primaryParticleDTO.ZCoord);
            p.Add("Radius", primaryParticleDTO.Radius);


            var id = await _dataAccess.SaveData(procedure, p, _connectionData);

            return id;
        }

        public async Task<PrimaryParticleDTO> GetPrimaryParticleById(int primaryParticleId)
        {
            var procedure = $@"SELECT *
            FROM {_tableName}
            WHERE Id = {primaryParticleId}";

            var recs = await _dataAccess.LoadData<PrimaryParticleDTO, dynamic>(procedure, new { }, _connectionData);

            return recs.FirstOrDefault();
        }

        public async Task<IEnumerable<PrimaryParticleDTO>> GetPrimaryParticlesOfAggregateOfSimulationById(int particleSimulationId, int aggregateId)
        {
            var procedure = $@"SELECT *
            FROM {_tableName}
            WHERE ParticleSimulationId = {particleSimulationId}
            WHERE AggregateId = {aggregateId}";

            var recs = await _dataAccess.LoadData<PrimaryParticleDTO, dynamic>(procedure, new { }, _connectionData);

            return recs;
        }

        public async Task<IEnumerable<PrimaryParticleDTO>> GetPrimaryParticlesOfSimulationById(int particleSimulationId)
        {
            var procedure = $@"SELECT *
            FROM {_tableName}
            WHERE ParticleSimulationId = {particleSimulationId}";

            var recs = await _dataAccess.LoadData<PrimaryParticleDTO, dynamic>(procedure, new { }, _connectionData);

            return recs;
        }

        private string GetInitializationProcedure()
        {
            return $@"create table {_tableName}
                (
                    Id                                  INTEGER PRIMARY KEY AUTOINCREMENT,
                    ParticleSimulationId                integer not null,
                    AggregateId                         integer,
                    ClusterId                           integer,
                    AggregateConfigurationId            integer not null,
                    XCoord                              numeric not null,
                    YCoord                              numeric not null,
                    ZCoord                              numeric not null,
                    Radius                              numeric not null
                )";
        }

        private string GetInitialDataProcedure()
        {
            return $@"INSERT INTO {_tableName}
            ( ParticleSimulationId, AggregateId, ClusterId, AggregateConfigurationId, XCoord, YCoord, ZCoord, Radius ) VALUES
            ( 1, 1, 1, 1, 0.0, 0.0, 0.0, 1 );
            select last_insert_rowid()";
        }
    }
}
