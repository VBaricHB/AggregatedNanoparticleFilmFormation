using System;
using System.Collections.Generic;
using System.Linq;

using ANPaX.Core.interfaces;

namespace ANPaX.Core
{
    public class Cluster : ICluster
    {
        public Cluster(int id, List<PrimaryParticle> primaryParticles)
        {
            Id = id;
            PrimaryParticles = primaryParticles;
        }

        public Cluster()
        {

        }

        public int NumberOfPrimaryParticles => PrimaryParticles.Count();

        public List<PrimaryParticle> PrimaryParticles { get; set; }

        public int Id { get; set; }

        public override string ToString()
        {
            return $"{Id}";
        }

        public void MoveTo(Vector3 vector)
        {
            var moveBy = vector - GetCenterOfMass();

            foreach (var particle in PrimaryParticles)
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

        public Vector3 GetCenterOfMass()
        {
            var M = PrimaryParticles.Sum(p => Math.Pow(p.Radius, 3));
            var x0 = PrimaryParticles.Sum(p => Math.Pow(p.Radius, 3) * p.Position.X) / M;
            var y0 = PrimaryParticles.Sum(p => Math.Pow(p.Radius, 3) * p.Position.Y) / M;
            var z0 = PrimaryParticles.Sum(p => Math.Pow(p.Radius, 3) * p.Position.Z) / M;
            return new Vector3(x0, y0, z0);
        }

    }
}
