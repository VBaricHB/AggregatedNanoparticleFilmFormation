using System;
using System.Linq;

using ANPaX.Simulation.AggregateFormation.interfaces;

namespace ANPaX.Simulation.AggregateFormation
{
    internal class TabulatedPrimaryParticleSizeDistribution : ISizeDistribution<double>
    {
        internal XMLSizeDistribution<double> _tabulatedSizeDistribution;
        private readonly Random _rndGen;
        private readonly IAggregateFormationConfig _config;
        public double Mean { get; private set; }

        public TabulatedPrimaryParticleSizeDistribution(
            XMLSizeDistribution<double> tabulatedSizeDistribution,
            Random rndGen,
            IAggregateFormationConfig config,
            bool integrate = true)
        {
            _rndGen = rndGen;
            _tabulatedSizeDistribution = tabulatedSizeDistribution;
            _config = config;

            if (integrate)
            {
                IntegrateProbabilities();
            }
            CalcMean();

        }

        public double GetRandomSize()
        {
            var probability = _rndGen.NextDouble();
            return _tabulatedSizeDistribution.Sizes.FirstOrDefault(s => s.Probability >= probability).Value;
        }

        private void IntegrateProbabilities()
        {
            for (var i = 1; i < _tabulatedSizeDistribution.Sizes.Length; i++)
            {
                _tabulatedSizeDistribution.Sizes[i].Probability += _tabulatedSizeDistribution.Sizes[i - 1].Probability;
            }

            foreach (var size in _tabulatedSizeDistribution.Sizes)
            {
                size.Probability /= _tabulatedSizeDistribution.Sizes.Last().Probability;
            }
        }

        private void CalcMean()
        {
            //var n = 10000;

            //var randomRadii = new double[n];
            //for (var i = 0; i < n; i++)
            //{
            //    randomRadii[i] = GetRandomSize();
            //}

            var randomRadii = new double[_tabulatedSizeDistribution.Sizes.Count()];
            for (var i = 0; i < _tabulatedSizeDistribution.Sizes.Count(); i++)
            {
                randomRadii[i] = _tabulatedSizeDistribution.Sizes[i].Value;
            }

            switch (_config.RadiusMeanCalculationMethod)
            {
                case MeanMethod.Geometric:
                    Mean = CalcGeometricMean(randomRadii);
                    return;
                case MeanMethod.Arithmetic:
                    Mean = randomRadii.Average();
                    return;
                case MeanMethod.Sauter:
                    Mean = CalcSauterRadius(randomRadii);
                    return;
            }

        }

        /// <summary>
        /// The Sauter mean radius is defined as the radius of a sphere that has the same volume/surface area ratio as a particle of interest.
        /// </summary>
        /// https://www.chemeurope.com/en/encyclopedia/Sauter_diameter.html
        /// <param name="randomRadii"> an array with radom radii</param>
        /// <returns></returns>
        private double CalcSauterRadius(double[] randomRadii)
        {
            var volume = 4.0 / 3.0 * Math.PI * randomRadii.Aggregate((result, item) => result + Math.Pow(item, 3));
            var area = 4.0 * Math.PI * randomRadii.Aggregate((result, item) => result + Math.Pow(item, 2));
            return 3 * volume / area;
        }

        /// <summary>
        /// antilog(((log(1) + log(2) + . . . + log(n))/n) 
        /// </summary>
        /// <param name="randomRadii"></param>
        /// <returns></returns>
        private double CalcGeometricMean(double[] randomRadii)
        {
            var sum = randomRadii.Aggregate(
                seed: 0.0,
                func: (result, item) => result + Math.Log(item),
                resultSelector: result => result / randomRadii.Count()
                );

            return Math.Exp(sum);
        }
    }
}
