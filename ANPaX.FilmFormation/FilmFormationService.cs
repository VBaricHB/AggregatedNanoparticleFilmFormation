using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using ANPaX.AggregateFormation;
using ANPaX.Core;
using ANPaX.Core.Neighborslist;
using ANPaX.Core.Extensions;
using ANPaX.FilmFormation.interfaces;
using System.Threading;

namespace ANPaX.FilmFormation
{
    public class AggregateFilmFormationService
    {

        private readonly IAggregateDepositionHandler _aggregateDepositionHandler;
        private readonly IFilmFormationConfig _filmFormationConfig;
        private readonly IWallCollisionHandler _wallCollisionHandler;
        private readonly Random _rndGen;
        private ISimulationBox _simulationBox;
        private INeighborslistFactory _neighborslistFactory;

        public IParticleFilm<Aggregate> ParticleFilm { get; set; }

        public AggregateFilmFormationService(IFilmFormationConfig filmFormationConfig)
            : this(filmFormationConfig, -1)
        { }

        public AggregateFilmFormationService(
            IFilmFormationConfig filmFormationConfig, int seed)
        {
            _simulationBox = new AbsoluteTetragonalSimulationBox(filmFormationConfig.FilmWidthAbsolute);
            var primaryParticleDepositionHandler = new BallisticSingleParticleDepositionHandler(filmFormationConfig);
            _filmFormationConfig = filmFormationConfig;
            _aggregateDepositionHandler = new BallisticAggregateDepositionHandler(primaryParticleDepositionHandler, filmFormationConfig);
            _wallCollisionHandler = new WallCollisionHandler(_simulationBox);
            _neighborslistFactory = new AccordNeighborslistFactory();
            _rndGen = new Random();
            if (seed > 0)
            {
                _rndGen = new Random(seed);
            }

        }

        public async Task<IParticleFilm<Aggregate>> BuildFilm_Async(IEnumerable<Aggregate> aggregates, IProgress<ProgressReportModel> progress, CancellationToken ct)
        {
            ParticleFilm = new TetragonalAggregatedParticleFilm(_simulationBox, _neighborslistFactory);
            var report = new ProgressReportModel();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await Task.Run(() => ProcessAggregates(aggregates, progress, report, stopwatch, ct));

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

        private async Task ProcessAggregates(IEnumerable<Aggregate> aggregates, IProgress<ProgressReportModel> progress, ProgressReportModel report, Stopwatch stopwatch, CancellationToken ct)
        {
            var maxRadius = aggregates.GetPrimaryParticles().GetMaxRadius();
            foreach (var aggregate in aggregates)
            {
                await ProcessAggregate(aggregate, maxRadius, ct);
                UpdateProgress(progress, report, aggregates, stopwatch);
            }
        }

        private async Task ProcessAggregate(Aggregate aggregate, double maxRadius, CancellationToken ct)
        {
            InitializeAggregate(aggregate);
            await _aggregateDepositionHandler.DepositAggregate_Async(aggregate, ParticleFilm.PrimaryParticles, ParticleFilm.Neighborslist2D, maxRadius, _filmFormationConfig.MaxCPU, ct);
            ParticleFilm.AddDepositedParticlesToFilm(aggregate);

        }

        public void InitializeAggregate(Aggregate aggregate)
        {
            var rndPos = GetRandomInitialPosition();
            aggregate.MoveTo(rndPos);
            _wallCollisionHandler.CheckPrimaryParticle(aggregate.Cluster.SelectMany(c => c.PrimaryParticles));
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
