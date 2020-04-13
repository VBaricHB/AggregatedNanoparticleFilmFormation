using ANPaX.Collection.interfaces;

namespace ANPaX.AggregateFormation
{
    class CustomParticleGenerationParameters : IParticleFormationParameters
    {
        public CustomParticleGenerationParameters()
        {
            TotalPrimaryParticles = 1000;
        }

        public CustomParticleGenerationParameters(int primaryParticles)
        {
            TotalPrimaryParticles = primaryParticles;
        }

        public int ClusterSize => 6;

        public int TotalPrimaryParticles { get; set; }
    }
}
