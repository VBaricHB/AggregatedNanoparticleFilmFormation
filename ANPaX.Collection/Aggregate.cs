using System;
using System.Collections.Generic;
using System.Linq;

using ANPaX.Collection.interfaces;

namespace ANPaX.Collection
{
    public class Aggregate : IAggregate
    {
        public Aggregate(IEnumerable<Cluster> cluster)
        {
            Cluster = cluster.ToList();
        }

        public Aggregate()
        {

        }

        public List<Cluster> Cluster { get; private set; }
        public int NumberOfPrimaryParticles => Cluster.Sum(c => c.PrimaryParticles.Count);
        public int NumberOfClusters => Cluster.Count();
        public bool IsDeposited { get; set; }
        public int Id { get; set; }

        public Vector3 GetCenterOfMass()
        {
            var primaryParticles = Cluster.SelectMany(c => c.PrimaryParticles);
            var M = primaryParticles.Sum(p => Math.Pow(p.Radius, 3));
            var x0 = primaryParticles.Sum(p => Math.Pow(p.Radius, 3) * p.Position.X) / M;
            var y0 = primaryParticles.Sum(p => Math.Pow(p.Radius, 3) * p.Position.Y) / M;
            var z0 = primaryParticles.Sum(p => Math.Pow(p.Radius, 3) * p.Position.Z) / M;
            return new Vector3(x0, y0, z0);
        }

        public void MoveBy(Vector3 vector)
        {
            foreach (var cluster in Cluster)
            {
                cluster.MoveBy(vector);
            }
        }

        public void MoveTo(Vector3 vector)
        {
            var moveBy = vector - GetCenterOfMass();

            foreach (var particle in Cluster.SelectMany(c => c.PrimaryParticles))
            {
                particle.MoveBy(moveBy);
            }
        }
    }
}
