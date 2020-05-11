using System.Collections.Generic;

using ANPaX.Collection;

namespace ANPaX.Core.Neighborslist
{
    public interface INeighborslistFactory
    {
        INeighborslist Build2DNeighborslist(IEnumerable<PrimaryParticle> primaryParticles);
        INeighborslist Build3DNeighborslist(IEnumerable<PrimaryParticle> primaryParticles);
        INeighborslist Build3DNeighborslist(IEnumerable<Cluster> cluster);
        INeighborslist BuildEmpty3DNeighborsList();
    }
}
