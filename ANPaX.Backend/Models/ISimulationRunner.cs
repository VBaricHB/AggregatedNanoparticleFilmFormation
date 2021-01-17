using System.Collections.Generic;
using System.Threading.Tasks;

using ANPaX.Core;
using ANPaX.Core.interfaces;

namespace ANPaX.Backend.Models
{
    public interface ISimulationRunner
    {
        Task Run_Async();

        IEnumerable<Aggregate> Get_Aggregates();
        IParticleFilm<Aggregate> Get_Film();

        void Cancel();
    }
}
