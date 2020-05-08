using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

using Moq;

using NLog;

using Xunit;

namespace ANPaX.AggregateFormation.tests
{
    public class IntegrationTest
    {
        private string _resources = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\Resources\\"));
        private static int _seed = 100;
        private Random _rndGen = new Random(_seed);
        private ILogger _logger = new Mock<ILogger>().Object;
        private CancellationTokenSource _cts = new Mock<CancellationTokenSource>().Object;

        [Fact]
        private void GenerateMonodisperseAggregates_Sync_AllGenerated()
        {
            var primaryParticles = 2000;

            var psd = new MonodispersePrimaryParticleSizeDistribution(5);
            var fileASD = _resources + "FSP_AggregateSizeDistribution.xml";

            var distASD = XMLSizeDistributionBuilder<int>.Read(fileASD);
            var config = new TestAggregateFormationConfig(primaryParticles);
            var asd = new TabulatedAggregateSizeDistribution(distASD, _rndGen, config, integrate: true);
            var cca = new ClusterClusterAggregationFactory(psd, config, _logger, _seed);
            var service = new AggregateFormationService(asd, psd, config, cca, _logger);
            var aggs = service.GenerateAggregates();
            Assert.True(aggs.Sum(agg => agg.NumberOfPrimaryParticles) >= primaryParticles);

        }

        [Fact]
        private void GeneratePolydisperseAggregates_Sync_AllGenerated()
        {
            var primaryParticles = 200;
            var config = new TestAggregateFormationConfig(primaryParticles);

            var filePSD = _resources + "FSP_PrimaryParticleSizeDistribution.xml";
            var dist = XMLSizeDistributionBuilder<double>.Read(filePSD);
            var psd = new TabulatedPrimaryParticleSizeDistribution(dist, _rndGen, config, integrate: true);

            var fileASD = _resources + "FSP_AggregateSizeDistribution.xml";
            var distASD = XMLSizeDistributionBuilder<int>.Read(fileASD);
            var asd = new TabulatedAggregateSizeDistribution(distASD, _rndGen, config, integrate: true);

            var cca = new ClusterClusterAggregationFactory(psd, config, _logger, _seed);
            var service = new AggregateFormationService(asd, psd, config, cca, _logger);
            var aggs = service.GenerateAggregates();
            Assert.True(aggs.Sum(agg => agg.NumberOfPrimaryParticles) >= primaryParticles);

        }

        [Fact]
        private async void GenerateMonodisperseAggregates_Async_AllGenerated()
        {
            var primaryParticles = 2000;
            var rndGen = new Random();
            var logger = new Mock<ILogger>().Object;
            var config = new TestAggregateFormationConfig(primaryParticles);

            var psd = new MonodispersePrimaryParticleSizeDistribution(5);
            var fileASD = _resources + "FSP_AggregateSizeDistribution.xml";
            var distASD = XMLSizeDistributionBuilder<int>.Read(fileASD);
            var asd = new TabulatedAggregateSizeDistribution(distASD, rndGen, config, integrate: true);

            var cca = new ClusterClusterAggregationFactory(psd, config, _logger, _seed);
            var service = new AggregateFormationService(asd, psd, config, cca, _logger);
            var aggs = await service.GenerateAggregates_Async();
            Assert.True(aggs.Sum(agg => agg.NumberOfPrimaryParticles) >= primaryParticles);

        }

        [Fact]
        private void GenerateMonodisperseAggregates_Sync_Parallel_AllGenerated()
        {
            var primaryParticles = 2000;
            var numCPU = 2;
            var rndGen = new Random();
            var logger = new Mock<ILogger>().Object;
            var config = new TestAggregateFormationConfig(primaryParticles);

            var psd = new MonodispersePrimaryParticleSizeDistribution(5);
            var fileASD = _resources + "FSP_AggregateSizeDistribution.xml";
            var distASD = XMLSizeDistributionBuilder<int>.Read(fileASD);
            var asd = new TabulatedAggregateSizeDistribution(distASD, rndGen, config, integrate: true);

            var cca = new ClusterClusterAggregationFactory(psd, config, _logger, _seed);
            var service = new AggregateFormationService(asd, psd, config, cca, _logger);
            var aggs = service.GenerateAggregates_Parallel(numCPU);
            Assert.True(aggs.Sum(agg => agg.NumberOfPrimaryParticles) >= primaryParticles);
        }

        [Fact]
        private async void GenerateMonodisperseAggregates_Async_Parallel_AllGenerated()
        {
            var primaryParticles = 2000;
            var numCPU = 4;
            var rndGen = new Random();
            var logger = new Mock<ILogger>().Object;

            var fileASD = _resources + "FSP_AggregateSizeDistribution.xml";
            var distASD = XMLSizeDistributionBuilder<int>.Read(fileASD);
            var config = new TestAggregateFormationConfig(primaryParticles);

            var asd = new TabulatedAggregateSizeDistribution(distASD, rndGen, config, integrate: true);
            var psd = new MonodispersePrimaryParticleSizeDistribution(5);

            var cca = new ClusterClusterAggregationFactory(psd, config, _logger, _seed);
            var service = new AggregateFormationService(asd, psd, config, cca, _logger);

            var progress = new Progress<ProgressReportModel>();
            progress.ProgressChanged += ReportProgress;
            var aggs = await service.GenerateAggregates_Parallel_Async(numCPU, progress, _cts.Token);

            Assert.True(aggs.Sum(agg => agg.NumberOfPrimaryParticles) >= primaryParticles);
        }

        [Fact]
        private async void GeneratePolydisperseAggregates_Async_Parallel_AllGenerated()
        {
            var primaryParticles = 500;
            var numCPU = 4;
            var rndGen = new Random();
            var logger = new Mock<ILogger>().Object;
            var config = new TestAggregateFormationConfig(primaryParticles);

            var filePSD = _resources + "FSP_PrimaryParticleSizeDistribution.xml";
            var dist = XMLSizeDistributionBuilder<double>.Read(filePSD);
            var psd = new TabulatedPrimaryParticleSizeDistribution(dist, _rndGen, config, integrate: true);

            var fileASD = _resources + "FSP_AggregateSizeDistribution.xml";
            var distASD = XMLSizeDistributionBuilder<int>.Read(fileASD);
            var asd = new TabulatedAggregateSizeDistribution(distASD, _rndGen, config, integrate: true);

            var cca = new ClusterClusterAggregationFactory(psd, config, _logger, _seed);
            var service = new AggregateFormationService(asd, psd, config, cca, _logger);

            var progress = new Progress<ProgressReportModel>();
            progress.ProgressChanged += ReportProgress;
            var aggs = await service.GenerateAggregates_Parallel_Async(numCPU, progress, _cts.Token);

            Assert.True(aggs.Sum(agg => agg.NumberOfPrimaryParticles) >= primaryParticles);
        }

        private void ReportProgress(object sender, ProgressReportModel e)
        {
            Debug.WriteLine($"{e.SimulationTime} ms: Finished aggregate {e.TotalAggregates} ({e.PercentageComplete} %) with {e.PrimaryParticlesLastAggregate} primary particles. Total {e.TotalPrimaryParticles} primary particles.");
        }

    }
}
