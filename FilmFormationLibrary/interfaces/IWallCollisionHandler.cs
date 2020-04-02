using CommonLibrary;
using System.Collections.Generic;

namespace FilmFormationLibrary.interfaces
{
    public interface IWallCollisionHandler
    {
        void CheckPrimaryParticle(PrimaryParticle primaryParticle);
        void CheckPrimaryParticle(IEnumerable<PrimaryParticle> enumerable);
    }
}