using ANPaX.IO.DTO;

namespace ANPaX.WebUI.Data
{
    public class StaticANPaXAPIHandler : IANPaXAPIHandler
    {
        public AggregateConfigurationDTO GetAggregateConfigurationById(int id)
        {
            return new AggregateConfigurationDTO
            {
                Description = DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                TotalPrimaryParticles = 10000,
                ClusterSize = 6,
                Df = 1.8,
                Kf = 1.3,
                Epsilon = 1.001,
                Delta = 1.01,
                MaxAttemptsPerCluster = 100000,
                MaxAttemptsPerAggregate = 100000,
                LargeNumber = 10000000,
                RadiusMeanCalculationMethod = "Geometric",
                AggregateSizeMeanCalculationMethod = "Geometric",
                PrimaryParticleSizeDistributionType = "DissDefault",
                AggregateSizeDistributionType = "DissDefault",
                AggregateFormationType = "ClusterClusterAggregation",
                RandomGeneratorSeed = -1
            };
        }
    }
}
