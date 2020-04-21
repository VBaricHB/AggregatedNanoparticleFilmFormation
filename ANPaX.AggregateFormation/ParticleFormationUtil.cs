using System;
using System.Collections.Generic;
using System.Linq;

using Accord.Collections;

using ANPaX.AggregateFormation.interfaces;
using ANPaX.Extensions;

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

        /// <summary>
        /// Check if the primary particle is in contact with any other primary particle (isInContact) and if it is overlapping with any other primary particle (hasNoOverlap)
        /// </summary>
        /// <param name="particle"></param>
        /// <param name="primaryParticles"></param>
        /// <param name="config"></param>
        /// <param name="neighbors"></param>
        /// <returns></returns>
        public static (bool isInContact, bool hasNoOverlap) IsAnyNeighborInContactOrOverlapping(PrimaryParticle particle, IEnumerable<PrimaryParticle> primaryParticles, IAggregateFormationConfig config, List<NodeDistance<KDTreeNode<double>>> neighbors)
        {
            var isInContact = false;
            var hasNoOverlap = true;
            foreach (var neigh in neighbors)
            {
                var radiusParticle2 = GetRadiusOfNodePrimaryParticle(neigh.Node.Position, primaryParticles);

                isInContact = isInContact || IsInContact(neigh.Distance, particle.Radius, radiusParticle2, config.Delta);
                hasNoOverlap = hasNoOverlap && HasNoOverlap(neigh.Distance, particle.Radius, radiusParticle2, config.Epsilon);
            }
            return (isInContact, hasNoOverlap);
        }


        /// <summary>
        /// Search in a defined radius around the primary particles if there are any more primary particles.
        /// </summary>
        /// <param name="primaryParticle">Primary particle that will be positioned</param>
        /// <param name="randomPosition">The random position where the primary particle might be positioned</param>
        /// <param name="otherPrimaryParticles">All primary particles that are already deposited (and remain fixed)</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static List<NodeDistance<KDTreeNode<double>>> GetPossibleNeighbors(
            PrimaryParticle primaryParticle,
            Vector3 randomPosition,
            KDTree<double> tree,
            IEnumerable<PrimaryParticle> otherPrimaryParticles,
            IAggregateFormationConfig config)
        {
            var searchRadius = (primaryParticle.Radius + otherPrimaryParticles.GetMaxRadius()) * config.Delta;
            var neighbors = tree.Nearest(randomPosition.ToArray(),
                radius: searchRadius);

            return neighbors;
        }

        /// <summary>
        /// Two primary particles are in contact if the distance of their center of masses is
        /// less or equal the sum of their radii + an additional threshold distance (relative to their size)
        /// </summary>
        /// <param name="distance">Distance of the two center of masses</param>
        /// <param name="radiusParticle1"></param>
        /// <param name="radiusParticle2"></param>
        /// <param name="delta">Max distance threshold factor</param>
        /// <returns></returns>
        public static bool IsInContact(double distance, double radiusParticle1, double radiusParticle2, double delta)
        {
            return (distance < delta * (radiusParticle2 + radiusParticle1));
        }

        /// <summary>
        /// Two primary particles overlap if the distance of ther center of masses is less than the sum of their radii + an min distance threshold factor
        /// (relative to their radii)
        /// </summary>
        /// <param name="distance">Distance of the two center of masses</param>
        /// <param name="radiusParticle1"></param>
        /// <param name="radiusParticle2"></param>
        /// <param name="epsilon">Min distance threshold factor</param>
        /// <returns></returns>
        public static bool HasNoOverlap(double distance, double radiusParticle1, double radiusParticle2, double epsilon)
        {
            return (distance >= epsilon * (radiusParticle2 + radiusParticle1));
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
