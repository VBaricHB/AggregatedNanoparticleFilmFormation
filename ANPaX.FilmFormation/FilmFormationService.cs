using System;
using System.Collections.Generic;
using System.Linq;

using ANPaX.Collection;
using ANPaX.Extensions;
using ANPaX.FilmFormation.interfaces;

namespace ANPaX.FilmFormation
{
    public class AggregateFilmFormationService
    {
        private readonly ISimulationDomain<Aggregate> _simulationDomain;
        private readonly IAggregateDepositionHandler _aggregateDepositionHandler;
        private readonly IList<Aggregate> _aggregates;
        private readonly IWallCollisionHandler _wallCollisionHandler;
        private readonly Random _rndGen;

        public AggregateFilmFormationService
            (
            IFilmFormationConfig config,
            IList<Aggregate> aggregates,
            Random rndGen,
            IFilmFormationConfig filmFormationConfig
            )
        {
            var maxRadius = aggregates.GetMaxRadius();
            var simulationBox = new RectangularSimulationBox(filmFormationConfig.FilmWidthAbsolute);
            var primaryParticleDepositionHandler = new BallisticSingleParticleDepositionHandler(config, maxRadius * config.Delta);

            _simulationDomain = new RectangularAggregateSimulationDomain(simulationBox, maxRadius);
            _aggregateDepositionHandler = new BallisticAggregateDepositionHandler(primaryParticleDepositionHandler, config);
            _wallCollisionHandler = new WallCollisionHandler(simulationBox);
            _aggregates = aggregates;
            _rndGen = rndGen;
        }

        public void BuildFilm()
        {
            foreach (var aggregate in _aggregates)
            {
                InitializeAggregate(aggregate);
                _aggregateDepositionHandler.DepositAggregate(aggregate, _simulationDomain.PrimaryParticles);
                _simulationDomain.AddDepositedParticlesToDomain(aggregate);
            }
        }

        public void InitializeAggregate(Aggregate aggregate)
        {
            var rndPos = GetRandomPosition();
            aggregate.MoveTo(rndPos);
            _wallCollisionHandler.CheckPrimaryParticle(aggregate.Cluster.SelectMany(c => c.PrimaryParticles));
        }

        private Vector3 GetRandomPosition()
        {
            var rndX = _simulationDomain.SimulationBox.XDim.Lower + _rndGen.NextDouble() * _simulationDomain.SimulationBox.XDim.Width;
            var rndY = _simulationDomain.SimulationBox.YDim.Lower + _rndGen.NextDouble() * _simulationDomain.SimulationBox.YDim.Width;
            var z = 1e6;
            return new Vector3(rndX, rndY, z);
        }
    }
}
