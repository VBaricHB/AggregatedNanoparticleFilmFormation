using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ANPaX.Core;
using ANPaX.Core.interfaces;
using ANPaX.Simulation.AggregateFormation.interfaces;

using NLog;

namespace ANPaX.Simulation.AggregateFormation
{
    public class AggregateFormationService
    {
        private readonly int _seed;
        private ISizeDistribution<int> _asd;
        private ISizeDistribution<double> _psd;
        private IParticleFactory<Aggregate> _particleFactory;
        private INeighborslistFactory _neighborslistFactory;
        private readonly IAggregateFormationConfig _config;
        private readonly ILogger _logger;


        public AggregateFormationService(IAggregateFormationConfig config, ILogger logger)
        {
            _config = config;
            _logger = logger;
            _neighborslistFactory = new AccordNeighborslistFactory();

            if (config.UseDefaultGenerationMethods)
            {
                BuildDefaultMethods();
            }
            else
            {
                BuildMethodsFromConfig();
            }
        }

        private void BuildMethodsFromConfig()
        {
            _asd = _config.AggregateSizeDistribution;
            _psd = _config.PrimaryParticleSizeDistribution;
            _particleFactory = _config.AggregateFormationFactory;
        }

        private void BuildDefaultMethods()
        {
            var rndGen = new Random();
            if (_config.RandomGeneratorSeed != -1)
            {
                rndGen = new Random(_config.RandomGeneratorSeed);
            }
            _asd = DefaultConfigurationBuilder.GetAggreateSizeDistribution(rndGen, _config);
            _psd = DefaultConfigurationBuilder.GetPrimaryParticleSizeDistribution(rndGen, _config);
            _particleFactory = new ClusterClusterAggregationFactory(_psd, _config, _logger, _neighborslistFactory, _config.RandomGeneratorSeed);
        }

        internal AggregateFormationService
            (
            ISizeDistribution<int> aggregateSizeDistribution,
            ISizeDistribution<double> primaryParticleSizeDistribution,
            IAggregateFormationConfig config,
            IParticleFactory<Aggregate> particleFactory,
            ILogger logger,
            int seed = -1
            )
        {
            _seed = seed;
            _asd = aggregateSizeDistribution;
            _psd = primaryParticleSizeDistribution;
            _config = config;
            _logger = logger;
            _particleFactory = particleFactory;
        }



        public List<Aggregate> GenerateAggregates()
        {
            var totalTime = new Stopwatch();
            var aggregateSizes = GenerateAggregateSizes();
            var aggregates = new List<Aggregate>();
            foreach (var size in aggregateSizes)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                aggregates.Add(_particleFactory.Build(size));
                Debug.WriteLine($"{DateTime.Now}: Built aggregate {aggregates.Count}/{aggregateSizes.Count} with {aggregates.Last().NumberOfPrimaryParticles} PP in {stopwatch.Elapsed}");
            }
            Debug.WriteLine($"Total ComputationTime {totalTime.Elapsed}");
            totalTime.Stop();

            AggregateIndexingHelper.SetAggregateIndices(aggregates);
            return aggregates;
        }

        public async Task<List<Aggregate>> GenerateAggregates_Async()
        {
            var aggregateSizes = GenerateAggregateSizes();
            var aggregateGenTasks = new List<Task<Aggregate>>();
            foreach (var size in aggregateSizes)
            {
                aggregateGenTasks.Add(Task.Run(() => _particleFactory.Build(size)));
            }

            var aggregates = await Task.WhenAll(aggregateGenTasks);

            AggregateIndexingHelper.SetAggregateIndices(aggregates);
            return aggregates.ToList();

        }

        /// <summary>
        /// This runs the generation in parallel but not asyncronosly.
        /// It first generates sizes for aggregates and subsequently generates them
        /// </summary>
        /// <param name="maxCPU"></param>
        /// <returns></returns>
        public List<Aggregate> GenerateAggregates_Parallel(int maxCPU)
        {
            var opt = new ParallelOptions
            {
                MaxDegreeOfParallelism = maxCPU
            };
            var aggregateSizes = GenerateAggregateSizes();
            var aggregates = new List<Aggregate>();

            Parallel.ForEach<int>(aggregateSizes, opt, size =>
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                aggregates.Add(_particleFactory.Build(size));
                Debug.WriteLine($"{DateTime.Now}: Built aggregate {aggregates.Count}/{aggregateSizes.Count} with {aggregates.Last().NumberOfPrimaryParticles} PP in {stopwatch.Elapsed}");

            });
            AggregateIndexingHelper.SetAggregateIndices(aggregates);
            return aggregates;
        }

        public async Task<List<Aggregate>> GenerateAggregates_Parallel_Async(int maxCPU, IProgress<ProgressReportModel> progress, CancellationToken ct)
        {
            var opt = new ParallelOptions
            {
                MaxDegreeOfParallelism = maxCPU,
                CancellationToken = ct
            };
            var aggregateSizes = GenerateAggregateSizes();
            var aggregates = new List<Aggregate>();
            var report = new ProgressReportModel();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                await Task.Run(() =>
                    Parallel.ForEach<int>(aggregateSizes, opt, size =>
                    {
                        aggregates.Add(_particleFactory.Build(size));
                        UpdateProgress(progress, aggregateSizes, aggregates, report, stopwatch);

                        // If the job is canceled, an error is thrown which is catched below
                        opt.CancellationToken.ThrowIfCancellationRequested();
                    }));

            }
            catch (OperationCanceledException e)
            {
                _logger.Warn($"Operation canceled: {e.Message}");
            }

            AggregateIndexingHelper.SetAggregateIndices(aggregates);
            return aggregates;
        }

        private static void UpdateProgress(IProgress<ProgressReportModel> progress, List<int> aggregateSizes, List<Aggregate> aggregates, ProgressReportModel report, Stopwatch stopwatch)
        {
            report.PercentageComplete = (aggregates.Count * 100) / aggregateSizes.Count;
            report.CumulatedAggregates = aggregates.Count;
            report.TotalAggregates = aggregateSizes.Count;
            report.CumulatedPrimaryParticles += aggregates.Last().NumberOfPrimaryParticles;
            report.PrimaryParticlesLastAggregate = aggregates.Last().NumberOfPrimaryParticles;
            report.SimulationTime = stopwatch.ElapsedMilliseconds;
            report.TotalPrimaryParticles = aggregateSizes.Sum();
            progress.Report(report);
        }

        private List<int> GenerateAggregateSizes()
        {
            var sizes = new List<int>();
            var sum = 0;
            while (sum < _config.TotalPrimaryParticles)
            {
                sizes.Add(_asd.GetRandomSize());
                sum = sizes.Sum();
            }

            return sizes;
        }
    }
}
