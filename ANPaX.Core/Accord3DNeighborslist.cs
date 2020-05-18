using System;
using System.Collections.Generic;
using System.Linq;

using Accord.Collections;

using ANPaX.Core.interfaces;

namespace ANPaX.Core
{
    public class Accord3DNeighborslist : INeighborslist
    {

        private KDTree<PrimaryParticle> _kdTree;

        public Accord3DNeighborslist(IEnumerable<PrimaryParticle> primaryParticles)
        {
            var particles = primaryParticles.ToArray();
            _kdTree = KDTree.FromData(primaryParticles.ToPositionArray(), particles);
        }

        public virtual void AddParticlesToNeighborsList(Aggregate aggregate)
        {
            foreach (var pp in aggregate.Cluster.SelectMany(c => c.PrimaryParticles))
            {
                _kdTree.Add(pp.Position.ToArray(), pp);
            }
        }

        public virtual void AddParticlesToNeighborsList(Cluster cluster)
        {
            foreach (var pp in cluster.PrimaryParticles)
            {
                _kdTree.Add(pp.Position.ToArray(), pp);
            }
        }

        public virtual void AddParticlesToNeighborsList(PrimaryParticle primaryParticle)
        {
            _kdTree.Add(primaryParticle.Position.ToArray(), primaryParticle);
        }

        public virtual IEnumerable<Tuple<PrimaryParticle, double>> GetPrimaryParticlesAndDistanceWithinRadius(Vector3 Position, double radius)
        {
            var neighbors = _kdTree.Nearest
            (
                position: Position.ToArray(),
                radius: radius
            );

            var particlesAndDistances = new List<Tuple<PrimaryParticle, double>>();

            foreach (var neighbor in neighbors)
            {
                var particleAndDistance = new Tuple<PrimaryParticle, double>(neighbor.Node.Value, neighbor.Distance);
                particlesAndDistances.Add(particleAndDistance);
            }

            return particlesAndDistances;
        }

        public virtual IEnumerable<PrimaryParticle> GetPrimaryParticlesWithinRadius(Vector3 Position, double radius)
        {

            var neighbors = _kdTree.Nearest
            (
                position: Position.ToArray(),
                radius: radius
            );

            return neighbors.Select(n => n.Node.Value);
        }

        public void AddVirtualParticlePeriodicBoundaries(Aggregate aggregate, double maxRadius, BoxDimension xDim, BoxDimension yDim)
        {
            foreach (var primaryParticle in aggregate.GetPrimaryParticles())
            {
                var virtualParticle = GetVirtualParticle(primaryParticle);
                CheckPeriodicBoundary(virtualParticle, primaryParticle.Position.X, maxRadius, xDim);
                CheckPeriodicBoundary(virtualParticle, primaryParticle.Position.Y, maxRadius, yDim);
            }
        }

        private void CheckPeriodicBoundary(PrimaryParticle virtualParticle, double oneDPosition, double maxRadius, BoxDimension dim)
        {
            if (oneDPosition + virtualParticle.Radius >= dim.Upper - maxRadius)
            {
                virtualParticle.Position.X -= dim.Width;
                AddParticlesToNeighborsList(virtualParticle);
            }
            else if (oneDPosition - virtualParticle.Radius <= dim.Lower + maxRadius)
            {
                virtualParticle.Position.X += dim.Width;
                AddParticlesToNeighborsList(virtualParticle);
            }
        }

        /// <summary>
        /// A virtual particle is only within the neighborslist to ensure that any
        /// particle that would hit another particle through periodic boundary conditions
        /// can see this virtual particle
        /// </summary>
        /// <param name="primaryParticle"></param>
        /// <returns></returns>
        private PrimaryParticle GetVirtualParticle(PrimaryParticle primaryParticle)
        {
            return new PrimaryParticle
            {
                Id = -1 * primaryParticle.Id,
                Position = new Vector3
                            (
                            primaryParticle.Position.X,
                            primaryParticle.Position.Y,
                            primaryParticle.Position.Z
                            ),
                Radius = primaryParticle.Radius
            };
        }
    }
}
