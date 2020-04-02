using CommonLibrary.interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AggregateFormation
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
