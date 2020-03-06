using System;
namespace Common.interfaces
{
    public interface IConfig
    {
        double NeighborAddDistance { get; set; }

        public double Epsilon { get; }

        public double Delta { get; }

        public double Df { get; }

        public double Kf { get; }
    }
}
