using System;
using System.Collections.Generic;
using System.IO;

using ANPaX.AggregateFormation.interfaces;
using ANPaX.Collection;
using ANPaX.Extensions;

using Moq;

using NLog;

using Xunit;

namespace ANPaX.AggregateFormation.tests
{
    public class FormAggregateTest
    {
        private string _resources = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\Resources\\"));
        private static int _seed = 100;
        private Random _rndGen = new Random(_seed);
        private IAggregateFormationConfig _config = new TestAggregateFormationConfig();
        private ILogger _logger = new Mock<ILogger>().Object;
        private ISizeDistribution<double> _monoPSD = new MonodispersePrimaryParticleSizeDistribution(5);

        [Fact]
        public void CorrectClusterSizeTest()
        {
            var clusterSizes = ClusterClusterAggregationFactory.GetClusterSizes(20, _config);
            Assert.Equal(4, ClusterClusterAggregationFactory.GetNumberOfCluster(20, _config));
            Assert.Equal(new List<int>() { 5, 5, 5, 5 }, clusterSizes);
        }

        [Fact]
        public void BuildAggregateTest()
        {

            var cca = new ClusterClusterAggregationFactory(_monoPSD, _config, _logger, _seed);
            var agg = cca.Build(24);
            Assert.Equal(24, agg.NumberOfPrimaryParticles);
        }

        [Fact]
        public void BuildPolydisperseAggregateTest()
        {
            var file = _resources + "FSP_PrimaryParticleSizeDistribution.xml";
            var dist = XMLSizeDistributionBuilder<double>.Read(file);
            var psd = new TabulatedPrimaryParticleSizeDistribution(dist, _rndGen, _config, integrate: true);
            var cca = new ClusterClusterAggregationFactory(psd, _config, _logger, _seed);
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
            var cca = new ClusterClusterAggregationFactory(psd, _config, _logger, _seed);
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

            var pp1 = new PrimaryParticle(1, new Vector3(0, 0, 0), 5);
            var pp2 = new PrimaryParticle(2, new Vector3(0, 0, 10), 5);

            var cluster1 = new Cluster(1, new List<PrimaryParticle>() { pp1, pp2 });
            var depositedCluster = new List<Cluster>
            {
                cluster1
            };

            var tree = cluster1.PrimaryParticles.ToNeighborsList();

            var pp3 = new PrimaryParticle(3, new Vector3(0, 0, 25), 5);
            var pp4 = new PrimaryParticle(4, new Vector3(0, 0, 19), 5);
            var pp5 = new PrimaryParticle(4, new Vector3(0, 0, 20.02), 5);

            var (anyNearby1, allFeasible1) = ClusterClusterAggregationFactory.IsPrimaryParticlePositionValid(tree, pp3, depositedCluster.GetPrimaryParticles(), _config);

            Assert.False(anyNearby1);
            Assert.True(allFeasible1);

            var (anyNearby2, allFeasible2) = ClusterClusterAggregationFactory.IsPrimaryParticlePositionValid(tree, pp4, depositedCluster.GetPrimaryParticles(), _config);

            Assert.True(anyNearby2);
            Assert.False(allFeasible2);

            var (anyNearby3, allFeasible3) = ClusterClusterAggregationFactory.IsPrimaryParticlePositionValid(tree, pp5, depositedCluster.GetPrimaryParticles(), _config);

            Assert.True(anyNearby3);
            Assert.True(allFeasible3);

        }

        [Fact]
        public void TrySetClusterTest()
        {
            var pp1 = new PrimaryParticle(1, new Vector3(0, 0, 0), 5);
            var pp2 = new PrimaryParticle(2, new Vector3(0, 0, 10), 5);

            var cluster1 = new Cluster(1, new List<PrimaryParticle>() { pp1, pp2 });
            var depositedCluster = new List<Cluster>
            {
                cluster1
            };
            var tree = cluster1.PrimaryParticles.ToNeighborsList();

            var pp3 = new PrimaryParticle(3, new Vector3(0, 0, 30), 5);
            var pp4 = new PrimaryParticle(4, new Vector3(0, 0, 20), 5);
            var cluster2 = new Cluster(1, new List<PrimaryParticle>() { pp3, pp4 });
            var position = new Vector3(0, 0, 25.02);
            cluster2.MoveTo(position);
            var set = ClusterClusterAggregationFactory.IsClusterPositionValid(cluster2, tree, depositedCluster, _config);
            Assert.True(set);

            var position2 = new Vector3(0, 0, 24);
            cluster2.MoveTo(position2);
            var set2 = ClusterClusterAggregationFactory.IsClusterPositionValid(cluster2, tree, depositedCluster, _config);
            Assert.False(set2);
        }

        [Fact]
        public void NoOverlappingParticlesTest()
        {
            var cca = new ClusterClusterAggregationFactory(_monoPSD, _config, _logger, _seed);
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
                            Assert.True(distance >= (pp1.Radius + pp2.Radius));
                        }
                    }
                }
            }
        }
    }
}
