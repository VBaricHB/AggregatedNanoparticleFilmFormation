
using ANPaX.AggregateFormation.interfaces;
using ANPaX.Core;

namespace ANPaX.AggregateFormation.tests
{
    public class TestAggregateFormationConfig : IAggregateFormationConfig
    {
        public TestAggregateFormationConfig()
        {
            TotalPrimaryParticles = 1000;
        }

        public TestAggregateFormationConfig(int primaryParticles)
        {
            TotalPrimaryParticles = primaryParticles;
        }

        public double Epsilon => 1.001;

        public double Delta => 1.01;

        public double Df => 1.8;

        public double Kf => 1.30;

        public double NeighborAddDistance => 1.0;
        public int MaxAttemptsPerCluster => 50000;

        public int MaxAttemptsPerAggregate => 200000;

        public double LargeNumber => 1e10;

        public int TotalPrimaryParticles { get; }

        public int ClusterSize => 6;

        public MeanMethod RadiusMeanCalculationMethod => MeanMethod.Geometric;
        public MeanMethod AggregateSizeMeanCalculationMethod => MeanMethod.Geometric;

        public bool UseDefaultGenerationMethods => true;

        public ISizeDistribution<double> PrimaryParticleSizeDistribution => null;

        public ISizeDistribution<int> AggregateSizeDistribution => null;

        public IParticleFactory<Aggregate> AggregateFormationFactory => null;

        public int RandomGeneratorSeed { get; } = 100;
    }
}
