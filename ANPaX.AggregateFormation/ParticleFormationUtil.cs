using System;
using System.Collections.Generic;
using System.Linq;

using Accord.Collections;

using ANPaX.AggregateFormation.interfaces;

namespace ANPaX.Collection
{
    public static class ParticleFormationUtil
    {
        public static Vector3 GetRandomPosition(Random rndGen, double distance)
        {
            var (z, theta, phi) = GetRandomHeightAndAngles(rndGen, distance);
            var (x, y) = GetPositionAtAngleAndDistance(distance, phi, theta);
            return new Vector3(x, y, z);
        }

        public static (double z, double theta, double phi) GetRandomHeightAndAngles(Random rndGen, double distance)
        {
            var z = distance * (1 - 2 * rndGen.NextDouble());
            var theta = Math.Acos(z / distance);
            var phi = rndGen.NextDouble() * 2 * Math.PI;
            return (z, theta, phi);
        }

        public static (double x, double y) GetPositionAtAngleAndDistance(double distance, double phi, double theta)
        {
            var x = distance * Math.Cos(phi) * Math.Sin(theta);
            var y = distance * Math.Sin(phi) * Math.Sin(theta);
            return (x, y);
        }

        public static (bool nearby, bool feasible) IsValidPosition(NodeDistance<KDTreeNode<double>> neigh,
            IEnumerable<PrimaryParticle> primaryParticles, double radius, IAggregateFormationConfig config)
        {
            var feasible = true;
            var nearby = false;
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

        public static double GetRadiusOfXYNodePrimaryParticle(double[] position, IEnumerable<PrimaryParticle> primaryParticles)
        {
            return primaryParticles.FirstOrDefault(p => Math.Abs(p.Position.X - position[0]) < 0.01
                                                     && Math.Abs(p.Position.Y - position[1]) < 0.01
                                                  ).Radius;
        }


    }
}
