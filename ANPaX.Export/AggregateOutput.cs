using System.Collections.Generic;

using ANPaX.Collection;

namespace ANPaX.Export
{
    public class AggregateOutput
    {
        public int NumberOfAggregates
        {
            get => Aggregates.Count;
            set { }
        }

        public string Units { get; set; } = "nano";
        public string AggregateFormationMethod { get; set; } = "undefined";
        public string ParticleSizeDistribution { get; set; } = "undefined";
        public string AggregateSizeDistribution { get; set; } = "undefined";
        public int ClusterSize { get; set; } = 0;
        public List<Aggregate> Aggregates { get; set; }

        public AggregateOutput(List<Aggregate> aggregates)
        {
            Aggregates = aggregates;
        }

        public AggregateOutput()
        {

        }


    }
}
