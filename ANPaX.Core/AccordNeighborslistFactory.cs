using System.Collections.Generic;
using System.Threading.Tasks;

using ANPaX.Core.interfaces;

namespace ANPaX.Core
{
    public class AccordNeighborslistFactory : INeighborslistFactory
    {

        public INeighborslist Build2DNeighborslist(IEnumerable<PrimaryParticle> primaryParticles)
        {
            return new Accord2DNeighborslist(primaryParticles);
        }

        public INeighborslist Build3DNeighborslist(IEnumerable<PrimaryParticle> primaryParticles)
        {
            return new Accord3DNeighborslist(primaryParticles);
        }

        public async Task<INeighborslist> Build3DNeighborslist_Async(IEnumerable<PrimaryParticle> primaryParticles)
        {
            var neighborslist = await Task.Run(() => new Accord3DNeighborslist(primaryParticles));
            return neighborslist;
        }

        public INeighborslist Build3DNeighborslist(IEnumerable<Cluster> cluster)
        {
            return new Accord3DNeighborslist(cluster.GetPrimaryParticles());
        }

        public INeighborslist BuildEmpty3DNeighborsList()
        {
            // The neighborslist requires a particle to be build. Therefore, a dummy particle
            // which can never be reached is added to the neighborslist
            // this particle will never show up in an aggregate
            var dummyPP = new PrimaryParticle(-1, new Vector3(-999999, -999999, -999999), 0);
            return new Accord3DNeighborslist(new List<PrimaryParticle> { dummyPP });
        }
    }
}
