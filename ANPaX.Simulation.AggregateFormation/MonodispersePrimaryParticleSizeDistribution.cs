using ANPaX.Simulation.AggregateFormation.interfaces;

namespace ANPaX.Simulation.AggregateFormation
{
    internal class MonodispersePrimaryParticleSizeDistribution : ISizeDistribution<double>
    {
        private readonly double _size;

        public MonodispersePrimaryParticleSizeDistribution(double size)
        {
            _size = size;
        }

        public double Mean => _size;

        public double GetRandomSize() => _size;
    }
}
