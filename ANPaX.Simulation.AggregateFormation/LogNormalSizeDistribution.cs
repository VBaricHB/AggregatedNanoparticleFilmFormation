using System;
using System.Collections.Generic;
using System.Linq;

using ANPaX.Simulation.AggregateFormation.interfaces;

using MathNet.Numerics.Distributions;

namespace ANPaX.Simulation.AggregateFormation
{
    public class LogNormalSizeDistribution : ISizeDistribution<double>
    {
        private double _arithmeticMean;
        private double _geometricMean;
        private double _sauterMean;

        public double Mean { get; }
        private Random _rndGen { get; }
        private IAggregateFormationConfig _config { get; }
        private LogNormal _logNormal { get; }
        private double Sigma
        {
            get
            {
                return Math.Sqrt(Math.Log((Math.Pow(_config.StdPPRadius, 2) / Math.Pow(_config.ModalRadius, 2)) + 1));
            }
        }

        private double Mu
        {
            get
            {
                return Math.Log(_config.ModalRadius) + Math.Pow(Sigma, 2) / 2;
            }
        }

        public LogNormalSizeDistribution(Random rndGen, IAggregateFormationConfig config)
        {
            _rndGen = rndGen;
            _config = config;

            _logNormal = new LogNormal(Mu, _config.StdPPRadius, rndGen);

            ComputeMeans();
            Mean = config.RadiusMeanCalculationMethod switch
            {
                MeanMethod.Arithmetic => _arithmeticMean,
                MeanMethod.Sauter => _sauterMean,
                MeanMethod.Geometric => _geometricMean,
                _ => _geometricMean,
            };
        }

        public double GetRandomSize()
        {
            var value = _logNormal.Sample();
            return value;
        }

        private void ComputeMeans()
        {
            var n = 1000;
            double exp = 1 / Convert.ToDouble(n);
            var values = new List<double>();
            double product = 1;
            double volume = 0;
            double surfaceArea = 0;
            for (var i = 0; i < n; i++)
            {
                var value = GetRandomSize();
                values.Add(value);
                product *= Math.Pow(value, exp);
                volume += 4 / 3 * Math.PI * Math.Pow(value, 3);
                surfaceArea += 4 * Math.PI * Math.Pow(value, 2);
            }

            _arithmeticMean = values.Average();
            _geometricMean = product;
            _sauterMean = 3 * volume / surfaceArea;
        }
    }
}
