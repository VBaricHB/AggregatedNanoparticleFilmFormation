using CommonLibrary;
using System.Collections.Generic;

namespace FilmFormationLibrary.interfaces
{
    public interface IAggregateDepositionHandler
    {
        void DepositAggregate(Aggregate aggregate, IEnumerable<PrimaryParticle> depositedPrimaryParticles);
    }
}