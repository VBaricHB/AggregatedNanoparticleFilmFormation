using System.Collections.Generic;

namespace CommonLibrary.interfaces
{
    public interface ICluster
    {
        int NumberOfPrimaryParticles { get; }
        List<PrimaryParticle> PrimaryParticles { get; }
    }
}
