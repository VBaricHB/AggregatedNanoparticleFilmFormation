using ANPaX.Collection;
using System.Collections.Generic;

namespace ANPaX.FilmFormation.interfaces
{
    public interface IAggregateDepositionHandler
    {
        void DepositAggregate(Aggregate aggregate, IEnumerable<PrimaryParticle> depositedPrimaryParticles);
    }
}