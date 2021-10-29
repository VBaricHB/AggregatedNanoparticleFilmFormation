using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Xunit;

namespace ANPaX.Simulation.AggregateFormation.tests
{
    public class SizeDistributionTest
    {
        private string _resources = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\Resources\\"));
        private int _seed = 100;

        [Fact]
        private void XMLPrimaryParticleSizeDistributionBuilder_FileReadCorrectly()
        {
            var file = _resources + "FSP_PrimaryParticleSizeDistribution.xml";
            var dist = XMLSizeDistributionBuilder<double>.Read(file);
            Assert.Equal(21, dist.Sizes.Length);
            Assert.Equal(1.5, dist.Sizes.First().Value);
            Assert.Equal(11.5, dist.Sizes.Last().Value);
        }

        [Fact]
        private void XMLAggregateSizeDistributionBuilder_FileReadCorrectly()
        {
            var file = _resources + "FSP_AggregateSizeDistribution.xml";
            var dist = XMLSizeDistributionBuilder<int>.Read(file);
            Assert.Equal(116, dist.Sizes.Length);
            Assert.Equal(13, dist.Sizes.First().Value);
            Assert.Equal(128, dist.Sizes.Last().Value);
        }

        [Fact]
        private void TabulatedPrimaryParticleSizeDistributionFromXML_IntegrationCorrectly()
        {
            var file = _resources + "FSP_PrimaryParticleSizeDistribution.xml";
            var dist = XMLSizeDistributionBuilder<double>.Read(file);
            var rndGen = new Random(_seed);
            var config = new TestAggregateFormationConfig();
            var psd = new TabulatedPrimaryParticleSizeDistribution(dist, rndGen, config, integrate: true);
            var list = psd._tabulatedSizeDistribution;

            Assert.Equal(1.0, list.Sizes.Last().Probability);
        }

        [Fact]
        private void LogNormalDistribution_Correct_Mean()
        {
            var expectedMean = 5;

            var rndGen = new Random(1);
            var config = new TestAggregateFormationConfig()
            {
                MeanPPRadius = expectedMean,
                StdPPRadius = 1
            };

            var dist = new LogNormalSizeDistribution(rndGen, config);

            Assert.Equal(expectedMean, dist.Mean);
        }

        [Fact]
        private void LogNormalDistribution_Dist_Correct_Mean()
        {
            var expectedMean = 5;
            var expectedLogMean = 8.4;
            var expectedLogStd = 11.0;

            var nValues = 1000000;

            var rndGen = new Random(1);
            var config = new TestAggregateFormationConfig()
            {
                MeanPPRadius = expectedMean,
                StdPPRadius = 1
            };

            var dist = new LogNormalSizeDistribution(rndGen, config);

            var distValues = new List<double>(nValues);
            for (int i = 0; i < nValues; i++)
            {
                distValues.Add(dist.GetRandomSize());
            }

            var actualDistMean = distValues.Average();
            var actualDistStd = Math.Sqrt(distValues.Average(v => Math.Pow(v - actualDistMean, 2)));

            Assert.Equal(expectedLogMean, actualDistMean, 1);
            Assert.Equal(expectedLogStd, actualDistStd, 0);
        }
    }
}
