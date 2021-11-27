using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ANPaX.Core;
using ANPaX.Core.interfaces;
using ANPaX.Core.ParticleFilm.interfaces;
using ANPaX.Simulation.FilmFormation.interfaces;

namespace ANPaX.Simulation.FilmFormation
{
    public class FilmFormationService
    {
        private readonly IAggregateDepositionHandler _aggregateDepositionHandler;
        private readonly IFilmFormationConfig _filmFormationConfig;
        private readonly IWallCollisionHandler _wallCollisionHandler;
        private readonly Random _rndGen;
        private ISimulationBox _simulationBox;
        private INeighborslistFactory _neighborslistFactory;

        public IParticleFilm<Aggregate> ParticleFilm { get; set; }

        public FilmFormationService(IFilmFormationConfig filmFormationConfig)
            : this(filmFormationConfig, -1)
        { }


        public FilmFormationService(
            IFilmFormationConfig filmFormationConfig,
            int seed
            )
        {
            _filmFormationConfig = filmFormationConfig ?? throw new ArgumentException(nameof(filmFormationConfig));
            _simulationBox = filmFormationConfig.SimulationBoxFactory.Build(_filmFormationConfig);
            _aggregateDepositionHandler = filmFormationConfig.AggregateDepositionHandler;
            _wallCollisionHandler = filmFormationConfig.WallCollisionHandler;
            _neighborslistFactory = filmFormationConfig.NeighborslistFactory;

            _rndGen = new Random();
            if (seed > 0)
            {
                _rndGen = new Random(seed);
            }
        }

        public async Task<IParticleFilm<Aggregate>> BuildFilm_Async(IEnumerable<Aggregate> aggregates, IProgress<ProgressReportModel> progress, double delta, CancellationToken ct)
        {
            ParticleFilm = new TetragonalAggregatedParticleFilm(_simulationBox, _neighborslistFactory);
            var report = new ProgressReportModel();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await ProcessAggregates(aggregates, progress, report, stopwatch, delta, ct);

            return ParticleFilm;
        }

        private void UpdateProgress(IProgress<ProgressReportModel> progress, ProgressReportModel report, IEnumerable<Aggregate> aggregates, Stopwatch stopwatch)
        {
            report.PercentageComplete = (ParticleFilm.Particles.Count * 100) / aggregates.Count();
            report.CumulatedAggregates = ParticleFilm.Particles.Count;
            report.TotalAggregates = aggregates.Count();
            report.CumulatedPrimaryParticles += ParticleFilm.Particles.Last().NumberOfPrimaryParticles;
            report.PrimaryParticlesLastAggregate = ParticleFilm.Particles.Last().NumberOfPrimaryParticles;
            report.SimulationTime = stopwatch.ElapsedMilliseconds;
            report.TotalPrimaryParticles = aggregates.GetNumberOfPrimaryParticles();
            progress.Report(report);
        }

        private async Task ProcessAggregates(IEnumerable<Aggregate> aggregates, IProgress<ProgressReportModel> progress, ProgressReportModel report, Stopwatch stopwatch, double delta, CancellationToken ct)
        {
            var maxRadius = aggregates.GetPrimaryParticles().GetMaxRadius();
            foreach (var aggregate in aggregates)
            {
                await ProcessAggregate(aggregate, maxRadius, delta, ct);
                UpdateProgress(progress, report, aggregates, stopwatch);
            }

        }


        private async Task ProcessAggregate(Aggregate aggregate, double maxRadius, double delta, CancellationToken ct)
        {
            InitializeAggregate(aggregate);
            await _aggregateDepositionHandler.DepositAggregate_Async(aggregate, ParticleFilm.PrimaryParticles, ParticleFilm.Neighborslist2D, maxRadius, _filmFormationConfig.MaxCPU, delta, ct);
            ParticleFilm.AddDepositedParticlesToFilm(aggregate);

        }

        public void InitializeAggregate(Aggregate aggregate)
        {
            var rndPos = GetRandomInitialPosition();
            aggregate.MoveTo(rndPos);
            _wallCollisionHandler.CheckPrimaryParticle(aggregate.Cluster.SelectMany(c => c.PrimaryParticles), _simulationBox);
        }

        private Vector3 GetRandomInitialPosition()
        {
            var rndX = ParticleFilm.SimulationBox.XDim.Lower + _rndGen.NextDouble() * ParticleFilm.SimulationBox.XDim.Width;
            var rndY = ParticleFilm.SimulationBox.YDim.Lower + _rndGen.NextDouble() * ParticleFilm.SimulationBox.YDim.Width;
            var z = 1e6;
            return new Vector3(rndX, rndY, z);
        }
    }
}
