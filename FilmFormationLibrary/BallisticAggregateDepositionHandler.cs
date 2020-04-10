using CommonLibrary;
using CommonLibrary.interfaces;
using FilmFormationLibrary.interfaces;
using ParticleExtensionMethodLibrary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FilmFormationLibrary
{
    internal class BallisticAggregateDepositionHandler : IAggregateDepositionHandler
    {
        private readonly IConfig _config;
        private readonly ISingleParticleDepositionHandler _singleParticleDepositionHandler;

        public BallisticAggregateDepositionHandler(ISingleParticleDepositionHandler singleParticleDepositionHandler, IConfig config)
        {
            _singleParticleDepositionHandler = singleParticleDepositionHandler;
            _config = config;
        }

        public void DepositAggregate(Aggregate aggregate, IEnumerable<PrimaryParticle> depositedPrimaryParticles)
        {
            var distances = new List<double>();
            foreach (var primaryParticle in aggregate.Cluster.SelectMany(c => c.PrimaryParticles))
            {
                distances.Add(_singleParticleDepositionHandler.GetMinDepositionDistance(primaryParticle, depositedPrimaryParticles));
            }

            if (IsWithoutContact(distances))
            {
                DepositOnGround(aggregate);
                return;
            }

            DepositAtParticle(aggregate, distances);
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
            return (Math.Abs(distances.Min() - _config.LargeNumber) < 1e-6);
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
