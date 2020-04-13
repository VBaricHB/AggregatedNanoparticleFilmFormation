using ANPaX.Collection;
using System.Collections.Generic;

namespace ANPaX.FilmFormation.interfaces
{
    public interface IWallCollisionHandler
    {
        void CheckPrimaryParticle(PrimaryParticle primaryParticle);
        void CheckPrimaryParticle(IEnumerable<PrimaryParticle> enumerable);
    }
}