using ANPaX.Core.ParticleFilm.interfaces;

namespace ANPaX.Core
{
    public class AbsoluteTetragonalSimulationBox : ISimulationBox
    {
        public BoxDimension XDim { get; }
        public BoxDimension YDim { get; }
        public BoxDimension ZDim { get; }

        public AbsoluteTetragonalSimulationBox(double width)
        {
            XDim = new BoxDimension(-0.5 * width, 0.5 * width);
            YDim = new BoxDimension(-0.5 * width, 0.5 * width);
            ZDim = new BoxDimension(0, 0);
        }
    }
}
