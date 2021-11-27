using ANPaX.Core;
using ANPaX.Simulation.AggregateFormation;
using ANPaX.Simulation.AggregateFormation.interfaces;

namespace ANPaX.UI.DesktopUI.Models
{
    public class AggregateFormationConfig : IAggregateFormationConfig
    {
        private bool _useDefaultGenerationMethods = true;

        public double NeighborAddDistance { get; set; } = 1.0;

        public double Epsilon { get; set; } = 1.001;

        public double Delta { get; set; } = 1.01;

        public double Df { get; set; } = 1.80;

        public double Kf { get; set; } = 1.30;

        public int MaxAttemptsPerCluster { get; set; } = 50000;

        public int MaxAttemptsPerAggregate { get; set; } = 200000;

        public double LargeNumber { get; set; } = 1e10;

        public int ClusterSize { get; set; } = 6;

        public int TotalPrimaryParticles { get; set; } = 5000;

        public MeanMethod RadiusMeanCalculationMethod { get; set; } = MeanMethod.Geometric;

        public MeanMethod AggregateSizeMeanCalculationMethod { get; set; } = MeanMethod.Geometric;

        public bool UseDefaultGenerationMethods
        {
            get
            {
                return _useDefaultGenerationMethods;
            }
            set
            {
            }
        }

        public ISizeDistribution<double> PrimaryParticleSizeDistribution { get; set; } = null;

        public ISizeDistribution<int> AggregateSizeDistribution { get; set; } = null;

        public IParticleFactory<Aggregate> AggregateFormationFactory { get; set; } = null;

        public int RandomGeneratorSeed { get; set; } = -1;
        public SizeDistributionType AggregateSizeDistributionType { get; set; } = SizeDistributionType.DissDefault;
        public SizeDistributionType PrimaryParticleSizeDistributionType { get; set; } = SizeDistributionType.DissDefault;
        public AggregateFormationType AggregateFormationType { get; set; } = AggregateFormationType.ClusterClusterAggregation;
        public double ModalRadius { get; set; } = 4.479;
        public double StdPPRadius { get; set; } = 0.230;
    }
}
