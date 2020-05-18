using System.Collections.Generic;

using ANPaX.Core;

namespace ANPaX.Simulation.FilmFormation.interfaces
{
    public interface IWallCollisionHandler
    {
        void CheckPrimaryParticle(PrimaryParticle primaryParticle);
        void CheckPrimaryParticle(IEnumerable<PrimaryParticle> enumerable);
    }
}
