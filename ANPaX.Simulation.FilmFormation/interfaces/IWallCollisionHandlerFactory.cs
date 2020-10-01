using ANPaX.Core.ParticleFilm.interfaces;
using ANPaX.Simulation.FilmFormation.interfaces;

namespace ANPaX.Simulation.FilmFormation
{
    public interface IWallCollisionHandlerFactory
    {
        IWallCollisionHandler Build(ISimulationBox simulationBox);
    }
}
