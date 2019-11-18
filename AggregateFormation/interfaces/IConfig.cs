using System;
namespace AggregateFormation.interfaces
{
    public interface IConfig
    {
        double Epsilon { get; }
        double Delta { get; }
        double Df { get; }
        double Kf { get; }
    }
}
