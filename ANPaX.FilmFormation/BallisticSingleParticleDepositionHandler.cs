using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Accord.Collections;

using ANPaX.Core;
using ANPaX.Core.Neighborslist;
using ANPaX.Core.Extensions;
using ANPaX.FilmFormation.interfaces;

namespace ANPaX.FilmFormation
{
    internal class BallisticSingleParticleDepositionHandler : ISingleParticleDepositionHandler
    {
        private readonly IFilmFormationConfig _config;

        public BallisticSingleParticleDepositionHandler(IFilmFormationConfig config)
        {
            _config = config;
        }

        public double GetMinDepositionDistance(
            PrimaryParticle primaryParticle,
            IEnumerable<PrimaryParticle> depositedPrimaryParticles,
            INeighborslist neighborsList2D,
            double maxRadius)
        {
            var distance = _config.LargeNumber;
            if (!depositedPrimaryParticles.Any())
            {
                return distance;
            }

            var searchRadius = (primaryParticle.Radius + maxRadius) * _config.Delta;
            var neighbors = neighborsList2D.GetPrimaryParticlesWithinRadius(primaryParticle.Position, searchRadius);
            if (!neighbors.Any())
            {
                return distance;
            }
            foreach (var neighbor in neighbors)
            {
                distance = Math.Min(distance, Get1DDistanceToNeighbor(primaryParticle, neighbor));
            }

            return distance;
        }

        internal double Get1DDistanceToNeighbor(PrimaryParticle primaryParticle, PrimaryParticle neighbor)
        {
            var distanceToCenterline = primaryParticle.GetDistanceToVerticalAxis(neighbor);
            var combinedRadius = neighbor.Radius + primaryParticle.Radius;

            // If the neighbor is close but it won't be hit during deposition return a large number
            if (distanceToCenterline > combinedRadius)
            {
                return _config.LargeNumber;
            }

            var extraHeight = Math.Sqrt(Math.Pow(combinedRadius, 2) - Math.Pow(distanceToCenterline, 2));
            var distance = primaryParticle.Position.Z - neighbor.Position.Z - extraHeight;

            return distance;
        }

    }
}
