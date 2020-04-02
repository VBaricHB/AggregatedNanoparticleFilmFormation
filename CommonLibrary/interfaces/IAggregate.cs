using System.Collections.Generic;

namespace CommonLibrary.interfaces
{
    public interface IAggregate
    {
        List<Cluster> Cluster { get; }
        int NumberOfPrimaryParticles { get; }
        int NumberOfClusters { get;  }
        bool IsDeposited { get; set; }
    }
}
