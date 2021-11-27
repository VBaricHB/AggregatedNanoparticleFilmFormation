using System.Linq;

using ANPaX.IO.DTO;

namespace ANPaX.IO.DBConnection.Data
{
    public static class DbInitializer
    {
        public static void Initialize(DataContext context)
        {
            context.Database.EnsureCreated();

            // Look for any aggconfigs.
            if (context.AggConfigs.Any())
            {
                return;   // DB has been seeded
            }

            var aggConfigs = new AggregateConfigurationDTO[]
            {
                new AggregateConfigurationDTO
                {
                    Description = "Default",
                    TotalPrimaryParticles = 600000,
                    ClusterSize = 6,
                    Df = 1.8,
                    Kf = 1.3,
                    Epsilon = 1.001,
                    Delta = 1.01,
                    MaxAttemptsPerCluster = 50000,
                    MaxAttemptsPerAggregate = 200000,
                    LargeNumber = 10000000,
                    RadiusMeanCalculationMethod = "Geometric",
                    AggregateSizeMeanCalculationMethod = "Geometric",
                    PrimaryParticleSizeDistributionType = "DissDefault",
                    AggregateSizeDistributionType = "DissDefault",
                    AggregateFormationType = "ClusterClusterAggregation",
                    RandomGeneratorSeed = -1
                }
            };

            context.AggConfigs.AddRange(aggConfigs);
            context.SaveChanges();

            var filmConfigs = new FilmFormationConfigurationDTO[]
            {
                new FilmFormationConfigurationDTO
                {
                    Description = "Default",
                    XWidth = 2000,
                    YWidth = 2000,
                    ZWidth = 2000,
                    MaxCPU = 4,
                    LargeNumber = 10000000,
                    Delta = 1.01,
                    DepositionMethod = "Ballistic",
                    WallCollisionMethod = "Periodic",
                    NeighborslistMethod = "Accord"
                }
            };

            context.FilmFormationConfigs.AddRange(filmConfigs);
            context.SaveChanges();
        }
    }
}
