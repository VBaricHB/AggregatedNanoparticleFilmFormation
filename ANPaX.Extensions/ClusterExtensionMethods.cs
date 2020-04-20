﻿using System;
using System.Collections.Generic;
using System.Linq;

using Accord.Collections;

using ANPaX.Collection;

namespace ANPaX.Extensions
{
    public static class ClusterExtensionMethods
    {
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

        public static Vector3 GetCenterOfMass(this IEnumerable<Cluster> cluster)
        {
            var primaryParticles = cluster.SelectMany(c => c.PrimaryParticles);
            var M = primaryParticles.Sum(p => Math.Pow(p.Radius, 3));
            var x0 = primaryParticles.Sum(p => Math.Pow(p.Radius, 3) * p.Position.X) / M;
            var y0 = primaryParticles.Sum(p => Math.Pow(p.Radius, 3) * p.Position.Y) / M;
            var z0 = primaryParticles.Sum(p => Math.Pow(p.Radius, 3) * p.Position.Z) / M;
            return new Vector3(x0, y0, z0);
        }

    }
}
