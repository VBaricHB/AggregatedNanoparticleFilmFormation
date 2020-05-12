using System.Collections.Generic;

namespace ANPaX.Core.interfaces
{
    public interface ICluster
    {
        int NumberOfPrimaryParticles { get; }
        List<PrimaryParticle> PrimaryParticles { get; }
    }
}
