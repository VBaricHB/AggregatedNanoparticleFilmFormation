using ANPaX.Core.interfaces;
using ANPaX.Simulation.FilmFormation.interfaces;

namespace ANPaX.Simulation.FilmFormation
{
    public interface IAggregateDepositionHandlerFactory
    {
        IAggregateDepositionHandler Build(ISingleParticleDepositionHandler singleParticleDepositionHandler, IFilmFormationConfig filmFormationConfig);
    }
}
