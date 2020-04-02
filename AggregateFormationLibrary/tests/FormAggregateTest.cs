using CommonLibrary;
using Export;
using Moq;
using NLog;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AggregateFormation.tests
{
    public class FormAggregateTest
    {
        [Fact]
        public void CorrectClusterSizeTest()
        {
            var logger = new Mock<ILogger>().Object;
            var aggregateFactory = new ClusterClusterAggregationFactory(6, new MonodispersePrimaryParticleSizeDistribution(5), new CustomConfig(), logger);
            var clusterSizes = aggregateFactory.GetClusterSizes(20);
            Assert.Equal(4, aggregateFactory.GetNumberOfCluster(20));
            Assert.Equal(new List<int>() { 5, 5, 5, 5 }, clusterSizes);
        }

        [Fact]
        public void BuildAggregateTest()
        {
            var logger = new Mock<ILogger>().Object;
            var aggregateFactory = new ClusterClusterAggregationFactory(6, new MonodispersePrimaryParticleSizeDistribution(5), new CustomConfig(), logger);
            var agg = aggregateFactory.Build(24);
            Assert.Equal(24, agg.NumberOfPrimaryParticles);
            var export = new ExportToLAMMPS(agg);
            export.WriteToFile("AggregateFormationTest.trj");
        }

        [Fact]
        public void FeasiblePositionTest()
        {
            var logger = new Mock<ILogger>().Object;
            var aggregateFactory = new ClusterClusterAggregationFactory(6, new MonodispersePrimaryParticleSizeDistribution(5), new CustomConfig(), logger);

            var pp1 = new PrimaryParticle(1, new Vector3(0, 0, 0), 5);
            var pp2 = new PrimaryParticle(2, new Vector3(0, 0, 10), 5);
                        
            var cluster1 = new Cluster(1, new List<PrimaryParticle>() { pp1, pp2 });
            var depositedCluster = new List<Cluster>();
            depositedCluster.Add(cluster1);

            var tree = ParticleFormationService.BuildNeighborsList(cluster1.PrimaryParticles);

            var pp3 = new PrimaryParticle(3, new Vector3(0, 0, 25), 5);
            var pp4 = new PrimaryParticle(4, new Vector3(0, 0, 19), 5);
            var pp5 = new PrimaryParticle(4, new Vector3(0, 0, 20), 5);

            var (anyNearby1, allFeasible1) = aggregateFactory.IsPrimaryParticleValid(tree, pp3, depositedCluster);

            Assert.False(anyNearby1);
            Assert.True(allFeasible1);

            var (anyNearby2, allFeasible2) = aggregateFactory.IsPrimaryParticleValid(tree, pp4, depositedCluster);

            Assert.True(anyNearby2);
            Assert.False(allFeasible2);

            var (anyNearby3, allFeasible3) = aggregateFactory.IsPrimaryParticleValid(tree, pp5, depositedCluster);

            Assert.True(anyNearby3);
            Assert.True(allFeasible3);

        }

        [Fact]
        public void TrySetClusterTest()
        {
            var logger = new Mock<ILogger>().Object;
            var aggregateFactory = new ClusterClusterAggregationFactory(6, new MonodispersePrimaryParticleSizeDistribution(5), new CustomConfig(), logger);

            var pp1 = new PrimaryParticle(1, new Vector3(0, 0, 0), 5);
            var pp2 = new PrimaryParticle(2, new Vector3(0, 0, 10), 5);

            var cluster1 = new Cluster(1, new List<PrimaryParticle>() { pp1, pp2 });
            var depositedCluster = new List<Cluster>();
            depositedCluster.Add(cluster1);
            var tree = ParticleFormationService.BuildNeighborsList(cluster1.PrimaryParticles);
            
            var pp3 = new PrimaryParticle(3, new Vector3(0, 0, 30), 5);
            var pp4 = new PrimaryParticle(4, new Vector3(0, 0, 20), 5);
            var cluster2 = new Cluster(1, new List<PrimaryParticle>() { pp3, pp4 });
            var position = new Vector3(0, 0, 25);
            var set = aggregateFactory.TrySetCluster(cluster2, position, tree, depositedCluster);
            Assert.True(set);

            var position2 = new Vector3(0, 0, 24);
            var set2 = aggregateFactory.TrySetCluster(cluster2, position2, tree, depositedCluster);
            Assert.False(set2);
        }

        [Fact]
        public void NoOverlappingParticlesTest()
        {
            for (var i = 0; i < 100; i++)
            {
                var logger = new Mock<ILogger>().Object;
                var aggregateFactory = new ClusterClusterAggregationFactory(6, new MonodispersePrimaryParticleSizeDistribution(5), new CustomConfig(), logger);
                var agg = aggregateFactory.Build(24);


                foreach (var pp1 in agg.Cluster.SelectMany(c => c.PrimaryParticles))
                {
                    foreach (var pp2 in agg.Cluster.SelectMany(c => c.PrimaryParticles))
                    {
                        if (pp1 != pp2)
                        {
                            var distance = ParticleFormationService.Distance(pp1, pp2);
                            Assert.True(distance >= (pp1.Radius + pp2.Radius));
                        }
                    }
                }
            }
        }
    }
}
