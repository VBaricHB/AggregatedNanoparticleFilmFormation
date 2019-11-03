using Accord.Collections;

namespace DirectDepositionAlgorithm
{
    public class PrimaryParticle
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public double Radius { get; set; }

        public PrimaryParticle(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double[] PositionToArray() => new double[] { X, Y, Z };
    }
}
