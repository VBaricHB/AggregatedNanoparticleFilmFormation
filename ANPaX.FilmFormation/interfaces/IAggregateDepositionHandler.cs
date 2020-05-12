using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ANPaX.Core;
using ANPaX.Core.Neighborslist;

namespace ANPaX.FilmFormation.interfaces
{
    public interface IAggregateDepositionHandler
    {
        Task DepositAggregate_Async(Aggregate aggregate, IEnumerable<PrimaryParticle> depositedPrimaryParticles, INeighborslist neighborslist, double maxRadius, int maxCPU, CancellationToken ct);
    }
}
