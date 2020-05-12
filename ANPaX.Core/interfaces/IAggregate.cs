using System.Collections.Generic;

namespace ANPaX.Core.interfaces
{
    public interface IAggregate
    {
        List<Cluster> Cluster { get; }
        int NumberOfPrimaryParticles { get; }
        int NumberOfClusters { get; }
        bool IsDeposited { get; set; }
    }
}
