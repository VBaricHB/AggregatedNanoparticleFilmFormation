using System;
using System.IO;
using System.Linq;
using Xunit;

namespace ANPaX.AggregateFormation.tests
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
            var file = _resources +  "FSP_AggregateSizeDistribution.xml";
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
            var psd = new TabulatedPrimaryParticleSizeDistribution(dist, rndGen, integrate: true);
            var list = psd._tabulatedSizeDistribution;

            Assert.Equal(1.0, list.Sizes.Last().Probability);
        }
    }
}
