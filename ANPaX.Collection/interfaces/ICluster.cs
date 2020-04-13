using System.Collections.Generic;

namespace ANPaX.Collection.interfaces
{
    public interface ICluster
    {
        int NumberOfPrimaryParticles { get; }
        List<PrimaryParticle> PrimaryParticles { get; }
    }
}
