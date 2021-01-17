using System;

using ANPaX.Simulation.AggregateFormation.interfaces;

namespace ANPaX.Simulation.AggregateFormation
{
    public class PrimaryParticleSizeDistributionFactory : IPrimaryParticleSizeDistributionFactory
    {
        public ISizeDistribution<double> Build(Random rndGen, IAggregateFormationConfig config)
        {
            switch (config.PrimaryParticleSizeDistributionType)
            {
                case SizeDistributionType.DissDefault:
                default:
                    return DefaultConfigurationBuilder.GetPrimaryParticleSizeDistribution(rndGen, config);
            }
        }
    }
}
