using System.Collections.Generic;
using System.Linq;

using ANPaX.Core;

namespace ANPaX.IO.Export
{
    internal static class UnitConversionHelper
    {
        public static void ConvertDistanceToMeter(IEnumerable<Aggregate> aggregates)
        {
            if (IsInMeter(aggregates.First()))
            {
                return;
            }

            foreach (var agg in aggregates)
            {
                foreach (var cluster in agg.Cluster)
                {
                    foreach (var pp in cluster.PrimaryParticles)
                    {
                        pp.Radius *= 1e-9;
                        pp.Position.X *= 1e-9;
                        pp.Position.Y *= 1e-9;
                        pp.Position.Z *= 1e-9;
                    }
                }
            }
        }

        private static bool IsInMeter(Aggregate aggregate)
        {
            return aggregate.Cluster.First().PrimaryParticles.First().Radius < 1e-3;
        }
    }
}
