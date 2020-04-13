using Accord.Collections;
using ANPaX.Collection;
using ANPaX.Collection.interfaces;
using ANPaX.Extensions;
using ANPaX.FilmFormation.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ANPaX.FilmFormation
{
    internal class BallisticSingleParticleDepositionHandler : ISingleParticleDepositionHandler
    {
        private readonly IConfig _config;
        private readonly double _searchRadius;

        public BallisticSingleParticleDepositionHandler(IConfig config, double searchRadius)
        {
            _config = config;
            _searchRadius = searchRadius;
        }

        public double GetMinDepositionDistance(PrimaryParticle primaryParticle, IEnumerable<PrimaryParticle> primaryParticles)
        {
            var neighbors = GetNeighbors(primaryParticle, primaryParticles);

            var distances = new List<double>();
            
            // This first entry is in case there is no neighbor that gets hit
            distances.Add(_config.LargeNumber);
            foreach (var neighbor in neighbors)
            {
                var neighborRadius = ParticleFormationService.GetRadiusOfXYNodePrimaryParticle(neighbor.Node.Position, primaryParticles);
                var neighbor3DPosition = Get3DPositionFrom2DProjection(neighbor.Node.Position, primaryParticles);
                distances.Add(Get1DDistanceToNeighbor(primaryParticle, neighbor3DPosition, neighborRadius));
            }

            return distances.Min();
        }

        private double[] Get3DPositionFrom2DProjection(double[] position, IEnumerable<PrimaryParticle> primaryParticles)
        {
            foreach(var particle in primaryParticles)
            {
                if (Math.Abs(particle.Position.X - position[0]) < 1e-6 &&
                    Math.Abs(particle.Position.Y - position[1]) < 1e-6)
                {
                    return particle.Position.ToArray();
                }
            }

            throw new ArgumentException("Neighborlist 2D->3D conversion error: Could not find primary particle");
        }

        public double Get1DDistanceToNeighbor(PrimaryParticle primaryParticle, double[] neighborPosition, double neighborRadius)
        {
            var distanceToCenterline = primaryParticle.GetDistanceToVerticalAxis(neighborPosition);
            var combinedRadius = neighborRadius + primaryParticle.Radius;
            
            // If the neighbor is close but it won't be hit during deposition return a large number
            if (distanceToCenterline > combinedRadius)
            {
                return _config.LargeNumber;
            }
            
            var extraHeight = Math.Sqrt(Math.Pow(combinedRadius, 2) - Math.Pow(distanceToCenterline, 2));
            var distance = primaryParticle.Position.Z - neighborPosition[2] - extraHeight;

            return distance;
        }

        public List<NodeDistance<KDTreeNode<double>>> GetNeighbors(PrimaryParticle primaryParticle, IEnumerable<PrimaryParticle> primaryParticles)
        {
            var neighborslist = primaryParticles.ToXYNeighborsList();

            var neighbors = neighborslist.Nearest
            (
                position: primaryParticle.Position.ToXYArray(),
                radius: (primaryParticle.Radius + _searchRadius) * _config.Delta
            );
            return neighbors;
        }
    }
}
