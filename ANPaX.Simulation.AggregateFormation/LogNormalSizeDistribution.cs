using System;

using ANPaX.Simulation.AggregateFormation.interfaces;

using MathNet.Numerics.Distributions;

namespace ANPaX.Simulation.AggregateFormation
{
    public class LogNormalSizeDistribution : ISizeDistribution<double>
    {
        public double Mean { get; }
        private Random _rndGen { get; }
        private IAggregateFormationConfig _config { get; }
        private LogNormal _logNormal { get; }
        private double Sigma
        {
            get
            {
                return Math.Sqrt(Math.Log((Math.Pow(_config.StdPPRadius, 2) / Math.Pow(_config.MeanPPRadius, 2)) + 1));
            }
        }

        private double Mu
        {
            get
            {
                return Math.Log(Mean) + Math.Pow(Sigma, 2) / 2;
            }
        }


        public LogNormalSizeDistribution(Random rndGen, IAggregateFormationConfig config)
        {
            _rndGen = rndGen;
            _config = config;
            Mean = config.MeanPPRadius;
            _logNormal = new LogNormal(Mu, _config.StdPPRadius, rndGen);
        }

        public double GetRandomSize()
        {
            var value = _logNormal.Sample();
            return value;
        }
    }
}
