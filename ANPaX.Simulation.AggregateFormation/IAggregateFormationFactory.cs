using ANPaX.Core;
using ANPaX.Core.interfaces;
using ANPaX.Simulation.AggregateFormation.interfaces;

using NLog;

namespace ANPaX.Simulation.AggregateFormation
{
    public interface IAggregateFormationFactory
    {
        IParticleFactory<Aggregate> Build(ISizeDistribution<double> psd, INeighborslistFactory neighborslistFactory, IAggregateFormationConfig config, ILogger logger);
    }
}