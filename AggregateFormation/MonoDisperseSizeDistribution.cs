using System;
using AggregateFormation.interfaces;

namespace AggregateFormation
{
    public class MonoDisperseSizeDistribution : IPrimaryParticleSizeDistribution
    {
        private double _size; 
        public MonoDisperseSizeDistribution(double size)
        {
            _size = size;
        }

        public double MeanRadius => _size;

        public double GetRadiusByProbability(double probability) => _size;
    }
}
