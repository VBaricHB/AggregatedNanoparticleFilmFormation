using System.Linq;
using Xunit;

namespace AggregateFormation.tests
{
    public class SizeDistributionTest
    {

        [Fact]
        private void XMLPrimaryParticleSizeDistributionBuilder_FileReadCorrectly()
        {
            var file = @"C:\Users\Valen\source\repos\AggregatedNanoparticleFilmFormation\AggregateFormationLibrary\resources\FSP_PrimaryParticleSizeDistribution.xml";
            var dist = XMLSizeDistributionBuilder<double>.Read(file);
            Assert.Equal(21, dist.Sizes.Length);
            Assert.Equal(1.5, dist.Sizes.First().Value);
            Assert.Equal(11.5, dist.Sizes.Last().Value);
        }

        [Fact]
        private void XMLAggregateSizeDistributionBuilder_FileReadCorrectly()
        {
            var file = @"C:\Users\Valen\source\repos\AggregatedNanoparticleFilmFormation\AggregateFormationLibrary\resources\FSP_AggregateSizeDistribution.xml";
            var dist = XMLSizeDistributionBuilder<int>.Read(file);
            Assert.Equal(116, dist.Sizes.Length);
            Assert.Equal(13, dist.Sizes.First().Value);
            Assert.Equal(128, dist.Sizes.Last().Value);
        }

        [Fact]
        private void TabulatedPrimaryParticleSizeDistributionFromXML_IntegrationCorrectly()
        {
            var file = @"C:\Users\Valen\source\repos\AggregatedNanoparticleFilmFormation\AggregateFormationLibrary\resources\FSP_PrimaryParticleSizeDistribution.xml";
            var dist = XMLSizeDistributionBuilder<double>.Read(file);
            var psd = new TabulatedPrimaryParticleSizeDistribution(dist, integrate: true);
            var list = psd._tabulatedSizeDistribution;

            Assert.Equal(1.0, list.Sizes.Last().Probability);
        }
    }
}
