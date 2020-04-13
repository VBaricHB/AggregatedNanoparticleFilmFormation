using ANPaX.AggregateFormation.interfaces;
using System;
using System.Linq;

namespace ANPaX.AggregateFormation
{
    internal class TabulatedPrimaryParticleSizeDistribution : ISizeDistribution<double>
    {
        internal XMLSizeDistribution<double> _tabulatedSizeDistribution;
        private readonly Random _rndGen;
        public double Mean { get; private set; }

        public TabulatedPrimaryParticleSizeDistribution(XMLSizeDistribution<double> tabulatedSizeDistribution, Random rndGen, bool integrate = true)
        {
            _rndGen = rndGen;
            _tabulatedSizeDistribution = tabulatedSizeDistribution;

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

            var listOfRandomR = new double[n];
            for (var i = 0; i < 10000; i++)
            {
                listOfRandomR[i] = GetRandomSize();
            }

            //listOfRandomR = _tabulatedSizeDistribution.Sizes.Select(c => c.Value).ToArray();
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

            Mean = Math.Exp(sum);
            //Mean = listOfRandomR.Average();
            //Mean = 5.65;
        }
    }
}
