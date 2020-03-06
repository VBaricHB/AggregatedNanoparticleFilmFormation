using Export;
using System;
using System.Collections.Generic;
using Xunit;

namespace AggregateFormation.tests
{
    public class FormAggregateTest
    {
        [Fact]
        public void CorrectClusterSizeTest()
        {
            var aggregateFactory = new ClusterClusterAggregationFactory(6, new MonoDisperseSizeDistribution(5), new CustomConfig());
            var clusterSizes = aggregateFactory.GetClusterSizes(20);
            Assert.Equal(4, aggregateFactory.GetNumberOfCluster(20));
            Assert.Equal(new List<int>() { 5, 5, 5, 5 }, clusterSizes);
        }

        [Fact]
        public void BuildAggregateTest()
        {
            var aggregateFactory = new ClusterClusterAggregationFactory(6, new MonoDisperseSizeDistribution(5), new CustomConfig());
            var agg = aggregateFactory.Build(24);
            Assert.Equal(24, agg.NumberOfPrimaryParticles);
            var export = new ExportToLAMMPS(agg);
            export.WriteToFile("AggregateFormationTest.trj");
        }
    }
}
