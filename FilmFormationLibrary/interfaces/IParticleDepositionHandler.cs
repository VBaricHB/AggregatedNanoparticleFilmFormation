using CommonLibrary;
using System.Collections.Generic;

namespace FilmFormationLibrary.interfaces
{
    internal interface ISingleParticleDepositionHandler
    {
        double GetMinDepositionDistance(PrimaryParticle primaryParticle, IEnumerable<PrimaryParticle> primaryParticles);
    }
}