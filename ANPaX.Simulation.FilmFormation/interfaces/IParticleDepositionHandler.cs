using System.Collections.Generic;

using ANPaX.Core;
using ANPaX.Core.interfaces;

namespace ANPaX.Simulation.FilmFormation.interfaces
{
    internal interface ISingleParticleDepositionHandler
    {
        double GetDepositionDistance(
            PrimaryParticle primaryParticle,
            IEnumerable<PrimaryParticle> depositedPrimaryParticles,
            INeighborslist neighborsList,
            double maxRadius);
    }
}
