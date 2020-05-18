using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ANPaX.Core;
using ANPaX.Core.interfaces;
using ANPaX.Simulation.AggregateFormation.interfaces;

using Moq;

using NLog;

using Xunit;

namespace ANPaX.Simulation.AggregateFormation.tests
{
    public class FormClusterTest
    {
        private string _resources = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\Resources\\"));
        private static int _seed = 100;
        private double _tolerance = 1e-6;
        private ILogger _logger = new Mock<ILogger>().Object;
        private Random _rndGen = new Random(_seed);
        private IAggregateFormationConfig _config = new TestAggregateFormationConfig();
        private INeighborslistFactory _neighborslistFactory = new AccordNeighborslistFactory();


        [Fact]
        public void RandomPositionGenerator_CorrectDistanceFromOrigin()
        {
            var distance = 10.0;
            var position = ParticleFormationUtil.GetRandomPosition(_rndGen, distance);
            var vectorLength = Math.Sqrt(Math.Pow(position.X, 2) + Math.Pow(position.Y, 2) + Math.Pow(position.Z, 2));
            Assert.True(Math.Abs(distance - vectorLength) < _tolerance);
        }

        [Fact]
        public void IsPrimaryParticlePositionValid_CorrectDetectionTest()
        {
            var pp1 = new PrimaryParticle(1, new Vector3(), 4.5);
            var pp2 = new PrimaryParticle(2, new Vector3(4.035344, 0.969021, 7.427756), 4.0);
            var primaryParticles = new List<PrimaryParticle>() { pp1, pp2 };

            var pp3 = new PrimaryParticle(3, 5.5);
            var com = primaryParticles.GetCenterOfMass();
            var setPosition = new Vector3(-11.098618, 1.316368, -6.026161) + com;
            var psd = new MonodispersePrimaryParticleSizeDistribution(5);
            var pca = new ParticleClusterAggregationFactory(psd, _rndGen, _config, _neighborslistFactory, _logger);
            var neighborslist = _neighborslistFactory.Build3DNeighborslist(primaryParticles);
            var check = pca.IsPrimaryParticlePositionValid(pp3, setPosition, neighborslist, primaryParticles, _config);
            Assert.True(check);
        }

        [Fact]
        public void FormClusterOfTwoParticlesTest()
        {
            var config = new TestAggregateFormationConfig();
            var psd = new MonodispersePrimaryParticleSizeDistribution(5);
            var pca = new ParticleClusterAggregationFactory(psd, _rndGen, _config, _neighborslistFactory, _logger);
            var cluster = pca.Build(2);
            Assert.Equal(2, cluster.PrimaryParticles.Count());
            var dist = Math.Round(config.Epsilon
                * (cluster.PrimaryParticles[0].Radius
                + cluster.PrimaryParticles[1].Radius), 6);
            Assert.Equal(dist, cluster.PrimaryParticles[0].GetDistanceToPrimaryParticle(cluster.PrimaryParticles[1]));
        }

        [Fact]
        public void FormClusterOfThreeParticlesTest()
        {
            var logger = new Mock<ILogger>().Object;
            var config = new TestAggregateFormationConfig();
            var psd = new MonodispersePrimaryParticleSizeDistribution(5);
            var rndGen = new Random(_seed);
            var pca = new ParticleClusterAggregationFactory(psd, _rndGen, _config, _neighborslistFactory, _logger);
            var cluster = pca.Build(3);
            Assert.Equal(3, cluster.PrimaryParticles.Count());
            var dist = Math.Round(cluster.PrimaryParticles[0].Radius + cluster.PrimaryParticles[1].Radius, 6);
            var realDist = cluster.PrimaryParticles[0].GetDistanceToPrimaryParticle(cluster.PrimaryParticles[1]);
            Assert.True(realDist >= config.Epsilon * dist);
            Assert.True(realDist <= config.Delta * dist);
        }

        [Fact]
        public void FormClusterOfFourParticlesTest()
        {
            var psd = new MonodispersePrimaryParticleSizeDistribution(5);
            var pca = new ParticleClusterAggregationFactory(psd, _rndGen, _config, _neighborslistFactory, _logger);
            var cluster = pca.Build(4);
            Assert.Equal(4, cluster.PrimaryParticles.Count());
            var dist = Math.Round(cluster.PrimaryParticles[1].Radius + cluster.PrimaryParticles[2].Radius, 6);
            var realDist = cluster.PrimaryParticles[0].GetDistanceToPrimaryParticle(cluster.PrimaryParticles[1]);
            Assert.True(realDist >= _config.Epsilon * dist);
            Assert.True(realDist <= _config.Delta * dist);
        }

        [Fact]
        public void FormClusterOfTwentyParticlesTest()
        {
            var psd = new MonodispersePrimaryParticleSizeDistribution(5);
            var pca = new ParticleClusterAggregationFactory(psd, _rndGen, _config, _neighborslistFactory, _logger);
            var cluster = pca.Build(20);
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
        }

        [Fact]
        public void FormClusterOfTwoPolydispersePrimaryParticlesTest()
        {
            var file = _resources + "FSP_PrimaryParticleSizeDistribution.xml";
            var dist = XMLSizeDistributionBuilder<double>.Read(file);
            var psd = new TabulatedPrimaryParticleSizeDistribution(dist, _rndGen, _config, integrate: true);
            var pca = new ParticleClusterAggregationFactory(psd, _rndGen, _config, _neighborslistFactory, _logger);
            var cluster = pca.Build(2);
            Assert.Equal(2, cluster.PrimaryParticles.Count());
            var distance = Math.Round(_config.Epsilon
                * (cluster.PrimaryParticles[0].Radius
                + cluster.PrimaryParticles[1].Radius), 6);
            Assert.Equal(distance, cluster.PrimaryParticles[0].GetDistanceToPrimaryParticle(cluster.PrimaryParticles[1]));
        }

        [Fact]
        public void FormClusterOfThreePolydisperseParticlesTest()
        {
            var file = _resources + "FSP_PrimaryParticleSizeDistribution.xml";
            var dist = XMLSizeDistributionBuilder<double>.Read(file);
            var psd = new TabulatedPrimaryParticleSizeDistribution(dist, _rndGen, _config, integrate: true);
            var pca = new ParticleClusterAggregationFactory(psd, _rndGen, _config, _neighborslistFactory, _logger);
            var cluster = pca.Build(3);
            Assert.Equal(3, cluster.PrimaryParticles.Count());
            var distance = Math.Round(cluster.PrimaryParticles[0].Radius + cluster.PrimaryParticles[1].Radius, 6);
            var realDist = cluster.PrimaryParticles[0].GetDistanceToPrimaryParticle(cluster.PrimaryParticles[1]);
            Assert.True(realDist >= _config.Epsilon * distance);
            Assert.True(realDist <= _config.Delta * distance);
        }

        [Fact]
        public void FormClusterOfFourPolydisperseParticlesTest()
        {

            var file = _resources + "FSP_PrimaryParticleSizeDistribution.xml";
            var dist = XMLSizeDistributionBuilder<double>.Read(file);
            var psd = new TabulatedPrimaryParticleSizeDistribution(dist, _rndGen, _config, integrate: true);
            var pca = new ParticleClusterAggregationFactory(psd, _rndGen, _config, _neighborslistFactory, _logger);
            var cluster = pca.Build(4);
            Assert.Equal(4, cluster.PrimaryParticles.Count());
        }

        [Fact]
        public void FormClusterOfTwentyPolydisperseParticlesTest()
        {
            var file = _resources + "FSP_PrimaryParticleSizeDistribution.xml";
            var dist = XMLSizeDistributionBuilder<double>.Read(file);
            var psd = new TabulatedPrimaryParticleSizeDistribution(dist, _rndGen, _config, integrate: true);
            var pca = new ParticleClusterAggregationFactory(psd, _rndGen, _config, _neighborslistFactory, _logger);
            var cluster = pca.Build(20);
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
        }

        [Fact]
        public void FormMultiplePolydisperseClustersTest_NoInfiniteLoop()
        {
            var file = _resources + "FSP_PrimaryParticleSizeDistribution.xml";
            var dist = XMLSizeDistributionBuilder<double>.Read(file);
            var psd = new TabulatedPrimaryParticleSizeDistribution(dist, _rndGen, _config, integrate: true);
            var pca = new ParticleClusterAggregationFactory(psd, _rndGen, _config, _neighborslistFactory, _logger);
            for (var i = 0; i < 4; i++)
            {
                var cluster = pca.Build(6);
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
