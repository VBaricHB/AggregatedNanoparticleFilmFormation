using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using ANPaX.Core;
using ANPaX.Core.interfaces;
using ANPaX.Simulation.AggregateFormation;
using ANPaX.Simulation.AggregateFormation.interfaces;

using NLog;

namespace ANPaX.Backend.Models
{
    public class AggregateFormationSimulationRunner : ISimulationRunner
    {
        private readonly IAggregateFormationConfig _aggregateFormationConfig;
        private readonly AggregateFormationService _aggregateFormationService;
        public IProgress<ProgressReportModel> Progress { get; }
        private CancellationTokenSource _cts;
        private readonly ILogger _logger;

        private IEnumerable<Aggregate> _aggregates;

        public int Id { get; set; }

        public AggregateFormationSimulationRunner(
            IAggregateSizeDistributionFactory aggregateSizeDistributionFactory,
            IPrimaryParticleSizeDistributionFactory primaryParticleSizeDistributionFactory,
            IAggregateFormationFactory aggregateFormationFactory,
            INeighborslistFactory neighborslistFactory,
            IAggregateFormationConfig aggregateFormationConfig,
            ILogger logger)
        {
            _aggregateFormationConfig = aggregateFormationConfig ?? throw new ArgumentException(nameof(aggregateFormationConfig));
            _logger = logger ?? throw new ArgumentException(nameof(logger));
            _aggregateFormationService = new AggregateFormationService(aggregateSizeDistributionFactory, primaryParticleSizeDistributionFactory, aggregateFormationFactory, neighborslistFactory, _aggregateFormationConfig, logger);
            Progress = new Progress<ProgressReportModel>();
            _cts = new CancellationTokenSource();
        }

        public void Cancel()
        {
            _cts.Cancel();
        }

        public IEnumerable<Aggregate> Get_Aggregates()
        {
            return _aggregates;
        }

        public IParticleFilm<Aggregate> Get_Film()
        {
            return null;
        }

        public async Task Run_Async()
        {
            _aggregates = await _aggregateFormationService.GenerateAggregates_Parallel_Async(4, Progress, _cts.Token);
        }

    }
}
