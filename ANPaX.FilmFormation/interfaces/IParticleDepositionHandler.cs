using ANPaX.Collection;
using System.Collections.Generic;

namespace ANPaX.FilmFormation.interfaces
{
    internal interface ISingleParticleDepositionHandler
    {
        double GetMinDepositionDistance(PrimaryParticle primaryParticle, IEnumerable<PrimaryParticle> primaryParticles);
    }
}