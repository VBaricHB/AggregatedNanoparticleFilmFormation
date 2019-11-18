using System.Collections.Generic;

namespace Common.interfaces
{
    public interface IAggregate
    {
        List<ICluster> Cluster { get; }
        int NumberOfPrimaryParticles { get; set; }
        int NumberOfClusters { get; set; }
        bool IsDeposited { get; set; }
    }
}
