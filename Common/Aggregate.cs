using System.Collections.Generic;
using System.Linq;
using Common.interfaces;

namespace Common
{
    public class Aggregate : IAggregate
    {
        public Aggregate(List<Cluster> cluster)
        {
            Cluster = cluster; 
        }

        public List<Cluster> Cluster { get; private set; }

        public int NumberOfPrimaryParticles => Cluster.Sum(c => c.PrimaryParticles.Count);
        public int NumberOfClusters => Cluster.Count;
        public bool IsDeposited { get; set; }
    }
}
