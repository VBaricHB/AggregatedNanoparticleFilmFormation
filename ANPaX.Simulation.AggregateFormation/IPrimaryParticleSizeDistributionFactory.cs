using System;

using ANPaX.Simulation.AggregateFormation.interfaces;

namespace ANPaX.Simulation.AggregateFormation
{
    public interface IPrimaryParticleSizeDistributionFactory
    {
        ISizeDistribution<double> Build(Random rndGen, IAggregateFormationConfig config);
    }
}
