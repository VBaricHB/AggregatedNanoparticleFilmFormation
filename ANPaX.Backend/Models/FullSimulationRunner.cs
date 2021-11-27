using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using ANPaX.Core;
using ANPaX.Core.interfaces;
using ANPaX.Simulation.AggregateFormation;
using ANPaX.Simulation.AggregateFormation.interfaces;
using ANPaX.Simulation.FilmFormation;
using ANPaX.Simulation.FilmFormation.interfaces;

using NLog;

namespace ANPaX.Backend.Models
{
    public class FullSimulationRunner : ISimulationRunner
    {
        private readonly IAggregateFormationConfig _aggregateFormationConfig;
        private readonly IAggregateSizeDistributionFactory _aggregateSizeDistributionFactory;
        private readonly IPrimaryParticleSizeDistributionFactory _primaryParticleSizeDistributionFactory;
        private readonly IAggregateFormationFactory _aggregateFormationFactory;
        private readonly INeighborslistFactory _neighborslistFactory;
        private readonly AggregateFormationService _aggregateFormationService;
        private readonly ILogger _logger;
        private readonly IFilmFormationConfig _filmFormationConfig;
        private readonly IProgress<ProgressReportModel> _progress;
        private CancellationTokenSource _cts;
        private double _delta = 1.01;
        private FilmFormationService _filmFormationService;
        private IParticleFilm<Aggregate> _result;

        public FullSimulationRunner(
            IAggregateFormationConfig aggregateFormationConfig,
            SimulationMonitor simulationMonitor,
            IAggregateSizeDistributionFactory aggregateSizeDistributionFactory,
            IPrimaryParticleSizeDistributionFactory primaryParticleSizeDistributionFactory,
            IAggregateFormationFactory aggregateFormationFactory,
            INeighborslistFactory neighborslistFactory,
            IFilmFormationConfig filmFormationConfig,
            ILogger logger)
        {
            _filmFormationConfig = filmFormationConfig;
            _progress = new Progress<ProgressReportModel>();
            _filmFormationService = new FilmFormationService(filmFormationConfig);
            _cts = new CancellationTokenSource();
            _aggregateFormationConfig = aggregateFormationConfig ?? throw new ArgumentException(nameof(aggregateFormationConfig));
            _aggregateSizeDistributionFactory = aggregateSizeDistributionFactory;
            _primaryParticleSizeDistributionFactory = primaryParticleSizeDistributionFactory;
            _aggregateFormationFactory = aggregateFormationFactory;
            _neighborslistFactory = neighborslistFactory;
            _logger = logger ?? throw new ArgumentException(nameof(logger));
            _aggregateFormationService = new AggregateFormationService(_aggregateSizeDistributionFactory, _primaryParticleSizeDistributionFactory, _aggregateFormationFactory, _neighborslistFactory, _aggregateFormationConfig, logger);
        }

        public void Cancel()
        {
            _cts.Cancel();
        }

        public IParticleFilm<Aggregate> Get_Film()
        {
            return _result;
        }

        public IEnumerable<Aggregate> Get_Aggregates()
        {
            return _result.Particles;
        }

        public async Task Run_Async()
        {
            var aggregates = await _aggregateFormationService.GenerateAggregates_Async();
            await _filmFormationService.BuildFilm_Async(aggregates, _progress, _delta, _cts.Token);
        }


    }
}
