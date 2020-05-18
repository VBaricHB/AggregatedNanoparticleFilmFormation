using System;
using System.Collections.Generic;
using System.IO;

using ANPaX.Core;
using ANPaX.Core.interfaces;
using ANPaX.Simulation.AggregateFormation.interfaces;

using Moq;

using NLog;

using Xunit;

namespace ANPaX.Simulation.AggregateFormation.tests
{
    public class FormAggregateTest
    {
        private string _resources = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\Resources\\"));
        private static int _seed = 100;
        private Random _rndGen = new Random(_seed);
        private IAggregateFormationConfig _config = new TestAggregateFormationConfig();
        private ILogger _logger = new Mock<ILogger>().Object;
        private ISizeDistribution<double> _monoPSD = new MonodispersePrimaryParticleSizeDistribution(5);
        private INeighborslistFactory _neighborslistFactory = new AccordNeighborslistFactory();

        [Fact]
        public void CorrectClusterSizeTest()
        {
            var ccaf = new ClusterClusterAggregationFactory(_monoPSD, _config, _logger, _neighborslistFactory);
            var clusterSizes = ccaf.GetClusterSizes(20);
            Assert.Equal(4, ccaf.GetNumberOfCluster(20));
            Assert.Equal(new List<int>() { 5, 5, 5, 5 }, clusterSizes);
        }

        [Fact]
        public void BuildAggregateTest()
        {

            var cca = new ClusterClusterAggregationFactory(_monoPSD, _config, _logger, _neighborslistFactory, _seed);
            var agg = cca.Build(24);
            Assert.Equal(24, agg.NumberOfPrimaryParticles);
        }

        [Fact]
        public void BuildPolydisperseAggregateTest()
        {
            var file = _resources + "FSP_PrimaryParticleSizeDistribution.xml";
            var dist = XMLSizeDistributionBuilder<double>.Read(file);
            var psd = new TabulatedPrimaryParticleSizeDistribution(dist, _rndGen, _config, integrate: true);
            var cca = new ClusterClusterAggregationFactory(psd, _config, _logger, _neighborslistFactory, _seed);
            var agg = cca.Build(24);
            Assert.Equal(24, agg.NumberOfPrimaryParticles);
        }

        [Fact]
        public void BuildMultiplePolydisperseAggregatesTest()
        {
            var file = _resources + "FSP_PrimaryParticleSizeDistribution.xml";
            var dist = XMLSizeDistributionBuilder<double>.Read(file);
            var psd = new TabulatedPrimaryParticleSizeDistribution(dist, _rndGen, _config, integrate: true);
            var aggs = new List<Aggregate>();
            var cca = new ClusterClusterAggregationFactory(psd, _config, _logger, _neighborslistFactory, _seed);
            for (var i = 0; i < 4; i++)
            {
                var agg = cca.Build(24);
                Assert.Equal(24, agg.NumberOfPrimaryParticles);
                aggs.Add(agg);
            }

        }

        [Fact]
        public void FeasiblePositionTest()
        {
            var logger = new Mock<ILogger>().Object;
            var rndGen = new Random(_seed);

            var cca = new ClusterClusterAggregationFactory(_monoPSD, _config, _logger, _neighborslistFactory, _seed);

            var pp1 = new PrimaryParticle(1, new Vector3(0, 0, 0), 5);
            var pp2 = new PrimaryParticle(2, new Vector3(0, 0, 10), 5);

            var cluster1 = new Cluster(1, new List<PrimaryParticle>() { pp1, pp2 });
            var depositedCluster = new List<Cluster>
            {
                cluster1
            };

            var neighborslist = _neighborslistFactory.Build3DNeighborslist(cluster1.PrimaryParticles);


            var pp3 = new PrimaryParticle(3, new Vector3(0, 0, 25), 5);
            var pp4 = new PrimaryParticle(4, new Vector3(0, 0, 19), 5);
            var pp5 = new PrimaryParticle(4, new Vector3(0, 0, 20.02), 5);

            var (anyNearby1, allFeasible1) = cca.IsPrimaryParticlePositionValid(neighborslist, pp3, depositedCluster.GetPrimaryParticles());

            Assert.False(anyNearby1);
            Assert.True(allFeasible1);

            var (anyNearby2, allFeasible2) = cca.IsPrimaryParticlePositionValid(neighborslist, pp4, depositedCluster.GetPrimaryParticles());

            Assert.True(anyNearby2);
            Assert.False(allFeasible2);

            var (anyNearby3, allFeasible3) = cca.IsPrimaryParticlePositionValid(neighborslist, pp5, depositedCluster.GetPrimaryParticles());

            Assert.True(anyNearby3);
            Assert.True(allFeasible3);

        }

        [Fact]
        public void TrySetClusterTest()
        {
            var pp1 = new PrimaryParticle(1, new Vector3(0, 0, 0), 5);
            var pp2 = new PrimaryParticle(2, new Vector3(0, 0, 10), 5);
            var cca = new ClusterClusterAggregationFactory(_monoPSD, _config, _logger, _neighborslistFactory, _seed);
            var cluster1 = new Cluster(1, new List<PrimaryParticle>() { pp1, pp2 });
            var depositedCluster = new List<Cluster>
            {
                cluster1
            };
            var neighborslist = _neighborslistFactory.Build3DNeighborslist(cluster1.PrimaryParticles);

            var pp3 = new PrimaryParticle(3, new Vector3(0, 0, 30), 5);
            var pp4 = new PrimaryParticle(4, new Vector3(0, 0, 20), 5);
            var cluster2 = new Cluster(1, new List<PrimaryParticle>() { pp3, pp4 });
            var position = new Vector3(0, 0, 25.02);
            cluster2.MoveTo(position);
            var set = cca.IsClusterPositionValid(cluster2, neighborslist, depositedCluster);
            Assert.True(set);

            var position2 = new Vector3(0, 0, 24);
            cluster2.MoveTo(position2);
            var set2 = cca.IsClusterPositionValid(cluster2, neighborslist, depositedCluster);
            Assert.False(set2);
        }

        [Fact]
        public void NoOverlappingParticlesTest()
        {
            var cca = new ClusterClusterAggregationFactory(_monoPSD, _config, _logger, _neighborslistFactory, _seed);
            var logger = new Mock<ILogger>().Object;
            var rndGen = new Random(_seed);
            for (var i = 0; i < 100; i++)
            {
                var agg = cca.Build(24);

                foreach (var pp1 in agg.Cluster.GetPrimaryParticles())
                {
                    foreach (var pp2 in agg.Cluster.GetPrimaryParticles())
                    {
                        if (pp1 != pp2)
                        {
                            var distance = pp1.GetDistanceToPrimaryParticle(pp2);
                            Assert.True(distance >= pp1.Radius + pp2.Radius);
                        }
                    }
                }
            }
        }
    }
}
