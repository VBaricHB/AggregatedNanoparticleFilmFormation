namespace ANPaX.Simulation.AggregateFormation.interfaces
{
    public interface IAggregateFormationConfig
    {
        public int TotalPrimaryParticles { get; }
        public int ClusterSize { get; }
        public double Df { get; }
        public double Kf { get; }
        public double Epsilon { get; }
        public double Delta { get; }
        public int MaxAttemptsPerCluster { get; }
        public int MaxAttemptsPerAggregate { get; }
        public double LargeNumber { get; }
        public MeanMethod RadiusMeanCalculationMethod { get; }
        public MeanMethod AggregateSizeMeanCalculationMethod { get; }
        public SizeDistributionType AggregateSizeDistributionType { get; set; }
        public SizeDistributionType PrimaryParticleSizeDistributionType { get; set; }
        public AggregateFormationType AggregateFormationType { get; set; }
        public bool UseDefaultGenerationMethods { get; }
        public int RandomGeneratorSeed { get; }
        public double MeanPPRadius { get; }
        public double StdPPRadius { get; }
    }
}
