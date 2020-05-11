using System;
using System.Collections.Generic;
using System.Linq;

using ANPaX.Collection;

namespace ANPaX.Extensions
{
    public static class PrimaryParticleExtensionMethods
    {

        public static double GetMaxRadius(this IEnumerable<PrimaryParticle> primaryParticles)
        {
            return primaryParticles.Max(p => p.Radius);
        }

        public static double GetDistanceToPrimaryParticle(this PrimaryParticle self, PrimaryParticle other)
        {
            var x = Math.Pow(self.Position.X - other.Position.X, 2);
            var y = Math.Pow(self.Position.Y - other.Position.Y, 2);
            var z = Math.Pow(self.Position.Z - other.Position.Z, 2);
            return Math.Round(Math.Sqrt(x + y + z), 6);
        }

        public static double GetDistanceToPosition(this PrimaryParticle self, Vector3 other)
        {
            var x = Math.Pow(self.Position.X - other.X, 2);
            var y = Math.Pow(self.Position.Y - other.Y, 2);
            var z = Math.Pow(self.Position.Z - other.Z, 2);
            var distance = Math.Round(Math.Sqrt(x + y + z), 6);
            return distance;
        }

        /// <summary>
        /// The squared distance does not require the square root. Hence, it is 
        /// computationally more efficient than the distance.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static double GetSquaredDistanceToPrimaryParticle(this PrimaryParticle self, PrimaryParticle other)
        {
            var x = Math.Pow(self.Position.X - other.Position.X, 2);
            var y = Math.Pow(self.Position.Y - other.Position.Y, 2);
            var z = Math.Pow(self.Position.Z - other.Position.Z, 2);
            return Math.Round(x + y + z, 6);
        }

        public static double GetDistanceToVerticalAxis(this PrimaryParticle primaryParticle, double[] neighborPosition)
        {
            return Math.Sqrt(Math.Pow(primaryParticle.Position.X - neighborPosition[0], 2) +
                             Math.Pow(primaryParticle.Position.Y - neighborPosition[1], 2)
                );
        }

        public static double GetDistanceToVerticalAxis(this PrimaryParticle primaryParticle, PrimaryParticle neighbor)
        {
            return Math.Sqrt(Math.Pow(primaryParticle.Position.X - neighbor.Position.X, 2) +
                             Math.Pow(primaryParticle.Position.Y - neighbor.Position.Y, 2)
                );
        }

        public static Vector3 GetCenterOfMass(this IEnumerable<PrimaryParticle> particles)
        {
            var M = particles.Sum(p => Math.Pow(p.Radius, 3));
            var x0 = particles.Sum(p => Math.Pow(p.Radius, 3) * p.Position.X) / M;
            var y0 = particles.Sum(p => Math.Pow(p.Radius, 3) * p.Position.Y) / M;
            var z0 = particles.Sum(p => Math.Pow(p.Radius, 3) * p.Position.Z) / M;
            return new Vector3(x0, y0, z0);
        }

        public static double GetGyrationRadius(this IEnumerable<PrimaryParticle> particles)
        {
            var com = particles.GetCenterOfMass();
            var rg = 0.0;
            foreach (var p in particles)
            {
                rg += Math.Pow(p.Position.X - com.X, 2)
                    + Math.Pow(p.Position.Y - com.Y, 2)
                    + Math.Pow(p.Position.Z - com.Z, 2);
            }
            return Math.Sqrt(rg / Convert.ToDouble(particles.Count()));
        }

        public static double[][] ToPositionArray(this IEnumerable<PrimaryParticle> particles)
        {
            var lparticles = particles.ToList();
            var array = new double[particles.Count()][];
            for (var p = 0; p < particles.Count(); p++)
            {
                array[p] = lparticles[p].Position.ToArray();
            }
            return array;
        }

        public static double[][] ToXYPositionArray(this IEnumerable<PrimaryParticle> particles)
        {
            var lparticles = particles.ToList();
            var array = new double[particles.Count()][];
            for (var p = 0; p < particles.Count(); p++)
            {
                array[p] = lparticles[p].Position.ToXYArray();
            }
            return array;
        }
    }
}
