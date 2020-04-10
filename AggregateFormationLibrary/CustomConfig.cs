
using CommonLibrary.interfaces;

namespace AggregateFormation
{
    public class CustomConfig : IConfig
    {
        public double Epsilon => 1.000;

        public double Delta => 1.01;

        public double Df => 1.78;

        public double Kf => 1.00;

        public double NeighborAddDistance => 1.0;
        public long MaxTimeMilliseconds => 5000;

        public double LargeNumber => 1e10;
    }
}
