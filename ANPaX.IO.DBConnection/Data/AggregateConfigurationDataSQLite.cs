
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
                    User                                text not null,
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
                    PrimaryParticleSizeDistributionType text not null,
                    AggregateSizeDistributionType       text not null,
                    AggregateFormationType              text not null,
                    RandomGeneratorSeed                 numeric not null
                )";
        }

        private string GetInitialDataProcedure()
        {
            return $@"INSERT INTO {_tableName}
            ( [Description], User, TotalPrimaryParticles, ClusterSize, Df, Kf, Epsilon, Delta, MaxAttemptsPerCluster, MaxAttemptsPerAggregate, LargeNumber, RadiusMeanCalculationMethod, AggregateSizeMeanCalculationMethod, PrimaryParticleSizeDistributionType, AggregateSizeDistributionType, AggregateFormationType, RandomGeneratorSeed ) VALUES
            ( 'Default','ANPaX', 600000, 6, 1.8, 1.30, 1.001, 1.01, 50000, 200000, 10000000,'Geometric', 'Geometric', 'DissDefault', 'DissDefault', 'ClusterClusterAggregation', -1 );
            select last_insert_rowid()";
        }


        public async Task<int> CreateAggregateConfiguration(AggregateConfigurationDTO aggregateConfigurationDTO)
        {
            var procedure = $@"INSERT INTO {_tableName}
            ( [Description],
              User,
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
		      PrimaryParticleSizeDistributionType, 
		      AggregateSizeDistributionType, 
		      AggregateFormationType, 
		      RandomGeneratorSeed
            )
            VALUES
            (
              @Description,
              @User,
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
		      @PrimaryParticleSizeDistributionType, 
		      @AggregateSizeDistributionType, 
		      @AggregateFormationType, 
		      @RandomGeneratorSeed
            );
            select last_insert_rowid()";

            var p = new DynamicParameters();

            p.Add("Description", aggregateConfigurationDTO.Description);
            p.Add("User", aggregateConfigurationDTO.User);
            p.Add("TotalPrimaryParticles", aggregateConfigurationDTO.TotalPrimaryParticles);
            p.Add("ClusterSize", aggregateConfigurationDTO.ClusterSize);
            p.Add("Df", aggregateConfigurationDTO.Df);
            p.Add("Kf", aggregateConfigurationDTO.Kf);
            p.Add("Epsilon", aggregateConfigurationDTO.Epsilon);
            p.Add("Delta", aggregateConfigurationDTO.Delta);
            p.Add("MaxAttemptsPerCluster", aggregateConfigurationDTO.MaxAttemptsPerCluster);
            p.Add("MaxAttemptsPerAggregate", aggregateConfigurationDTO.MaxAttemptsPerAggregate);
            p.Add("LargeNumber", aggregateConfigurationDTO.LargeNumber);
            p.Add("RadiusMeanCalculationMethod", aggregateConfigurationDTO.RadiusMeanCalculationMethod);
            p.Add("AggregateSizeMeanCalculationMethod", aggregateConfigurationDTO.AggregateSizeMeanCalculationMethod);
            p.Add("PrimaryParticleSizeDistributionType", aggregateConfigurationDTO.PrimaryParticleSizeDistributionType);
            p.Add("AggregateSizeDistributionType", aggregateConfigurationDTO.AggregateSizeDistributionType);
            p.Add("AggregateFormationType", aggregateConfigurationDTO.AggregateFormationType);
            p.Add("RandomGeneratorSeed", aggregateConfigurationDTO.RandomGeneratorSeed);

            var id = await _dataAccess.SaveData(procedure, p, _connectionData);

            return id;
        }

        public async Task<AggregateConfigurationDTO> GetAggregateConfigurationById(int aggregateConfigurationId)
        {
            var procedure = $@"SELECT *
            FROM {_tableName}
            WHERE Id = {aggregateConfigurationId}";

            var recs = await _dataAccess.LoadData<AggregateConfigurationDTO, dynamic>(procedure, new { }, _connectionData);

            return recs.FirstOrDefault();
        }

        public async Task<List<AggregateConfigurationDTO>> GetAggregateConfigurations()
        {
            var procedure = $@"SELECT *
            FROM {_tableName}";

            return await _dataAccess.LoadData<AggregateConfigurationDTO, dynamic>(procedure, new { }, _connectionData);
        }
    }
}
