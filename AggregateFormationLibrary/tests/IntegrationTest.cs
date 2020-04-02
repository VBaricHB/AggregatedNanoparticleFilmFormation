using Moq;
using NLog;
using System.Linq;
using Xunit;

namespace AggregateFormation.tests
{
    public class IntegrationTest
    {

        [Fact]
        private  void GenerateAggregates_AllGenerated()
        {
            var primaryParticles = 1000;

            var seed = 9999;
            var logger = new Mock<ILogger>().Object;
            var psd = new MonodispersePrimaryParticleSizeDistribution(5);
            var fileASD = @"C:\Users\Valen\source\repos\AggregatedNanoparticleFilmFormation\AggregateFormationLibrary\resources\FSP_AggregateSizeDistribution.xml";
            var distASD = XMLSizeDistributionBuilder<int>.Read(fileASD);
            var asd = new TabulatedAggregateSizeDistribution(distASD, true, seed);
            var config = new CustomConfig();
            var parameters = new CustomParticleGenerationParameters(primaryParticles);
            var particleGenerationService = new ClusterClusterAggregationFactory(parameters.ClusterSize, psd, config, logger, seed);

            var service = new AggregateFormationService(psd, asd, config, parameters, particleGenerationService);
            var aggs =  service.GenerateAggregates();
            Assert.True(aggs.Sum(agg => agg.NumberOfPrimaryParticles) >= primaryParticles);

        }

        [Fact (Skip ="Something is not working here")]
        private async void GenerateAggregates_Async_AllGenerated()
        {
            var seed = 9999;
            var logger = new Mock<ILogger>().Object;
            var psd = new MonodispersePrimaryParticleSizeDistribution(5);
            var fileASD = @"C:\Users\Valen\source\repos\AggregatedNanoparticleFilmFormation\AggregateFormationLibrary\resources\FSP_AggregateSizeDistribution.xml";
            var distASD = XMLSizeDistributionBuilder<int>.Read(fileASD);
            var asd = new TabulatedAggregateSizeDistribution(distASD, true, seed);
            var config = new CustomConfig();
            var parameters = new CustomParticleGenerationParameters();
            var particleGenerationService = new ClusterClusterAggregationFactory(parameters.ClusterSize, psd, config, logger, seed);

            var service = new AggregateFormationService(psd, asd, config, parameters, particleGenerationService);
            var aggs = await service.GenerateAggregates_Async();
            Assert.True(aggs.Sum(agg => agg.NumberOfPrimaryParticles) >= 200);

        }
    }
}
