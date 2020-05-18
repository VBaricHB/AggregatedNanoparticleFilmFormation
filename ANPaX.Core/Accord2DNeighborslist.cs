using System;
using System.Collections.Generic;
using System.Linq;

using Accord.Collections;

using ANPaX.Core.interfaces;

namespace ANPaX.Core
{
    public class Accord2DNeighborslist : Accord3DNeighborslist, INeighborslist
    {
        private KDTree<PrimaryParticle> _kdTree;

        public Accord2DNeighborslist(IEnumerable<PrimaryParticle> primaryParticles) : base(primaryParticles)
        {
            var particles = primaryParticles.ToArray();
            _kdTree = KDTree.FromData(primaryParticles.ToXYPositionArray(), particles);
        }

        public override void AddParticlesToNeighborsList(Aggregate aggregate)
        {
            foreach (var pp in aggregate.Cluster.SelectMany(c => c.PrimaryParticles))
            {
                _kdTree.Add(pp.Position.ToXYArray(), pp);
            }
        }

        public override void AddParticlesToNeighborsList(Cluster cluster)
        {
            foreach (var pp in cluster.PrimaryParticles)
            {
                _kdTree.Add(pp.Position.ToXYArray(), pp);
            }
        }

        public override void AddParticlesToNeighborsList(PrimaryParticle primaryParticle)
        {
            _kdTree.Add(primaryParticle.Position.ToXYArray(), primaryParticle);
        }

        public override IEnumerable<Tuple<PrimaryParticle, double>> GetPrimaryParticlesAndDistanceWithinRadius(Vector3 Position, double radius)
        {
            var neighbors = _kdTree.Nearest
            (
                position: Position.ToXYArray(),
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

        public override IEnumerable<PrimaryParticle> GetPrimaryParticlesWithinRadius(Vector3 Position, double radius)
        {

            var neighbors = _kdTree.Nearest
            (
                position: Position.ToXYArray(),
                radius: radius
            );

            return neighbors.Select(n => n.Node.Value);
        }

    }
}
