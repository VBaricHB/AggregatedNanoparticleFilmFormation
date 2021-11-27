using ANPaX.Simulation.AggregateFormation;
using ANPaX.Simulation.AggregateFormation.interfaces;

namespace ANPaX.Backend.Models
{
    public class AggregateFormationConfig : IAggregateFormationConfig
    {
        public int TotalPrimaryParticles { get; set; }
        public int ClusterSize { get; set; }
        public double Df { get; set; }
        public double Kf { get; set; }
        public double Epsilon { get; set; }
        public double Delta { get; set; }
        public int MaxAttemptsPerCluster { get; set; }
        public int MaxAttemptsPerAggregate { get; set; }
        public double LargeNumber { get; set; }
        public MeanMethod RadiusMeanCalculationMethod { get; set; }
        public MeanMethod AggregateSizeMeanCalculationMethod { get; set; }
        public bool UseDefaultGenerationMethods { get; set; }
        public SizeDistributionType AggregateSizeDistributionType { get; set; }
        public SizeDistributionType PrimaryParticleSizeDistributionType { get; set; }
        public AggregateFormationType AggregateFormationType { get; set; }
        public int RandomGeneratorSeed { get; set; }
    }
}
