using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using ANPaX.Collection;
using ANPaX.Core.Neighborslist;
using ANPaX.Extensions;
using ANPaX.FilmFormation.interfaces;

namespace ANPaX.FilmFormation
{
    internal class BallisticAggregateDepositionHandler : IAggregateDepositionHandler
    {
        private readonly IFilmFormationConfig _filmFormationConfig;
        private readonly ISingleParticleDepositionHandler _singleParticleDepositionHandler;
        private readonly INeighborslistFactory _neighborslistFactory;

        public BallisticAggregateDepositionHandler(ISingleParticleDepositionHandler singleParticleDepositionHandler, IFilmFormationConfig filmFormationConfig)
        {
            _singleParticleDepositionHandler = singleParticleDepositionHandler;
            _filmFormationConfig = filmFormationConfig;
            _neighborslistFactory = new AccordNeighborslistFactory();
        }

        public void DepositAggregate(Aggregate aggregate, IEnumerable<PrimaryParticle> depositedPrimaryParticles, INeighborslist neighborslist)
        {
            var distanceTime = new Stopwatch();
            distanceTime.Start();
            var distances = new List<double>();
            var primaryParticles = aggregate.Cluster.SelectMany(c => c.PrimaryParticles);
            if (depositedPrimaryParticles.Any())
            {
                var maxRadius = depositedPrimaryParticles.GetMaxRadius();
                foreach (var primaryParticle in primaryParticles)
                {
                    distances.Add(_singleParticleDepositionHandler.GetMinDepositionDistance(primaryParticle, depositedPrimaryParticles, neighborslist));
                }
            }
            else
            {
                distances.Add(_filmFormationConfig.LargeNumber);
            }
            var distanceTimePassed = distanceTime.Elapsed;

            var depositGroundTime = new Stopwatch();
            depositGroundTime.Start();
            if (IsWithoutContact(distances))
            {
                DepositOnGround(aggregate);
                return;
            }
            var depositGroundTimePassed = depositGroundTime.Elapsed;

            var depositParticleTime = new Stopwatch();
            depositParticleTime.Start();
            DepositAtParticle(aggregate, distances);
            var depositParticleTimePassed = depositParticleTime.Elapsed;
        }

        /// <summary>
        /// If during deposition any other particle is hit, than the respective value
        /// in the list of deposition distances is smaller than IConfig.LargeNumber.
        /// In that case the deposition length equals the minimum of the deposition distances.
        /// </summary>
        /// <param name="aggregate">Aggregate to be deposited</param>
        /// <param name="distances">List of distances the individual primary particles can move</param>
        public void DepositAtParticle(Aggregate aggregate, List<double> distances)
        {
            aggregate.MoveBy(new Vector3(0, 0, -1 * distances.Min()));
        }

        /// <summary>
        /// Evaluate if the aggregate hits any other particle during ballistic deposition.
        /// 
        /// </summary>
        /// <param name="distances">List of the distances the individual primary particles can move
        /// before deposition.</param>
        /// <returns></returns>
        public bool IsWithoutContact(List<double> distances)
        {
            // If there is no contact, than all values in distance equal IConfig.LargeNumber.
            return (Math.Abs(distances.Min() - _filmFormationConfig.LargeNumber) < 1e-6);
        }

        /// <summary>
        /// If an aggregate is supposed to be deposited on the ground, get the minimum z position
        /// of the aggregate, which is the minimum of the lowest primary particle minus its radius.
        /// Then move the aggregate by this height.
        /// </summary>
        /// <param name="aggregate"></param>
        public void DepositOnGround(Aggregate aggregate)
        {
            var minZ = aggregate.Cluster.SelectMany(c => c.PrimaryParticles).Select(p => p.Position.Z - p.Radius).Min();

            aggregate.MoveBy(new Vector3(0, 0, -1 * minZ));
        }
    }
}
