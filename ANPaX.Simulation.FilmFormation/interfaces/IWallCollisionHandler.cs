using System.Collections.Generic;

using ANPaX.Core;
using ANPaX.Core.ParticleFilm.interfaces;

namespace ANPaX.Simulation.FilmFormation.interfaces
{
    public interface IWallCollisionHandler
    {
        void CheckPrimaryParticle(PrimaryParticle primaryParticle, ISimulationBox simulationBox);
        void CheckPrimaryParticle(IEnumerable<PrimaryParticle> enumerable, ISimulationBox simulationBox);
    }
}
