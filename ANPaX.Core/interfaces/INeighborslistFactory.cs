using System.Collections.Generic;
using System.Threading.Tasks;

namespace ANPaX.Core.interfaces
{
    public interface INeighborslistFactory
    {
        INeighborslist Build2DNeighborslist(IEnumerable<PrimaryParticle> primaryParticles);
        INeighborslist Build3DNeighborslist(IEnumerable<PrimaryParticle> primaryParticles);
        Task<INeighborslist> Build3DNeighborslist_Async(IEnumerable<PrimaryParticle> primaryParticles);
        INeighborslist Build3DNeighborslist(IEnumerable<Cluster> cluster);
        INeighborslist BuildEmpty3DNeighborsList();
    }
}
