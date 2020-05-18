using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using ANPaX.Core;
using ANPaX.Core.interfaces;

namespace ANPaX.Simulation.FilmFormation.interfaces
{
    public interface IAggregateDepositionHandler
    {
        Task DepositAggregate_Async(Aggregate aggregate, IEnumerable<PrimaryParticle> depositedPrimaryParticles, INeighborslist neighborslist, double maxRadius, int maxCPU, CancellationToken ct);

        void DepositAggregate(Aggregate aggregate, IEnumerable<PrimaryParticle> depositedPrimaryParticles, INeighborslist neighborslist, double maxRadius);
    }
}
