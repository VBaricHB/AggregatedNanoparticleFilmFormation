using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Accord.Collections;

using ANPaX.Collection;
using ANPaX.Core.Neighborslist;
using ANPaX.Extensions;
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

        public double GetMinDepositionDistance(PrimaryParticle primaryParticle, IEnumerable<PrimaryParticle> depositedPrimaryParticles, INeighborslist neighborsList2D)
        {
            if (!depositedPrimaryParticles.Any())
            {
                return _config.LargeNumber;
            }

            var nListTime = new Stopwatch();
            nListTime.Start();

            //var neighbors = GetNeighbors(primaryParticle, neighborsList, maxRadius);
            var searchRadius = (primaryParticle.Radius + depositedPrimaryParticles.GetMaxRadius()) * _config.Delta;
            var neighbors = neighborsList2D.GetPrimaryParticlesWithinRadius(primaryParticle.Position, searchRadius);

            var nListTimePassed = nListTime.Elapsed;

            var distances = new List<double>
            {
                // This first entry is in case there is no neighbor that gets hit
                _config.LargeNumber
            };

            var nSearchTime = new Stopwatch();
            nSearchTime.Start();
            foreach (var neighbor in neighbors)
            {
                var searchTimeSingle = new Stopwatch();
                searchTimeSingle.Start();
                distances.Add(Get1DDistanceToNeighbor(primaryParticle, neighbor));
                var searchTimeSinglePassed = searchTimeSingle.Elapsed;
            }
            var nSearchTimePassed = nSearchTime.Elapsed;

            var getMinTime = new Stopwatch();
            getMinTime.Start();
            var min = distances.Min();
            var minTimePassed = getMinTime.Elapsed;
            return min;
        }


        //private double[] Get3DPositionFrom2DProjection(double[] position, IEnumerable<PrimaryParticle> primaryParticles)
        //{
        //    foreach (var particle in primaryParticles)
        //    {
        //        if (Math.Abs(particle.Position.X - position[0]) < 1e-6 &&
        //            Math.Abs(particle.Position.Y - position[1]) < 1e-6)
        //        {
        //            return particle.Position.ToArray();
        //        }
        //    }

        //    throw new ArgumentException("Neighborlist 2D->3D conversion error: Could not find primary particle");
        //}

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

        public double Get1DDistanceToNeighbor(PrimaryParticle primaryParticle, PrimaryParticle neighbor)
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

        public List<NodeDistance<KDTreeNode<double>>> GetNeighbors(PrimaryParticle primaryParticle, KDTree<double> neighborsList, double maxRadius)
        {
            var buildTime = new Stopwatch();
            buildTime.Start();
            var buildTimePassed = buildTime.Elapsed;

            var searchTime = new Stopwatch();
            searchTime.Start();
            var neighbors = neighborsList.Nearest
            (
                position: primaryParticle.Position.ToXYArray(),
                radius: (primaryParticle.Radius + maxRadius) * _config.Delta
            );

            var searchTimePassed = searchTime.Elapsed;
            return neighbors;
        }
    }
}
