using ANPaX.Core;
using ANPaX.Core.interfaces;
using ANPaX.Simulation.AggregateFormation.interfaces;

using NLog;

namespace ANPaX.Simulation.AggregateFormation
{
    public class AggregateFormationFactory : IAggregateFormationFactory
    {
        public IParticleFactory<Aggregate> Build(ISizeDistribution<double> psd, INeighborslistFactory neighborslistFactory, IAggregateFormationConfig config, ILogger logger)
        {
            switch (config.AggregateFormationType)
            {
                case AggregateFormationType.ClusterClusterAggregation:
                default:
                    return new ClusterClusterAggregationFactory(psd, config, logger, neighborslistFactory, config.RandomGeneratorSeed);
            }
        }
    }
}
