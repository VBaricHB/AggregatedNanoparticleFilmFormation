using System.Collections.Generic;

using ANPaX.Collection;
using ANPaX.Core.Neighborslist;

namespace ANPaX.FilmFormation.interfaces
{
    public interface IAggregateDepositionHandler
    {
        void DepositAggregate(Aggregate aggregate, IEnumerable<PrimaryParticle> depositedPrimaryParticles, INeighborslist neighborslist);
    }
}
