using AggregatedNanoparticleFilmFormation;

namespace FilmFormationLibrary.interfaces
{
    public interface ISimulationBox
    {
        BoxDimension XDim { get; }
        BoxDimension YDim { get; }
        BoxDimension ZDim { get; }

    }
}