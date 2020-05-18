using System.Collections.Generic;

using ANPaX.Core.ParticleFilm;

namespace ANPaX.Core.interfaces
{
    public interface IParticleFilm<T>
    {
        ISimulationBox SimulationBox { get; }
        int NumberOfPrimaryParticles { get; }
        void AddDepositedParticlesToFilm(T particle);
        IList<T> Particles { get; }
        IEnumerable<PrimaryParticle> PrimaryParticles { get; }
        INeighborslist Neighborslist2D { get; }
    }
}
