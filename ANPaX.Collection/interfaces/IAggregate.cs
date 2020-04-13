using System.Collections.Generic;

namespace ANPaX.Collection.interfaces
{
    public interface IAggregate
    {
        IEnumerable<Cluster> Cluster { get; }
        int NumberOfPrimaryParticles { get; }
        int NumberOfClusters { get;  }
        bool IsDeposited { get; set; }
    }
}
