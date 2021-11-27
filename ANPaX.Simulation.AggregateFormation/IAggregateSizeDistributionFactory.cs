using System;

using ANPaX.Simulation.AggregateFormation.interfaces;

namespace ANPaX.Simulation.AggregateFormation
{
    public interface IAggregateSizeDistributionFactory
    {
        ISizeDistribution<int> Build(Random rndGen, IAggregateFormationConfig config);
    }
}
