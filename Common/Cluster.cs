using System;
using System.Collections.Generic;
using System.Linq;
using Common.interfaces;

namespace Common
{
    public class Cluster : ICluster
    {
        public Cluster(IEnumerable<PrimaryParticle> primaryParticles)
        {
            PrimaryParticles = primaryParticles;
        }

        public int NumberOfPrimaryParticles => PrimaryParticles.Count();

        public IEnumerable<PrimaryParticle> PrimaryParticles { get; }

        public void MoveTo(Vector3 vector)
        {
            foreach(var particle in PrimaryParticles)
            {
                particle.MoveTo(vector);
            }
        }

        public void MoveBy(Vector3 vector)
        {
            foreach (var particle in PrimaryParticles)
            {
                particle.MoveBy(vector);
            }
        }
    }
}
