using System;
using System.Collections.Generic;

using ANPaX.Core;

namespace ANPaX.Core.Neighborslist
{
    public interface INeighborslist
    {
        IEnumerable<PrimaryParticle> GetPrimaryParticlesWithinRadius(Vector3 Position, double radius);

        IEnumerable<Tuple<PrimaryParticle, double>> GetPrimaryParticlesAndDistanceWithinRadius(Vector3 Position, double radius);
        void AddParticlesToNeighborsList(Aggregate aggregate);
        void AddParticlesToNeighborsList(Cluster cluster);
        void AddParticlesToNeighborsList(PrimaryParticle primaryParticle);
    }
}
