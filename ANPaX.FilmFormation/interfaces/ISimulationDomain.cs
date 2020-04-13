using System.Collections.Generic;
using ANPaX.Collection;

namespace ANPaX.FilmFormation.interfaces
{
    public interface ISimulationDomain<T>
    {
        ISimulationBox SimulationBox {get; }
        int NumberOfPrimaryParticles { get; }
        double MaxPrimaryParticleRadius { get; set; }

        void AddDepositedParticlesToDomain(T particle);

        IEnumerable<PrimaryParticle> PrimaryParticles { get; }
    }
}
