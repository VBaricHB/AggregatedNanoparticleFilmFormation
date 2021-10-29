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
                case SizeDistributionType.Monodisperse:
                    return new MonodispersePrimaryParticleSizeDistribution(config.MeanPPRadius);
                case SizeDistributionType.LogNormal:
                    return new LogNormalSizeDistribution(rndGen, config);
                case SizeDistributionType.DissDefault:
                default:
                    return DefaultConfigurationBuilder.GetPrimaryParticleSizeDistribution(rndGen, config);
            }
        }
    }
}
