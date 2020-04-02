using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Collections;
using CommonLibrary;
using CommonLibrary.interfaces;

namespace CommonLibrary
{
    public static class ParticleFormationService
    {
        public static double DistanceSquared(PrimaryParticle particle1, PrimaryParticle particle2)
        {
            var x = Math.Pow(particle2.Position.X - particle1.Position.X, 2);
            var y = Math.Pow(particle2.Position.Y - particle1.Position.Y, 2);
            var z = Math.Pow(particle2.Position.Z - particle1.Position.Z, 2);
            return x + y + z;
        }

        public static double Distance(PrimaryParticle particle1, PrimaryParticle particle2)
        {
            var x = Math.Pow(particle2.Position.X - particle1.Position.X, 2);
            var y = Math.Pow(particle2.Position.Y - particle1.Position.Y, 2);
            var z = Math.Pow(particle2.Position.Z - particle1.Position.Z, 2);
            return Math.Round(Math.Sqrt(x + y + z), 6);
        }

        public static Vector3 GetCenterOfMass(IEnumerable<PrimaryParticle> particles)
        {
            var M = particles.Sum(p => Math.Pow(p.Radius, 3));
            var x0 = particles.Sum(p => Math.Pow(p.Radius, 3) * p.Position.X) / M;
            var y0 = particles.Sum(p => Math.Pow(p.Radius, 3) * p.Position.Y) / M;
            var z0 = particles.Sum(p => Math.Pow(p.Radius, 3) * p.Position.Z) / M;
            return new Vector3(x0, y0, z0);
        }

        public static double GetRadiusOfGyration(Cluster cluster)
        {
            return GetRadiusOfGyration(cluster.PrimaryParticles);
        }

        public static double GetRadiusOfGyration(List<Cluster> cluster)
        {
            var particles = cluster.SelectMany(c => c.PrimaryParticles);
            return GetRadiusOfGyration(particles);
        }

        public static double GetRadiusOfGyration(IEnumerable<PrimaryParticle> particles)
        {
            var com = ParticleFormationService.GetCenterOfMass(particles);
            var rg = 0.0;
            foreach (var p in particles)
            {
                rg += Math.Pow(p.Position.X - com.X, 2)
                    + Math.Pow(p.Position.Y - com.Y, 2)
                    + Math.Pow(p.Position.Z - com.Z, 2);
            }
            return Math.Sqrt(rg / Convert.ToDouble(particles.Count()));
        }

        public static double[][] GetPositionArray(IEnumerable<PrimaryParticle> particles)
        {
            var lparticles = particles.ToList();
            var array = new double[particles.Count()][];
            for (var p = 0; p < particles.Count(); p++)
            {
                array[p] = lparticles[p].Position.ToArray();
            }
            return array;
        }

        public static KDTree<double> BuildNeighborsList(IEnumerable<PrimaryParticle> particles)
        {
            return KDTree.FromData<double>(ParticleFormationService.GetPositionArray(particles));
        }

        public static Vector3 GetRandomPosition(Random rndGen, double distance)
        {
            var z = distance * (1 - 2 * rndGen.NextDouble());
            var theta = Math.Acos(z / distance);
            var phi = rndGen.NextDouble() * 2 * Math.PI;

            var x = distance * Math.Cos(phi) * Math.Sin(theta);
            var y = distance * Math.Sin(phi) * Math.Sin(theta);
            return new Vector3(x, y, z);
        }

        public static (bool nearby, bool feasible) IsValidPosition(NodeDistance<KDTreeNode<double>> neigh,
            IEnumerable<PrimaryParticle> primaryParticles, double radius, IConfig config)
        {
            bool feasible = true;
            bool nearby = false;
            var r2 = GetRadiusOfNodePrimaryParticle(neigh.Node.Position, primaryParticles);
            // is the neighbor within the threshold distance
            if (neigh.Distance < config.Delta * (r2 + radius))
            {
                nearby = true;
            }
            if (neigh.Distance < config.Epsilon * (r2 + radius))
            {
                feasible = false;
            }
            return (nearby, feasible);
        }


        public static double GetRadiusOfNodePrimaryParticle(double[] position, IEnumerable<PrimaryParticle> primaryParticles)
        {
            return primaryParticles.FirstOrDefault(p => Math.Abs(p.Position.X - position[0]) < 0.01
                                                     && Math.Abs(p.Position.Y - position[1]) < 0.01
                                                     && Math.Abs(p.Position.Z - position[2]) < 0.01
                                                  ).Radius;
        }

        public static double GetDistanceToCenterline(PrimaryParticle primaryParticle, double[] neighborPosition)
        {
            return Math.Sqrt(Math.Pow(primaryParticle.Position.X - neighborPosition[0], 2) +
                             Math.Pow(primaryParticle.Position.Y - neighborPosition[1], 2)
                );
        }
    }
}
