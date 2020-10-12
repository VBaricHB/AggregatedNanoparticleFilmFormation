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

        public Task<List<ParticleSimulationModel>> GetParticleSimulations()
        {
            return _dataAccess.LoadData<ParticleSimulationModel, dynamic>("dbo.spSimulations_All",
                                                                          new { },
                                                                          _connectionString);
        }

        public async Task<int> CreateParticleSimulation(ParticleSimulationModel particleSimulation)
        {
            var p = new DynamicParameters();

            p.Add("Description", particleSimulation.Description);
            p.Add("User", particleSimulation.User);
            p.Add("EMail", particleSimulation.EMail);
            p.Add("PrimaryParticles", particleSimulation.PrimaryParticles);
            p.Add("XWidth", particleSimulation.XWidth);
            p.Add("YWidth", particleSimulation.YWidth);
            p.Add("ZWidth", particleSimulation.ZWidth);
            p.Add("Date", particleSimulation.Date);
            p.Add("SimulationType", particleSimulation.SimulationType);
            p.Add("SimulationPath", particleSimulation.SimulationPath);
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

        public async Task<ParticleSimulationModel> GetParticleSimulationById(int particleSimulationId)
        {
            var recs = await _dataAccess.LoadData<ParticleSimulationModel, dynamic>("dbo.spParticleSimulations_GetbyId",
                                                                                    new { Id = particleSimulationId },
                                                                                    _connectionString);

            return recs.FirstOrDefault();
        }
    }
}
