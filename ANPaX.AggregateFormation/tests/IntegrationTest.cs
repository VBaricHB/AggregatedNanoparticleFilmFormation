using Moq;
using NLog;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace ANPaX.AggregateFormation.tests
{
    public class IntegrationTest
    {
        private string _resources = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\Resources\\"));
        private int _seed = 100;

        [Fact]
        private  void GenerateAggregates_AllGenerated()
        {
            var primaryParticles = 200;

            var rndGen = new Random(_seed);
            var logger = new Mock<ILogger>().Object;
            var psd = new MonodispersePrimaryParticleSizeDistribution(5);
            var fileASD = _resources + "FSP_AggregateSizeDistribution.xml";

            var distASD = XMLSizeDistributionBuilder<int>.Read(fileASD);
            var asd = new TabulatedAggregateSizeDistribution(distASD, rndGen, true);
            var config = new CustomConfig();
            var parameters = new CustomParticleGenerationParameters(primaryParticles);
            var particleGenerationService = new ClusterClusterAggregationFactory(parameters.ClusterSize, psd, config, logger, rndGen);

            var service = new AggregateFormationService(psd, asd, config, parameters, particleGenerationService);
            var aggs =  service.GenerateAggregates();
            Assert.True(aggs.Sum(agg => agg.NumberOfPrimaryParticles) >= primaryParticles);

        }

        [Fact]
        private async void GenerateAggregates_Async_AllGenerated()
        {
            var primaryParticles = 200;
            var rndGen = new Random(_seed);
            var logger = new Mock<ILogger>().Object;
            var psd = new MonodispersePrimaryParticleSizeDistribution(5);
            var fileASD = _resources + "FSP_AggregateSizeDistribution.xml";
            var distASD = XMLSizeDistributionBuilder<int>.Read(fileASD);
            var asd = new TabulatedAggregateSizeDistribution(distASD, rndGen, true);
            var config = new CustomConfig();
            var parameters = new CustomParticleGenerationParameters(primaryParticles);
            var particleGenerationService = new ClusterClusterAggregationFactory(parameters.ClusterSize, psd, config, logger, rndGen);

            var service = new AggregateFormationService(psd, asd, config, parameters, particleGenerationService);
            var aggs = await service.GenerateAggregates_Async();
            Assert.True(aggs.Sum(agg => agg.NumberOfPrimaryParticles) >= primaryParticles);

        }
    }
}
