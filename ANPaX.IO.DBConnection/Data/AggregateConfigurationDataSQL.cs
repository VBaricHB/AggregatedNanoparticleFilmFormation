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
    public class AggregateConfigurationDataSQL : IAggregateConfigurationData
    {
        private readonly IDataAccess _dataAccess;
        private readonly ConnectionData _connectionData;

        public AggregateConfigurationDataSQL(IDataAccess dataAccess, ConnectionData connectionData)
        {
            _dataAccess = dataAccess;
            _connectionData = connectionData;
        }

        public async Task<AggregateConfigurationModel> GetAggregateConfigurationById(int aggregateConfigurationId)
        {
            var recs = await _dataAccess.LoadData<AggregateConfigurationModel, dynamic>("dbo.spAggConfig_GetById",
                                                                                  new { Id = aggregateConfigurationId },
                                                                                  _connectionData);

            return recs.FirstOrDefault();

        }

        public async Task<int> CreateAggregateConfiguration(AggregateConfigurationModel aggregateConfigurationModel)
        {
            var p = new DynamicParameters();

            p.Add("Description", aggregateConfigurationModel.Description);
            p.Add("TotalPrimaryParticles", aggregateConfigurationModel.TotalPrimaryParticles);
            p.Add("ClusterSize", aggregateConfigurationModel.ClusterSize);
            p.Add("Df", aggregateConfigurationModel.Df);
            p.Add("Kf", aggregateConfigurationModel.Kf);
            p.Add("Epsilon", aggregateConfigurationModel.Epsilon);
            p.Add("Delta", aggregateConfigurationModel.Delta);
            p.Add("MaxAttemptsPerCluster", aggregateConfigurationModel.MaxAttemptsPerCluster);
            p.Add("MaxAttemptsPerAggregate", aggregateConfigurationModel.MaxAttemptsPerAggregate);
            p.Add("LargeNumber", aggregateConfigurationModel.LargeNumber);
            p.Add("RadiusMeanCalculationMethod", aggregateConfigurationModel.RadiusMeanCalculationMethod);
            p.Add("AggregateSizeMeanCalculationMethod", aggregateConfigurationModel.AggregateSizeMeanCalculationMethod);
            p.Add("PrimaryParticleSizeDistribution", aggregateConfigurationModel.PrimaryParticleSizeDistribution);
            p.Add("AggregateSizeDistribution", aggregateConfigurationModel.AggregateSizeDistribution);
            p.Add("AggregateFormationFactory", aggregateConfigurationModel.AggregateFormationFactory);
            p.Add("RandomGeneratorSeed", aggregateConfigurationModel.RandomGeneratorSeed);
            p.Add("Id", DbType.Int32, direction: ParameterDirection.Output);

            await _dataAccess.SaveData("dbo.spAggConfig_Insert", p, _connectionData);

            return p.Get<int>("Id");
        }

        public Task<List<AggregateConfigurationModel>> GetAggregateConfigurations()
        {
            return _dataAccess.LoadData<AggregateConfigurationModel, dynamic>("dbo.spAggConfig_All",
                                                                          new { },
                                                                          _connectionData);
        }
    }
}
