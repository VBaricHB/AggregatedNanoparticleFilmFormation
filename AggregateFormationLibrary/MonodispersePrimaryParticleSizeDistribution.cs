using System;
using AggregateFormation.interfaces;

namespace AggregateFormation
{
    internal class MonodispersePrimaryParticleSizeDistribution : ISizeDistribution<double>
    {
        private double _size; 

        public MonodispersePrimaryParticleSizeDistribution(double size)
        {
            _size = size;
        }

        public double Mean => _size;

        public double GetRandomSize() => _size;
    }
}
