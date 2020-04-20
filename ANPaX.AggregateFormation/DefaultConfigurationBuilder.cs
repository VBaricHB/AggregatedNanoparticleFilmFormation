﻿using System;
using System.IO;

using ANPaX.AggregateFormation.interfaces;

namespace ANPaX.AggregateFormation
{
    internal static class DefaultConfigurationBuilder
    {
        public static ISizeDistribution<double> GetPrimaryParticleSizeDistribution(Random rndGen, IAggregateFormationConfig config)
        {
            var resources = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\..\\ANPaX.AggregateFormation\\Resources\\"));
            var filePSD = resources + "FSP_PrimaryParticleSizeDistribution.xml";
            var dist = XMLSizeDistributionBuilder<double>.Read(filePSD);
            var psd = new TabulatedPrimaryParticleSizeDistribution(dist, rndGen, config, integrate: true);
            return psd;
        }

        public static ISizeDistribution<int> GetAggreateSizeDistribution(Random rndGen, IAggregateFormationConfig config)
        {
            var resources = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\..\\ANPaX.AggregateFormation\\Resources\\"));
            var fileASD = resources + "FSP_AggregateSizeDistribution.xml";
            var distASD = XMLSizeDistributionBuilder<int>.Read(fileASD);
            var asd = new TabulatedAggregateSizeDistribution(distASD, rndGen, config, integrate: true);
            return asd;
        }
    }
}
