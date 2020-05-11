using System.Collections.Generic;

using ANPaX.Collection;
using ANPaX.Core.Neighborslist;

namespace ANPaX.FilmFormation.interfaces
{
    internal interface ISingleParticleDepositionHandler
    {
        double GetMinDepositionDistance(PrimaryParticle primaryParticle, IEnumerable<PrimaryParticle> depositedPrimaryParticles, INeighborslist neighborsList);
    }
}
