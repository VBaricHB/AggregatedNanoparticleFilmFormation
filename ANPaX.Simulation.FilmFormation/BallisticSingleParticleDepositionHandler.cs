using System;
using System.Collections.Generic;
using System.Linq;

using ANPaX.Core;
using ANPaX.Core.interfaces;
using ANPaX.Simulation.FilmFormation.interfaces;

namespace ANPaX.Simulation.FilmFormation
{
    internal class BallisticSingleParticleDepositionHandler : ISingleParticleDepositionHandler
    {
        private readonly IFilmFormationConfig _config;

        public BallisticSingleParticleDepositionHandler(IFilmFormationConfig config)
        {
            _config = config;
        }

        public double GetDepositionDistance(
            PrimaryParticle primaryParticle,
            IEnumerable<PrimaryParticle> depositedPrimaryParticles,
            INeighborslist neighborsList2D,
            double maxRadius)
        {
            var distance = primaryParticle.Position.Z - primaryParticle.Radius;
            if (!depositedPrimaryParticles.Any())
            {
                return distance;
            }

            var searchRadius = (primaryParticle.Radius + maxRadius) * _config.Delta;
            var neighbors = neighborsList2D.GetPrimaryParticlesWithinRadius(primaryParticle.Position, searchRadius);
            foreach (var neighbor in neighbors)
            {
                distance = Math.Min(distance, Get1DDistanceToNeighbor(primaryParticle, neighbor));
            }

            return distance;
        }

        internal static double Get1DDistanceToNeighbor(PrimaryParticle primaryParticle, PrimaryParticle neighbor)
        {
            var distanceToCenterline = primaryParticle.GetDistanceToVerticalAxis(neighbor);
            var combinedRadius = neighbor.Radius + primaryParticle.Radius;

            // If the neighbor is close but it won't be hit during deposition return max deposition distance
            if (distanceToCenterline > combinedRadius)
            {
                return primaryParticle.Position.Z - primaryParticle.Radius;
            }

            var extraHeight = Math.Sqrt(Math.Pow(combinedRadius, 2) - Math.Pow(distanceToCenterline, 2));
            var distance = primaryParticle.Position.Z - neighbor.Position.Z - extraHeight;

            return distance;
        }

    }
}
