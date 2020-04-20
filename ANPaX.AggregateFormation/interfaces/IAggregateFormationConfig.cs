using ANPaX.Collection;

namespace ANPaX.AggregateFormation.interfaces
{
    public interface IAggregateFormationConfig
    {
        double NeighborAddDistance { get; }
        public double Epsilon { get; }
        public double Delta { get; }
        public double Df { get; }
        public double Kf { get; }
        public int MaxAttemptsPerCluster { get; }
        public int MaxAttemptsPerAggregate { get; }
        public double LargeNumber { get; }
        public int ClusterSize { get; }
        public int TotalPrimaryParticles { get; }
        public MeanMethod RadiusMeanCalculationMethod { get; }
        public MeanMethod AggregateSizeMeanCalculationMethod { get; }
        public bool UseDefaultGenerationMethods { get; }
        public ISizeDistribution<double> PrimaryParticleSizeDistribution { get; }
        public ISizeDistribution<int> AggregateSizeDistribution { get; }
        public IParticleFactory<Aggregate> AggregateFormationFactory { get; }
        public int RandomGeneratorSeed { get; }

    }
}
