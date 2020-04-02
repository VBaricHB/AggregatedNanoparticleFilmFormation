using Accord.Collections;
using CommonLibrary;
using CommonLibrary.interfaces;
using FilmFormationLibrary.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FilmFormationLibrary
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
                var neighborRadius = ParticleFormationService.GetRadiusOfNodePrimaryParticle(neighbor.Node.Position, primaryParticles);
                distances.Add(Get1DDistanceToNeighbor(primaryParticle, neighbor.Node.Position, neighborRadius));
            }

            return distances.Min();
        }

        public double Get1DDistanceToNeighbor(PrimaryParticle primaryParticle, double[] neighborPosition, double neighborRadius)
        {
            var distanceToCenterline = ParticleFormationService.GetDistanceToCenterline(primaryParticle, neighborPosition);
            var combinedRadius = neighborRadius + primaryParticle.Radius;
            
            // If the neighbor is close but it won't be hit during deposition return a large number
            if (distanceToCenterline > combinedRadius)
            {
                return _config.LargeNumber;
            }
            
            var extraHeight = Math.Sqrt(Math.Pow(combinedRadius, 2) - Math.Pow(distanceToCenterline, 2));
            var distance = primaryParticle.Position.Z - neighborPosition[2] + extraHeight;

            return distance;
        }

        private List<NodeDistance<KDTreeNode<double>>> GetNeighbors(PrimaryParticle primaryParticle, IEnumerable<PrimaryParticle> primaryParticles)
        {
            var neighborslist = ParticleFormationService.BuildNeighborsList(primaryParticles);

            var neighbors = neighborslist.Nearest
            (
                position: primaryParticle.Position.ToArray(),
                radius: (primaryParticle.Radius + _searchRadius) * _config.Delta
            );
            return neighbors;
        }
    }
}
