using System.Linq;
using Xunit;
using System;
using ANPaX.Export;
using Moq;
using NLog;
using ANPaX.Extensions;
using System.IO;
using ANPaX.Collection;
using System.Collections.Generic;

namespace ANPaX.AggregateFormation.tests
{
    public class FormClusterTest
    {
        private string _resources = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\Resources\\"));
        private int _seed = 100;
        private double _tolerance = 1e-6;


        [Fact]
        public void RandomPositionGenerator_CorrectDistanceFromOrigin()
        {
            var random = new Random(_seed);
            var distance = 10.0;
            var position = ParticleFormationService.GetRandomPosition(random, distance);
            var vectorLength = Math.Sqrt(Math.Pow(position.X, 2) + Math.Pow(position.Y, 2) + Math.Pow(position.Z, 2));
            Assert.True(Math.Abs(distance - vectorLength) < _tolerance);
        }

        [Fact]
        public void TrySetPrimaryParticle_CorrectDetectionTest()
        {
            var logger = new Mock<ILogger>().Object;
            CustomConfig config = new CustomConfig();
            var rndGen = new Random(_seed);
            var psd = new MonodispersePrimaryParticleSizeDistribution(5);

            var pp1 = new PrimaryParticle(1, new Vector3(), 4.5);
            var pp2 = new PrimaryParticle(2, new Vector3(4.035344, 0.969021, 7.427756), 4.0);
            var primaryParticles = new List<PrimaryParticle>() { pp1, pp2 };

            var pp3 = new PrimaryParticle(3, 5.5);
            var com = primaryParticles.GetCenterOfMass();
            var setPosition = new Vector3(-11.098618, 1.316368, -6.026161)+ com;

            var check = ParticleClusterAggregationFactory.TrySetPrimaryParticle(pp3, setPosition, primaryParticles, config);
            Assert.True(check);
        }

        [Fact]
        public void FormClusterOfTwoParticlesTest()
        {
            var logger = new Mock<ILogger>().Object;
            CustomConfig config = new CustomConfig();
            var psd = new MonodispersePrimaryParticleSizeDistribution(5);
            var rndGen = new Random(_seed);
            var cluster = ParticleClusterAggregationFactory.Build(2, psd, rndGen, config, logger);
            Assert.Equal(2, cluster.PrimaryParticles.Count());
            var dist = Math.Round(config.Epsilon
                * (cluster.PrimaryParticles[0].Radius
                + cluster.PrimaryParticles[1].Radius),6);
            Assert.Equal(dist, cluster.PrimaryParticles[0].GetDistanceToPrimaryParticle(cluster.PrimaryParticles[1]));
        }

        [Fact]
        public void FormClusterOfThreeParticlesTest()
        {
            var logger = new Mock<ILogger>().Object;
            CustomConfig config = new CustomConfig();
            var psd = new MonodispersePrimaryParticleSizeDistribution(5);
            var rndGen = new Random(_seed);
            var cluster = ParticleClusterAggregationFactory.Build(3, psd, rndGen, config, logger);
            Assert.Equal(3, cluster.PrimaryParticles.Count());
            var dist = Math.Round(cluster.PrimaryParticles[0].Radius + cluster.PrimaryParticles[1].Radius,6);
            var realDist = cluster.PrimaryParticles[0].GetDistanceToPrimaryParticle(cluster.PrimaryParticles[1]);
            Assert.True(realDist >= config.Epsilon * dist);
            Assert.True(realDist <= config.Delta * dist);
        }

        [Fact]
        public void FormClusterOfFourParticlesTest()
        {
            var logger = new Mock<ILogger>().Object;
            CustomConfig config = new CustomConfig();
            var psd = new MonodispersePrimaryParticleSizeDistribution(5);
            var rndGen = new Random(_seed);
            var cluster = ParticleClusterAggregationFactory.Build(4, psd, rndGen, config, logger);
            Assert.Equal(4, cluster.PrimaryParticles.Count());
            var dist = Math.Round(cluster.PrimaryParticles[1].Radius + cluster.PrimaryParticles[2].Radius, 6);
            var realDist = cluster.PrimaryParticles[0].GetDistanceToPrimaryParticle(cluster.PrimaryParticles[1]);
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
            var rndGen = new Random(_seed);
            var cluster = ParticleClusterAggregationFactory.Build(20, psd, rndGen, config, logger);
            Assert.Equal(20, cluster.PrimaryParticles.Count());
            foreach (var pp1 in cluster.PrimaryParticles)
            {
                foreach(var pp2 in cluster.PrimaryParticles)
                {
                    if(pp1 != pp2)
                    {
                        Assert.True(pp1.GetDistanceToPrimaryParticle(pp2) >= pp1.Radius + pp2.Radius);
                    }
                }
            }
            var export = new ExportToLAMMPS(cluster);
            export.WriteToFile("ClusterOf20.trj");
        }

        [Fact]
        public void FormClusterOfTwoPolydispersePrimaryParticlesTest()
        {
            var logger = new Mock<ILogger>().Object;
            CustomConfig config = new CustomConfig();
            var file = _resources + "FSP_PrimaryParticleSizeDistribution.xml";
            var dist = XMLSizeDistributionBuilder<double>.Read(file);
            var rndGen = new Random(_seed);
            var psd = new TabulatedPrimaryParticleSizeDistribution(dist, rndGen, integrate: true);
            var cluster = ParticleClusterAggregationFactory.Build(2, psd, rndGen, config, logger);
            Assert.Equal(2, cluster.PrimaryParticles.Count());
            var distance = Math.Round(config.Epsilon
                * (cluster.PrimaryParticles[0].Radius
                + cluster.PrimaryParticles[1].Radius), 6);
            Assert.Equal(distance, cluster.PrimaryParticles[0].GetDistanceToPrimaryParticle(cluster.PrimaryParticles[1]));
        }

        [Fact]
        public void FormClusterOfThreePolydisperseParticlesTest()
        {
            var logger = new Mock<ILogger>().Object;
            CustomConfig config = new CustomConfig();
            var file = _resources + "FSP_PrimaryParticleSizeDistribution.xml";
            var dist = XMLSizeDistributionBuilder<double>.Read(file);
            var rndGen = new Random(_seed);
            var psd = new TabulatedPrimaryParticleSizeDistribution(dist, rndGen, integrate: true);
            var cluster = ParticleClusterAggregationFactory.Build(3, psd, rndGen, config, logger);
            Assert.Equal(3, cluster.PrimaryParticles.Count());
            var distance = Math.Round(cluster.PrimaryParticles[0].Radius + cluster.PrimaryParticles[1].Radius, 6);
            var realDist = cluster.PrimaryParticles[0].GetDistanceToPrimaryParticle(cluster.PrimaryParticles[1]);
            Assert.True(realDist >= config.Epsilon * distance);
            Assert.True(realDist <= config.Delta * distance);
        }

        [Fact]
        public void FormClusterOfFourPolydisperseParticlesTest()
        {
            var logger = new Mock<ILogger>().Object;
            CustomConfig config = new CustomConfig();
            var file = _resources + "FSP_PrimaryParticleSizeDistribution.xml";
            var dist = XMLSizeDistributionBuilder<double>.Read(file);
            var rndGen = new Random(_seed);
            var psd = new TabulatedPrimaryParticleSizeDistribution(dist, rndGen, integrate: true);
            var cluster = ParticleClusterAggregationFactory.Build(4, psd, rndGen, config, logger);
            Assert.Equal(4, cluster.PrimaryParticles.Count());
            var export = new ExportToLAMMPS(cluster);
            export.WriteToFile("ClusterOf4.trj");
        }

        [Fact]
        public void FormClusterOfTwentyPolydisperseParticlesTest()
        {
            var logger = new Mock<ILogger>().Object;
            CustomConfig config = new CustomConfig();
            var file = _resources + "FSP_PrimaryParticleSizeDistribution.xml";
            var dist = XMLSizeDistributionBuilder<double>.Read(file);
            var rndGen = new Random(_seed);
            var psd = new TabulatedPrimaryParticleSizeDistribution(dist, rndGen, integrate: true);
            var cluster = ParticleClusterAggregationFactory.Build(20, psd, rndGen, config, logger);
            Assert.Equal(20, cluster.PrimaryParticles.Count());
            foreach (var pp1 in cluster.PrimaryParticles)
            {
                foreach (var pp2 in cluster.PrimaryParticles)
                {
                    if (pp1 != pp2)
                    {
                        Assert.True(pp1.GetDistanceToPrimaryParticle(pp2) >= pp1.Radius + pp2.Radius);
                    }
                }
            }
            var export = new ExportToLAMMPS(cluster);
            export.WriteToFile("ClusterOf20.trj");
        }

        [Fact]
        public void FormMultiplePolydisperseClustersTest_NoInfiniteLoop()
        {
            var logger = new Mock<ILogger>().Object;
            CustomConfig config = new CustomConfig();
            var file = _resources + "FSP_PrimaryParticleSizeDistribution.xml";
            var dist = XMLSizeDistributionBuilder<double>.Read(file);
            var rndGen = new Random(_seed);
            var psd = new TabulatedPrimaryParticleSizeDistribution(dist, rndGen, integrate: true);
            for (var i = 0; i < 4; i++)
            {
                var cluster = ParticleClusterAggregationFactory.Build(6, psd, rndGen, config, logger);
                Assert.Equal(6, cluster.PrimaryParticles.Count());

                foreach (var pp1 in cluster.PrimaryParticles)
                {
                    foreach (var pp2 in cluster.PrimaryParticles)
                    {
                        if (pp1 != pp2)
                        {
                            Assert.True(pp1.GetDistanceToPrimaryParticle(pp2) >= pp1.Radius + pp2.Radius);
                        }
                    }
                }
            }
        }

    }
}
