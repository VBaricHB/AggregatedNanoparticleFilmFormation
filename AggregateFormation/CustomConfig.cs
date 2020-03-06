
using Common.interfaces;

namespace AggregateFormation
{
    public class CustomConfig : IConfig
    {
        public double Epsilon => 1.001;

        public double Delta => 1.01;

        public double Df => 1.78;

        public double Kf => 1.00;

        public double NeighborAddDistance { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    }
}
