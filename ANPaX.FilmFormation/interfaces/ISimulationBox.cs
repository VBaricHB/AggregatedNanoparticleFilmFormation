using ANPaX.FilmFormation;

namespace ANPaX.FilmFormation.interfaces
{
    public interface ISimulationBox
    {
        BoxDimension XDim { get; }
        BoxDimension YDim { get; }
        BoxDimension ZDim { get; }

    }
}