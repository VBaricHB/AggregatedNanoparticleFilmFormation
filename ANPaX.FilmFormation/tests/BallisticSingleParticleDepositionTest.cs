﻿using System;
using System.Collections.Generic;
using System.Linq;

using ANPaX.Collection;
using ANPaX.Core.Neighborslist;
using ANPaX.Extensions;
using ANPaX.FilmFormation.interfaces;

using Xunit;

namespace ANPaX.FilmFormation.tests
{
    public class BallisticSingleParticleDepositionTest
    {

        private IFilmFormationConfig _config = new TestFilmFormationConfig();

        private INeighborslistFactory _neighborslistFactory = new AccordNeighborslistFactory();

        [Fact]
        private void DistanceToCenterLine_CorrectDistanceComputed()
        {
            var r = 5;
            var pos = new Vector3(0, 0, 100);
            var primaryParticle = new PrimaryParticle(0, pos, r);

            var neighborPosposition = new[] { 1.0, 1.0 };

            var dist = primaryParticle.GetDistanceToVerticalAxis(neighborPosposition);

            Assert.Equal(Math.Sqrt(2), dist);
        }

        [Fact]
        private void GetNeighbors_3OtherPrimaryParticlesAre2DClose_1isTooFarAway()
        {
            var r = 5;
            var pos = new Vector3(0, 0, 100);
            var primaryParticle = new PrimaryParticle(0, pos, r);

            var handler = new BallisticSingleParticleDepositionHandler(_config);

            var pos2 = new Vector3(0, 3, 10);
            var pos3 = new Vector3(3, -3, 10);
            var pos4 = new Vector3(-3, -3, 10);
            var pos5 = new Vector3(-30, -30, 10);
            var pp2 = new PrimaryParticle(1, pos2, r);
            var pp3 = new PrimaryParticle(2, pos3, r);
            var pp4 = new PrimaryParticle(3, pos4, r);
            var pp5 = new PrimaryParticle(4, pos5, r);

            var particles = new List<PrimaryParticle>() { pp2, pp3, pp4, pp5 };
            var neighborsList = _neighborslistFactory.Build2DNeighborslist(particles);
            var maxRadius = particles.GetMaxRadius();
            var searchRadius = (primaryParticle.Radius + maxRadius) * _config.Delta;
            var neighbors = neighborsList.GetPrimaryParticlesWithinRadius(primaryParticle.Position, searchRadius);
            //var neighbors = handler.GetNeighbors(primaryParticle, neighborsList, maxRadius);

            Assert.Equal(3, neighbors.Count());
            foreach (var neighbor in neighbors)
            {
                Assert.NotEqual(pos5, neighbor.Position);
            }
        }

        [Fact]
        private void ComputeCorrectDepositionDistance_CollisionWithParticle()
        {
            var r = 1.0;
            var pos = new Vector3(0, 0, 100);
            var primaryParticle = new PrimaryParticle(0, pos, r);

            var handler = new BallisticSingleParticleDepositionHandler(_config);

            var neighborPosition = new double[] { 1, 0, 10 };
            var neighborRadius = 1.0;

            var dist = handler.Get1DDistanceToNeighbor(primaryParticle, neighborPosition, neighborRadius);

            // Math.Sqrt(3) results from the square of the combined radius (4) - the distance to centerline. 
            // This origins from the triangle: final position pp1. position neigbor, center projection neighbor.
            var shouldBeDistance = 90 - Math.Sqrt(3);
            Assert.Equal(shouldBeDistance, dist);
        }

        [Fact]
        private void ComputeCorrectDepositionDistance_NoCollisionWithParticle()
        {
            var r = 1.0;
            var pos = new Vector3(0, 0, 100);
            var primaryParticle = new PrimaryParticle(0, pos, r);

            var handler = new BallisticSingleParticleDepositionHandler(_config);

            var neighborPosition = new double[] { 5, 0, 10 };
            var neighborRadius = 1.0;

            var dist = handler.Get1DDistanceToNeighbor(primaryParticle, neighborPosition, neighborRadius);

            // Math.Sqrt(3) results from the square of the combined radius (4) - the distance to centerline. 
            // This origins from the triangle: final position pp1. position neigbor, center projection neighbor.
            var shouldBeDistance = _config.LargeNumber;
            Assert.Equal(shouldBeDistance, dist);
        }

        [Fact]
        private void ComputeCorrectMinDepositionDistance_CollisionWithPrimaryParticleOnlyOneNeighbor()
        {
            var r = 1.0;
            var pos = new Vector3(0, 0, 100);
            var primaryParticle = new PrimaryParticle(0, pos, r);

            var handler = new BallisticSingleParticleDepositionHandler(_config);

            var pos2 = new Vector3(1, 0, 10);
            var pp2 = new PrimaryParticle(1, pos2, r);
            var otherParticles = new List<PrimaryParticle>() { pp2 };
            var neighborsList = _neighborslistFactory.Build2DNeighborslist(otherParticles);

            var dist = handler.GetMinDepositionDistance(primaryParticle, otherParticles, neighborsList);

            // Math.Sqrt(3) results from the square of the combined radius (4) - the distance to centerline. 
            // This origins from the triangle: final position pp1. position neigbor, center projection neighbor.
            var shouldBeDistance = 90 - Math.Sqrt(3);
            Assert.Equal(shouldBeDistance, dist);
        }

        [Fact]
        private void ComputeCorrectMinDepositionDistance_CollisionWithPrimaryParticleTwoNeighbors()
        {
            var r = 1.0;
            var pos = new Vector3(0, 0, 100);
            var primaryParticle = new PrimaryParticle(0, pos, r);

            var handler = new BallisticSingleParticleDepositionHandler(_config);

            var pos2 = new Vector3(1, 0, 10);
            var pp2 = new PrimaryParticle(1, pos2, r);
            var pos3 = new Vector3(1, 0, 5);
            var pp3 = new PrimaryParticle(1, pos3, r);
            var otherParticles = new List<PrimaryParticle>() { pp2, pp3 };

            var neighborsList = _neighborslistFactory.Build2DNeighborslist(otherParticles);

            var dist = handler.GetMinDepositionDistance(primaryParticle, otherParticles, neighborsList);

            var shouldBeDistance = 90 - Math.Sqrt(3);
            Assert.Equal(shouldBeDistance, dist);
        }

        [Fact]
        private void ComputeCorrectMinDepositionDistance_NoCollision()
        {
            var r = 1.0;
            var pos = new Vector3(0, 0, 100);
            var primaryParticle = new PrimaryParticle(0, pos, r);

            var handler = new BallisticSingleParticleDepositionHandler(_config);

            var pos2 = new Vector3(5, 0, 10);
            var pp2 = new PrimaryParticle(1, pos2, r);
            var otherParticles = new List<PrimaryParticle>() { pp2 };

            var neighborsList = _neighborslistFactory.Build2DNeighborslist(otherParticles);

            var dist = handler.GetMinDepositionDistance(primaryParticle, otherParticles, neighborsList);

            var shouldBeDistance = _config.LargeNumber;
            Assert.Equal(shouldBeDistance, dist);
        }
    }
}
