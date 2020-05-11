using System;
using System.Collections.Generic;
using System.Linq;

using Accord.Collections;

using ANPaX.Collection;
using ANPaX.Extensions;

namespace ANPaX.Core.Neighborslist
{
    public class Accord3DNeighborslist : INeighborslist
    {

        private KDTree<PrimaryParticle> _kdTree;

        public Accord3DNeighborslist(IEnumerable<PrimaryParticle> primaryParticles)
        {
            var particles = primaryParticles.ToArray();
            _kdTree = KDTree.FromData(primaryParticles.ToPositionArray(), particles);
        }

        public void AddParticlesToNeighborsList(Aggregate aggregate)
        {
            foreach (var pp in aggregate.Cluster.SelectMany(c => c.PrimaryParticles))
            {
                _kdTree.Add(pp.Position.ToArray(), pp);
            }
        }

        public void AddParticlesToNeighborsList(Cluster cluster)
        {
            foreach (var pp in cluster.PrimaryParticles)
            {
                _kdTree.Add(pp.Position.ToArray(), pp);
            }
        }

        public void AddParticlesToNeighborsList(PrimaryParticle primaryParticle)
        {
            _kdTree.Add(primaryParticle.Position.ToArray(), primaryParticle);
        }

        public IEnumerable<Tuple<PrimaryParticle, double>> GetPrimaryParticlesAndDistanceWithinRadius(Vector3 Position, double radius)
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

        public IEnumerable<PrimaryParticle> GetPrimaryParticlesWithinRadius(Vector3 Position, double radius)
        {

            var neighbors = _kdTree.Nearest
            (
                position: Position.ToArray(),
                radius: radius
            );

            return neighbors.Select(n => n.Node.Value);
        }
    }
}
