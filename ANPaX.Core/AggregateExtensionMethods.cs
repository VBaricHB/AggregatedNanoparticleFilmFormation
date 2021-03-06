﻿using System.Collections.Generic;
using System.Linq;

namespace ANPaX.Core
{
    public static class AggregateExtensionMethods
    {
        public static IEnumerable<PrimaryParticle> GetPrimaryParticles(this Aggregate aggregate)
        {
            return aggregate.Cluster.SelectMany(c => c.PrimaryParticles);
        }

        public static IEnumerable<PrimaryParticle> GetPrimaryParticles(this IEnumerable<Aggregate> aggregates)
        {
            return aggregates.SelectMany(a => a.Cluster.SelectMany(c => c.PrimaryParticles));
        }

        public static double GetMaxRadius(this IList<Aggregate> aggregates)
        {
            return aggregates.GetPrimaryParticles().Select(p => p.Radius).Max();
        }

        public static List<List<int>> Split(this List<int> aggregateSizes, int subListSize)
        {
            return aggregateSizes.Select((item1, index) => new { Index = index, Value = item1 })
                .GroupBy(x => x.Index / subListSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        public static int GetNumberOfPrimaryParticles(this IEnumerable<Aggregate> aggregates)
        {
            return aggregates.GetPrimaryParticles().Count();
        }



    }
}
