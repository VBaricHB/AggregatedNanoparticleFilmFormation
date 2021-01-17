using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ANPaX.IO.DBConnection.Db;
using ANPaX.IO.DTO;
using ANPaX.IO.interfaces;

using Dapper;

namespace ANPaX.IO.DBConnection.Data
{
    public class ParticleSimulationDataSQLite : IParticleSimulationData
    {
        private readonly IDataAccess _dataAccess;
        private readonly ConnectionData _connectionData;
        private readonly string _tableName = "ParticleSimulationData";

        public ParticleSimulationDataSQLite(IDataAccess dataAccess, ConnectionData connectionData)
        {
            _dataAccess = dataAccess;
            _connectionData = connectionData;

            var da = (SqliteDB)_dataAccess;
            da.CreateDatabaseIfNotExist(GetInitializationProcedure(), GetInitialDataProcedure(), _tableName, _connectionData);
        }

        public async Task<int> CreateParticleSimulation(ParticleSimulationDTO particleSimulation)
        {
            var procedure = $@"INSERT INTO {_tableName}
            ( [Description],
		      User, 
		      Date, 
		      SimulationType, 
		      AggregateFormationConfigId, 
		      FilmFormationConfigId, 
		      Status, 
		      Percentage
            )
            VALUES
            (
              @Description,
		      @User, 
		      @Date, 
		      @SimulationType, 
		      @AggregateFormationConfigId, 
		      @FilmFormationConfigId, 
		      @Status, 
		      @Percentage
            );
            select last_insert_rowid()";

            var p = new DynamicParameters();

            p.Add("Description", particleSimulation.Description);
            p.Add("User", particleSimulation.User);
            p.Add("Date", particleSimulation.Date);
            p.Add("SimulationType", particleSimulation.SimulationType);
            p.Add("AggregateFormationConfigId", particleSimulation.AggregateFormationConfigId);
            p.Add("FilmFormationConfigId", particleSimulation.FilmFormationConfigId);
            p.Add("Status", particleSimulation.Status);
            p.Add("Percentage", particleSimulation.Percentage);

            var id = await _dataAccess.SaveData(procedure, p, _connectionData);

            return id;
        }

        public Task<int> DeleteParticleSimulation(int particleSimulationId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ParticleSimulationDTO> GetParticleSimulationById(int particleSimulationId)
        {
            var procedure = $@"SELECT *
            FROM {_tableName}
            WHERE Id = {particleSimulationId}";

            var recs = await _dataAccess.LoadData<ParticleSimulationDTO, dynamic>(procedure, new { }, _connectionData);

            return recs.FirstOrDefault();
        }

        public async Task<List<ParticleSimulationDTO>> GetParticleSimulationByUser(string user)
        {
            var procedure = $@"SELECT *
            FROM {_tableName}
            WHERE User = {user}";

            var recs = await _dataAccess.LoadData<ParticleSimulationDTO, dynamic>(procedure, new { }, _connectionData);

            return recs.ToList();
        }

        public async Task<List<ParticleSimulationDTO>> GetParticleSimulations()
        {
            var procedure = $@"SELECT *
            FROM {_tableName}";

            return await _dataAccess.LoadData<ParticleSimulationDTO, dynamic>(procedure, new { }, _connectionData);
        }

        public Task<int> UpdateParticleSimulationPercentage(int particleSimulationId, double percentage)
        {
            throw new System.NotImplementedException();
        }

        public Task<int> UpdateParticleSimulationStatus(int particleSimulationId, string status)
        {
            throw new System.NotImplementedException();
        }

        private string GetInitializationProcedure()
        {
            return $@"create table {_tableName}
                (
                    Id                                  INTEGER PRIMARY KEY AUTOINCREMENT,
                    Description                         text not null,
                    User                                text not null,
                    Date                                text not null,
                    SimulationType                      text not null,
                    AggregateFormationConfigId          integer not null,
                    FilmFormationConfigId               integer not null,
                    Status                              text not null,
                    Percentage                          numeric not null
                )";
        }

        private string GetInitialDataProcedure()
        {
            return $@"INSERT INTO {_tableName}
            ( [Description], User, Date, SimulationType, AggregateFormationConfigId, FilmFormationConfigId, Status, Percentage ) VALUES
            ( 'Test', 'anpax', '2020-12-20 20:00:00', 'Full', 1, 1, 'complete', 100 );
            select last_insert_rowid()";
        }
    }
}
