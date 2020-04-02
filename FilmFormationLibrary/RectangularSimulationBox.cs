using AggregatedNanoparticleFilmFormation;
using FilmFormationLibrary.interfaces;

namespace FilmFormationLibrary
{
    internal class RectangularSimulationBox : ISimulationBox
    {
        public BoxDimension XDim { get; }
        public BoxDimension YDim { get; }
        public BoxDimension ZDim { get; }

        public RectangularSimulationBox(double width)
        {
            XDim = new BoxDimension(- 0.5 * width, 0.5 * width);
            YDim = new BoxDimension(-0.5 * width, 0.5 * width);
            ZDim = new BoxDimension(0, 0);
        }
    }
}
