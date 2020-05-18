using System;
using System.Linq;

using ANPaX.Simulation.AggregateFormation.interfaces;

namespace ANPaX.Simulation.AggregateFormation
{
    internal class TabulatedAggregateSizeDistribution : ISizeDistribution<int>
    {
        private Random _rndGen;
        private IFileSizeDistribution<int> _tabulatedSizeDistribution;
        private readonly IAggregateFormationConfig _config;

        public int Mean { get; private set; }

        public TabulatedAggregateSizeDistribution(IFileSizeDistribution<int> tabulatedSizeDistribution, Random rndGen, IAggregateFormationConfig config, bool integrate = true)
        {
            _rndGen = rndGen;
            _config = config;
            _tabulatedSizeDistribution = tabulatedSizeDistribution;

            if (integrate)
            {
                IntegrateProbabilities();
            }
            CalcMean();
        }

        public int GetRandomSize()
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
            var n = 10000;

            var listOfRandomR = new int[n];
            for (var i = 0; i < 10000; i++)
            {
                listOfRandomR[i] = GetRandomSize();
            }

            switch (_config.AggregateSizeMeanCalculationMethod)
            {
                case MeanMethod.Geometric:
                    Mean = CalcGeometricMean(listOfRandomR);
                    return;
                case MeanMethod.Arithmetic:
                    Mean = Convert.ToInt32(Math.Round(listOfRandomR.Average()));
                    return;
                case MeanMethod.Sauter:
                    throw new ArgumentException($"MeanMethod.Sauter cannot be applied to aggregate size.");
            }

        }

        /// <summary>
        /// antilog(((log(1) + log(2) + . . . + log(n))/n) 
        /// </summary>
        /// <param name="randomRadii"></param>
        /// <returns></returns>
        private int CalcGeometricMean(int[] randomRadii)
        {
            var sum = randomRadii.Aggregate(
                seed: 0.0,
                func: (result, item) => result + Math.Log(item),
                resultSelector: result => result / randomRadii.Count()
                );

            return Convert.ToInt32(Math.Exp(sum));
        }
    }
}
