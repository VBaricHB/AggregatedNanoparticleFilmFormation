using System.Collections.Generic;

using ANPaX.Core;
using ANPaX.Simulation.AggregateFormation.interfaces;

namespace ANPaX.IO
{
    internal static class AggregateOutputMapper
    {
        public static AggregateOutput MapToAggregateOutput(List<Aggregate> aggregates, IAggregateFormationConfig config)
        {
            return MapToAggregateOutput(aggregates, config, true);
        }

        public static AggregateOutput MapToAggregateOutput(List<Aggregate> aggregates, IAggregateFormationConfig config, bool convertToSI)
        {
            var units = "nano";
            if (convertToSI)
            {
                UnitConversionHelper.ConvertDistanceToMeter(aggregates);
                units = "SI";
            }

            var output = new AggregateOutput(aggregates)
            {
                AggregateFormationMethod = GetAggregateFormationMethodString(config),
                ParticleSizeDistribution = GetParticleSizeDistributionString(config),
                AggregateSizeDistribution = GetAggregateSizeDistributionString(config),
                ClusterSize = config.ClusterSize,
                Units = units
            };

            return output;
        }

        private static string GetAggregateSizeDistributionString(IAggregateFormationConfig config)
        {
            var asdString = "FSP Default";
            if (!config.UseDefaultGenerationMethods)
            {
                asdString = config.AggregateSizeDistribution.ToString();
            }

            return asdString;
        }

        private static string GetParticleSizeDistributionString(IAggregateFormationConfig config)
        {
            var psdString = "FSP Default";
            if (!config.UseDefaultGenerationMethods)
            {
                psdString = config.PrimaryParticleSizeDistribution.ToString();
            }

            return psdString;
        }

        private static string GetAggregateFormationMethodString(IAggregateFormationConfig config)
        {
            var aggregateFormationFactoryString = "Cluster Cluster Aggregation";
            if (!config.UseDefaultGenerationMethods)
            {
                aggregateFormationFactoryString = config.AggregateFormationFactory.ToString();
            }

            return aggregateFormationFactoryString;
        }
    }
}
