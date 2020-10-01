using ANPaX.Core;
using ANPaX.Core.ParticleFilm.interfaces;
using ANPaX.Simulation.FilmFormation.interfaces;

namespace ANPaX.Simulation.FilmFormation
{
    public class AbsoluteTetragonalSimulationBoxFactory : ISimulationBoxFactory
    {
        private readonly IFilmFormationConfig _filmFormationConfig;

        ISimulationBox ISimulationBoxFactory.Build(IFilmFormationConfig filmFormationConfig)
        {
            return new AbsoluteTetragonalSimulationBox(filmFormationConfig.XFilmWidthAbsolute);
        }
    }
}
