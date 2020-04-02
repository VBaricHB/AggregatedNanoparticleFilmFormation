using System.Linq;
using Xunit;
using System;
using Export;
using CommonLibrary;
using Moq;
using NLog;

namespace AggregateFormation.tests
{
    public class FormClusterTest
    {

        [Fact]
        public void FormClusterOfTwoParticlesTest()
        {
            var logger = new Mock<ILogger>().Object;
            CustomConfig config = new CustomConfig();
            var psd = new MonodispersePrimaryParticleSizeDistribution(5);
            var seed = 1;
            var pca = new ParticleClusterAggregationFactory(psd, config, logger, seed);
            var cluster = pca.Build(2);
            Assert.Equal(2, cluster.PrimaryParticles.Count());
            var dist = Math.Round(config.Epsilon
                * (cluster.PrimaryParticles[0].Radius
                + cluster.PrimaryParticles[1].Radius),6);
            Assert.Equal(dist, ParticleFormationService.Distance(cluster.PrimaryParticles[0], cluster.PrimaryParticles[1]));
        }

        [Fact]
        public void FormClusterOfThreeParticlesTest()
        {
            var logger = new Mock<ILogger>().Object;
            CustomConfig config = new CustomConfig();
            var psd = new MonodispersePrimaryParticleSizeDistribution(5);
            var seed = 1;
            var pca = new ParticleClusterAggregationFactory(psd, config, logger, seed);
            var cluster = pca.Build(3);
            Assert.Equal(3, cluster.PrimaryParticles.Count());
            var dist = Math.Round(cluster.PrimaryParticles[0].Radius + cluster.PrimaryParticles[1].Radius,6);
            var realDist = ParticleFormationService.Distance(cluster.PrimaryParticles[0], cluster.PrimaryParticles[1]);
            Assert.True(realDist >= config.Epsilon * dist);
            Assert.True(realDist <= config.Delta * dist);
        }

        [Fact]
        public void FormClusterOfFourParticlesTest()
        {
            var logger = new Mock<ILogger>().Object;
            CustomConfig config = new CustomConfig();
            var psd = new MonodispersePrimaryParticleSizeDistribution(5);
            var seed = 1;
            var pca = new ParticleClusterAggregationFactory(psd, config, logger, seed);
            var cluster = pca.Build(4);
            Assert.Equal(4, cluster.PrimaryParticles.Count());
            var dist = Math.Round(cluster.PrimaryParticles[1].Radius + cluster.PrimaryParticles[2].Radius, 6);
            var realDist = ParticleFormationService.Distance(cluster.PrimaryParticles[0], cluster.PrimaryParticles[1]);
            Assert.True(realDist >= config.Epsilon * dist);
            Assert.True(realDist <= config.Delta * dist);
            var export = new ExportToLAMMPS(cluster);
            export.WriteToFile("ClusterOf4.trj");
        }

        [Fact]
        public void FormClusterOfTwentyParticlesTest()
        {
            var logger = new Mock<ILogger>().Object;
            CustomConfig config = new CustomConfig();
            var psd = new MonodispersePrimaryParticleSizeDistribution(5);
            var seed = 1;
            var pca = new ParticleClusterAggregationFactory(psd, config, logger, seed);
            var cluster = pca.Build(20);
            Assert.Equal(20, cluster.PrimaryParticles.Count());
            foreach (var pp1 in cluster.PrimaryParticles)
            {
                foreach(var pp2 in cluster.PrimaryParticles)
                {
                    if(pp1 != pp2)
                    {
                        Assert.True(ParticleFormationService.Distance(pp1, pp2) >= pp1.Radius + pp2.Radius);
                    }
                }
            }
            var export = new ExportToLAMMPS(cluster);
            export.WriteToFile("ClusterOf20.trj");
        }

    }
}
