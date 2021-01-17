using System;

using ANPaX.Simulation.AggregateFormation.interfaces;

namespace ANPaX.Simulation.AggregateFormation
{
    public class AggregateSizeDistributionFactory : IAggregateSizeDistributionFactory
    {
        public ISizeDistribution<int> Build(Random rndGen, IAggregateFormationConfig config)
        {
            switch (config.AggregateSizeDistributionType)
            {
                case SizeDistributionType.DissDefault:
                default:
                    return DefaultConfigurationBuilder.GetAggreateSizeDistribution(rndGen, config);
            }
        }
    }
}
