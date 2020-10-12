
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ANPaX.IO.DBConnection.Db;
using ANPaX.IO.DTO;
using ANPaX.IO.interfaces;

using Dapper;

namespace ANPaX.IO.DBConnection.Data
{
    public class AggregateConfigurationDataSQLite : IAggregateConfigurationData
    {
        private readonly IDataAccess _dataAccess;
        private readonly ConnectionData _connectionData;
        private readonly string _tableName = "AggegrateConfiguration";

        public AggregateConfigurationDataSQLite(IDataAccess dataAccess, ConnectionData connectionData)
        {
            _dataAccess = dataAccess;
            _connectionData = connectionData;

            var da = (SqliteDB)_dataAccess;
            da.CreateDatabaseIfNotExist(GetInitializationProcedure(), GetInitialDataProcedure(), _tableName, _connectionData);
        }

        private string GetInitializationProcedure()
        {
            return $@"create table {_tableName}
                (
                    Id                                  INTEGER PRIMARY KEY AUTOINCREMENT,
                    Description                         text not null,
                    TotalPrimaryParticles               integer not null,
                    ClusterSize                         integer not null,
                    Df                                  numeric not null,
                    kf                                  numeric not null,
                    Epsilon                             numeric not null,
                    Delta                               numeric not null,
                    MaxAttemptsPerCluster               integer not null,
                    MaxAttemptsPerAggregate             integer not null,
                    LargeNumber                         numeric not null,
                    RadiusMeanCalculationMethod         text not null,
                    AggregateSizeMeanCalculationMethod  text not null,
                    PrimaryParticleSizeDistribution     text not null,
                    AggregateSizeDistribution           text not null,
                    AggregateFormationFactory           text not null,
                    RandomGeneratorSeed                 numeric not null
                )";
        }

        private string GetInitialDataProcedure()
        {
            return $@"INSERT INTO {_tableName}
            ( [Description], TotalPrimaryParticles, ClusterSize, Df, Kf, Epsilon, Delta, MaxAttemptsPerCluster, MaxAttemptsPerAggregate, LargeNumber, RadiusMeanCalculationMethod, AggregateSizeMeanCalculationMethod, PrimaryParticleSizeDistribution, AggregateSizeDistribution, AggregateFormationFactory, RandomGeneratorSeed ) VALUES
            ( 'Default', 600000, 6, 1.8, 1.30, 1.001, 1.01, 50000, 200000, 10000000,'Geometric', 'Geometric', 'DissDefault', 'DissDefault', 'CCA', -1 );
            select last_insert_rowid()";
        }


        public async Task<int> CreateAggregateConfiguration(AggregateConfigurationModel aggregateConfigurationModel)
        {
            var procedure = $@"INSERT INTO {_tableName}
            ( [Description],
		      TotalPrimaryParticles, 
		      ClusterSize, 
		      Df, 
		      Kf, 
		      Epsilon, 
		      Delta, 
		      MaxAttemptsPerCluster, 
		      MaxAttemptsPerAggregate, 
		      LargeNumber, 
		      RadiusMeanCalculationMethod, 
		      AggregateSizeMeanCalculationMethod, 
		      PrimaryParticleSizeDistribution, 
		      AggregateSizeDistribution, 
		      AggregateFormationFactory, 
		      RandomGeneratorSeed
            )
            VALUES
            (
              @Description,
		      @TotalPrimaryParticles, 
		      @ClusterSize, 
		      @Df, 
		      @Kf, 
		      @Epsilon, 
		      @Delta, 
		      @MaxAttemptsPerCluster, 
		      @MaxAttemptsPerAggregate, 
		      @LargeNumber, 
		      @RadiusMeanCalculationMethod, 
		      @AggregateSizeMeanCalculationMethod, 
		      @PrimaryParticleSizeDistribution, 
		      @AggregateSizeDistribution, 
		      @AggregateFormationFactory, 
		      @RandomGeneratorSeed
            );
            select last_insert_rowid()";

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

            var id = await _dataAccess.SaveData(procedure, p, _connectionData);

            return id;
        }

        public async Task<AggregateConfigurationModel> GetAggregateConfigurationById(int aggregateConfigurationId)
        {
            var procedure = $@"SELECT *
            FROM {_tableName}
            WHERE Id = {aggregateConfigurationId}";

            var recs = await _dataAccess.LoadData<AggregateConfigurationModel, dynamic>(procedure, new { }, _connectionData);

            return recs.FirstOrDefault();
        }

        public async Task<List<AggregateConfigurationModel>> GetAggregateConfigurations()
        {
            var procedure = $@"SELECT *
            FROM {_tableName}";

            return await _dataAccess.LoadData<AggregateConfigurationModel, dynamic>(procedure, new { }, _connectionData);
        }
    }
}
