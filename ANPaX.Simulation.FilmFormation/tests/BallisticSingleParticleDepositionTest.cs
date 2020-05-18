using System;
using System.Collections.Generic;
using System.Linq;

using ANPaX.Core;
using ANPaX.Core.interfaces;
using ANPaX.Simulation.FilmFormation.interfaces;

using Xunit;

namespace ANPaX.Simulation.FilmFormation.tests
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

            var neighbor = new PrimaryParticle(1, new Vector3(1, 0, 10), 1.0);

            var dist = BallisticSingleParticleDepositionHandler.Get1DDistanceToNeighbor(primaryParticle, neighbor);

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


            var neighbor = new PrimaryParticle(1, new Vector3(5, 0, 10), 1.0);

            var dist = BallisticSingleParticleDepositionHandler.Get1DDistanceToNeighbor(primaryParticle, neighbor);

            // Math.Sqrt(3) results from the square of the combined radius (4) - the distance to centerline. 
            // This origins from the triangle: final position pp1. position neigbor, center projection neighbor.
            var shouldBeDistance = primaryParticle.Position.Z - primaryParticle.Radius;
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

            var dist = handler.GetDepositionDistance(primaryParticle, otherParticles, neighborsList, otherParticles.GetMaxRadius());

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

            var dist = handler.GetDepositionDistance(primaryParticle, otherParticles, neighborsList, otherParticles.GetMaxRadius());

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

            var dist = handler.GetDepositionDistance(primaryParticle, otherParticles, neighborsList, otherParticles.GetMaxRadius());

            var shouldBeDistance = primaryParticle.Position.Z - primaryParticle.Radius;
            Assert.Equal(shouldBeDistance, dist);
        }
    }
}
