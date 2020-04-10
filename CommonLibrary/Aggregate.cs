using System.Collections.Generic;
using System.Linq;
using CommonLibrary.interfaces;

namespace CommonLibrary
{
    public class Aggregate : IAggregate
    {
        public Aggregate(IEnumerable<Cluster> cluster)
        {
            Cluster = cluster; 
        }

        public IEnumerable<Cluster> Cluster { get; private set; }
        public int NumberOfPrimaryParticles => Cluster.Sum(c => c.PrimaryParticles.Count);
        public int NumberOfClusters => Cluster.Count();
        public bool IsDeposited { get; set; }
    }
}
