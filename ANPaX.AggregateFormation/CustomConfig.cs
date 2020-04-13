
using ANPaX.Collection.interfaces;

namespace ANPaX.AggregateFormation
{
    public class CustomConfig : IConfig
    {
        public double Epsilon => 1.001;

        public double Delta => 1.01;

        public double Df => 1.8;

        public double Kf => 1.30;

        public double NeighborAddDistance => 1.0;
        public long MaxTimePerClusterMilliseconds => 500;

        public long MaxTimePerAggregateMilliseconds => 5000;

        public double LargeNumber => 1e10;
    }
}
