using System;
using System.Collections.Generic;
using System.Linq;
using CommonLibrary.interfaces;

namespace CommonLibrary
{
    public class Cluster : ICluster
    {
        public Cluster(int id, List<PrimaryParticle> primaryParticles)
        {
            Id = id;
            PrimaryParticles = primaryParticles;
        }

        public int NumberOfPrimaryParticles => PrimaryParticles.Count();

        public List<PrimaryParticle> PrimaryParticles { get; }

        public int Id { get; }

        public override string ToString()
        {
            return $"{Id}";
        }
    }
}
