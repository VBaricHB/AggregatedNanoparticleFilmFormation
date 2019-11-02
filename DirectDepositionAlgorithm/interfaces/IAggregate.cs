using System.Collections.Generic;

namespace DirectDepositionAlgorithm.interfaces
{
    public interface IAggregate
    {
        List<PrimaryParticle> PrimaryParticles { get; }
        double RadiusGyration { get; set; }
        int NumberOfPrimaryParticles { get; set; }
        int NumberOfClusters { get; set; }
        bool IsDeposited { get; set; }
    }
}
