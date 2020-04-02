using AggregateFormation.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AggregateFormation
{
    internal class TabulatedAggregateSizeDistribution : ISizeDistribution<int>
    {
        private Random _rndGen;
        private XMLSizeDistribution<int> _tabulatedSizeDistribution;

        public int Mean { get; private set; }

        public TabulatedAggregateSizeDistribution(XMLSizeDistribution<int> tabulatedSizeDistribution, bool integrate = true, int seed = -1)
        {
            _rndGen = new Random();
            if (seed == -1)
            {
                _rndGen = new Random(seed);
            }

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
            for (int i = 1; i < _tabulatedSizeDistribution.Sizes.Length; i++)
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
            var listOfRandomR = new List<int>();
            for (var i = 0; i < 10000; i++)
            {
                listOfRandomR.Add(GetRandomSize());
            }

            Mean = Convert.ToInt32(Math.Round(listOfRandomR.Average()));
        }
    }
}
