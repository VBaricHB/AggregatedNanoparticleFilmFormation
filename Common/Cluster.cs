using System;
using System.Collections.Generic;
using System.Linq;
using Common.interfaces;

namespace Common
{
    public class Cluster : ICluster
    {
        public Cluster(List<PrimaryParticle> primaryParticles)
        {
            PrimaryParticles = primaryParticles;
        }

        public int NumberOfPrimaryParticles => PrimaryParticles.Count();

        public List<PrimaryParticle> PrimaryParticles { get; }

        public void MoveTo(Vector3 vector)
        {
            var moveBy = vector -  Utility.GetCenterOfMass(PrimaryParticles);

            foreach(var particle in PrimaryParticles)
            {
                particle.MoveBy(moveBy);
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
