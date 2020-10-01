using ANPaX.Core.interfaces;
using ANPaX.Core.ParticleFilm.interfaces;
using ANPaX.Simulation.FilmFormation.interfaces;

namespace ANPaX.Simulation.FilmFormation
{
    public interface ISimulationBoxFactory
    {
        ISimulationBox Build(IFilmFormationConfig filmFormationConfig);
    }
}
