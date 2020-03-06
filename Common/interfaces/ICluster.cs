using System.Collections.Generic;

namespace Common.interfaces
{
    public interface ICluster
    {
        int NumberOfPrimaryParticles { get; }
        List<PrimaryParticle> PrimaryParticles { get; }
    }
}
