using System.Collections.Generic;

using ANPaX.Collection;

namespace ANPaX.AggregateFormation
{
    internal static class AggregateIndexingHelper
    {
        public static void SetAggregateIndices(IEnumerable<Aggregate> aggregates)
        {
            var primaryParticleIndex = 1;
            var clusterIndex = 1;
            var aggregateIndex = 1;
            foreach (var agg in aggregates)
            {
                foreach (var cluster in agg.Cluster)
                {
                    foreach (var pp in cluster.PrimaryParticles)
                    {
                        pp.Id = primaryParticleIndex++;
                    }

                    cluster.Id = clusterIndex++;
                }

                agg.Id = aggregateIndex++;
            }
        }
    }
}
