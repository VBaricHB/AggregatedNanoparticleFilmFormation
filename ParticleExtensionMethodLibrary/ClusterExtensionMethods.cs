using Accord.Collections;
using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ParticleExtensionMethodLibrary
{
    public static class ClusterExtensionMethods
    {

        public static void MoveTo(this Cluster cluster, Vector3 vector)
        {
            var moveBy = vector - cluster.PrimaryParticles.GetCenterOfMass();

            foreach (var particle in cluster.PrimaryParticles)
            {
                particle.MoveBy(moveBy);
            }
        }

        public static void MoveBy(this Cluster cluster, Vector3 vector)
        {
            foreach (var particle in cluster.PrimaryParticles)
            {
                particle.MoveBy(vector);
            }
        }

        public static Vector3 GetCenterOfMass(this IEnumerable<Cluster> cluster)
        {
            var particles = cluster.SelectMany(c => c.PrimaryParticles);
            var M = particles.Sum(p => Math.Pow(p.Radius, 3));
            var x0 = particles.Sum(p => Math.Pow(p.Radius, 3) * p.Position.X) / M;
            var y0 = particles.Sum(p => Math.Pow(p.Radius, 3) * p.Position.Y) / M;
            var z0 = particles.Sum(p => Math.Pow(p.Radius, 3) * p.Position.Z) / M;
            return new Vector3(x0, y0, z0);
        }

        public static double GetGyrationRadius(this IEnumerable<Cluster> cluster)
        {
            var particles = cluster.SelectMany(c => c.PrimaryParticles);
            return particles.GetGyrationRadius();
        }

        public static double GetGyrationRadius(this Cluster cluster)
        {
            return cluster.PrimaryParticles.GetGyrationRadius();
        }

        

        

        public static KDTree<double> ToNeighborsList(this IEnumerable<Cluster> cluster)
        {
            var particles = cluster.SelectMany(c => c.PrimaryParticles);
            return KDTree.FromData<double>(particles.ToPositionArray());
        }

        
    }
}
