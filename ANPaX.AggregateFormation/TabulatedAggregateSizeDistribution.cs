using ANPaX.AggregateFormation.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ANPaX.AggregateFormation
{
    internal class TabulatedAggregateSizeDistribution : ISizeDistribution<int>
    {
        private Random _rndGen;
        private XMLSizeDistribution<int> _tabulatedSizeDistribution;
        private readonly IAggregateFormationConfig _config;

        public int Mean { get; private set; }

        public TabulatedAggregateSizeDistribution(XMLSizeDistribution<int> tabulatedSizeDistribution, Random rndGen, IAggregateFormationConfig config, bool integrate = true)
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
            var n = 10000;

            var listOfRandomR = new int[n];
            for (var i = 0; i < 10000; i++)
            {
                listOfRandomR[i] = GetRandomSize();
            }

            switch (_config.MeanMethod)
            {
                case MeanMethod.Geometric:
                    Mean = CalcGeometricMean(listOfRandomR);
                    return;
                case MeanMethod.Arithmetic:
                    Mean = Convert.ToInt32(Math.Round(listOfRandomR.Average()));
                    return;
            }

        }

        private int CalcGeometricMean(int[] listOfRandomR)
        {
            //declare sum variable and
            // initialize it to 1.
            double sum = 0;

            // Compute the sum of all the 
            // elements in the array. 
            for (int i = 0; i < listOfRandomR.Count(); i++)
                sum += Math.Log(listOfRandomR[i]);

            // compute geometric mean through formula 
            // antilog(((log(1) + log(2) + . . . + log(n))/n) 
            // and return the value to main function. 
            sum /= listOfRandomR.Count();

            return Convert.ToInt32(Math.Round(Math.Exp(sum)));
        }
    }
}
