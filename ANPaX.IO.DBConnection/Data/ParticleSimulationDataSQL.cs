using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using ANPaX.IO.DBConnection.Db;
using ANPaX.IO.DTO;
using ANPaX.IO.interfaces;

using Dapper;

namespace ANPaX.IO.DBConnection.Data
{
    public class ParticleSimulationDataSQL : IParticleSimulationData
    {
        private readonly IDataAccess _dataAccess;
        private readonly ConnectionData _connectionString;

        public ParticleSimulationDataSQL(IDataAccess dataAccess, ConnectionData connectionString)
        {
            _dataAccess = dataAccess;
            _connectionString = connectionString;
        }

        public Task<List<ParticleSimulationDTO>> GetParticleSimulations()
        {
            return _dataAccess.LoadData<ParticleSimulationDTO, dynamic>("dbo.spSimulations_All",
                                                                          new { },
                                                                          _connectionString);
        }

        public async Task<int> CreateParticleSimulation(ParticleSimulationDTO particleSimulation)
        {
            var p = new DynamicParameters();

            p.Add("Description", particleSimulation.Description);
            p.Add("User", particleSimulation.User);
            p.Add("Date", particleSimulation.Date);
            p.Add("SimulationType", particleSimulation.SimulationType);
            p.Add("AggregateFormationConfigId", particleSimulation.AggregateFormationConfigId);
            p.Add("FilmFormationConfigId", particleSimulation.FilmFormationConfigId);
            p.Add("Percentage", particleSimulation.Percentage);
            p.Add("Id", DbType.Int32, direction: ParameterDirection.Output);

            await _dataAccess.SaveData("dbo.spParticleSimulations_Insert", p, _connectionString);

            return p.Get<int>("Id");
        }

        public Task<int> UpdateParticleSimulationStatus(int particleSimulationId, string status)
        {
            return _dataAccess.SaveData("dbo.spParticleSimulations_UpdateStatus",
                                        new
                                        {
                                            Id = particleSimulationId,
                                            Status = status
                                        },
                                        _connectionString);
        }

        public Task<int> UpdateParticleSimulationPercentage(int particleSimulationId, double percentage)
        {
            return _dataAccess.SaveData("dbo.spParticleSimulations_UpdatePercentage",
                                        new
                                        {
                                            Id = particleSimulationId,
                                            Percentage = percentage
                                        },
                                        _connectionString);
        }

        public Task<int> DeleteParticleSimulation(int particleSimulationId)
        {
            return _dataAccess.SaveData("dbo.spParticleSimulations_Delete",
                                        new
                                        {
                                            Id = particleSimulationId
                                        },
                                        _connectionString);
        }

        public async Task<ParticleSimulationDTO> GetParticleSimulationById(int particleSimulationId)
        {
            var recs = await _dataAccess.LoadData<ParticleSimulationDTO, dynamic>("dbo.spParticleSimulations_GetbyId",
                                                                                    new { Id = particleSimulationId },
                                                                                    _connectionString);

            return recs.FirstOrDefault();
        }

        public async Task<List<ParticleSimulationDTO>> GetParticleSimulationByUser(string user)
        {
            var recs = await _dataAccess.LoadData<ParticleSimulationDTO, dynamic>("dbo.spSimulations_All",
                                                                          new { },
                                                                          _connectionString);
            return recs.Where(r => r.User == user).ToList();
        }
    }
}
