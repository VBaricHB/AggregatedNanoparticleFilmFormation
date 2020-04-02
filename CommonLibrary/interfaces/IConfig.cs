using System;
namespace CommonLibrary.interfaces
{
    public interface IConfig
    {
        double NeighborAddDistance { get; }

        public double Epsilon { get; }

        public double Delta { get; }

        public double Df { get; }

        public double Kf { get; }

        public long MaxTimeMilliseconds { get;  }

        public double LargeNumber { get; }
    }
}
