using System.Collections.Generic;

namespace Common.interfaces
{
    public interface ICluster
    {
        int NumberOfPrimaryParticles { get; }
        IEnumerable<PrimaryParticle> PrimaryParticles { get; }
    }
}
