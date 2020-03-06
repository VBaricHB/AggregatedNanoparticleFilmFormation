﻿using System.Linq;
using Xunit;
using System;
using Export;
using Common;

namespace AggregateFormation.tests
{
    public class FormClusterTest
    {

        [Fact]
        public void FormClusterOfTwoParticlesTest()
        {
            CustomConfig config = new CustomConfig();
            var psd = new MonoDisperseSizeDistribution(5);
            var seed = 1;
            var pca = new ParticleClusterAggregationFactory(psd, config,seed);
            var cluster = pca.Build(2);
            Assert.Equal(2, cluster.PrimaryParticles.Count());
            var dist = Math.Round(config.Epsilon
                * (cluster.PrimaryParticles[0].Radius
                + cluster.PrimaryParticles[1].Radius),6);
            Assert.Equal(dist, CalcDistance.Distance(cluster.PrimaryParticles[0], cluster.PrimaryParticles[1]));
        }

        [Fact]
        public void FormClusterOfThreeParticlesTest()
        {
            CustomConfig config = new CustomConfig();
            var psd = new MonoDisperseSizeDistribution(5);
            var seed = 1;
            var pca = new ParticleClusterAggregationFactory(psd, config, seed);
            var cluster = pca.Build(3);
            Assert.Equal(3, cluster.PrimaryParticles.Count());
            var dist = Math.Round(cluster.PrimaryParticles[0].Radius + cluster.PrimaryParticles[1].Radius,6);
            var realDist = CalcDistance.Distance(cluster.PrimaryParticles[0], cluster.PrimaryParticles[1]);
            Assert.True(realDist >= config.Epsilon * dist);
            Assert.True(realDist <= config.Delta * dist);
        }

        [Fact]
        public void FormClusterOfFourParticlesTest()
        {
            CustomConfig config = new CustomConfig();
            var psd = new MonoDisperseSizeDistribution(5);
            var seed = 1;
            var pca = new ParticleClusterAggregationFactory(psd, config, seed);
            var cluster = pca.Build(4);
            Assert.Equal(4, cluster.PrimaryParticles.Count());
            var dist = Math.Round(cluster.PrimaryParticles[1].Radius + cluster.PrimaryParticles[2].Radius, 6);
            var realDist = CalcDistance.Distance(cluster.PrimaryParticles[0], cluster.PrimaryParticles[1]);
            Assert.True(realDist >= config.Epsilon * dist);
            Assert.True(realDist <= config.Delta * dist);
            var export = new ExportToLAMMPS(cluster);
            export.WriteToFile("ClusterOf4.trj");
        }

        [Fact]
        public void FormClusterOfTwentyParticlesTest()
        {
            CustomConfig config = new CustomConfig();
            var psd = new MonoDisperseSizeDistribution(5);
            var seed = 1;
            var pca = new ParticleClusterAggregationFactory(psd, config, seed);
            var cluster = pca.Build(20);
            Assert.Equal(20, cluster.PrimaryParticles.Count());
            var dist = Math.Round(cluster.PrimaryParticles[1].Radius + cluster.PrimaryParticles[2].Radius, 6);
            var realDist = CalcDistance.Distance(cluster.PrimaryParticles[0], cluster.PrimaryParticles[1]);
            Assert.True(realDist >= config.Epsilon * dist);
            Assert.True(realDist <= config.Delta * dist);
            var export = new ExportToLAMMPS(cluster);
            export.WriteToFile("ClusterOf20.trj");
        }

    }
}
