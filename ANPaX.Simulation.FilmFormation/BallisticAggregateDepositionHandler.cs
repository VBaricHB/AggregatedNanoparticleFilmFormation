using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ANPaX.Core;
using ANPaX.Core.interfaces;
using ANPaX.Simulation.FilmFormation.interfaces;

namespace ANPaX.Simulation.FilmFormation
{
    public class BallisticAggregateDepositionHandler : IAggregateDepositionHandler
    {
        private readonly ISingleParticleDepositionHandler _singleParticleDepositionHandler;

        public BallisticAggregateDepositionHandler(ISingleParticleDepositionHandler singleParticleDepositionHandler)
        {
            _singleParticleDepositionHandler = singleParticleDepositionHandler;
        }

        public async Task DepositAggregate_Async(Aggregate aggregate, IEnumerable<PrimaryParticle> depositedPrimaryParticles, INeighborslist neighborslist, double maxRadius, int maxCPU, double delta, CancellationToken ct)
        {
            var distancesTasks = new List<Task<double>>();
            var primaryParticles = aggregate.Cluster.SelectMany(c => c.PrimaryParticles);

            var distance = aggregate.Cluster.SelectMany(c => c.PrimaryParticles).Select(p => p.Position.Z - p.Radius).Min();
            var tasks = new List<Task<double>>();

            if (depositedPrimaryParticles.Any())
            {

                foreach (var primaryParticle in primaryParticles)
                {
                    tasks.Add(Task.Run(() => _singleParticleDepositionHandler.GetDepositionDistance(primaryParticle, depositedPrimaryParticles, neighborslist, maxRadius, delta)));
                }

                var distances = await Task.WhenAll(tasks);
                //var distances = new List<double>();
                //foreach (var primaryParticle in primaryParticles)
                //{
                //    distances.Add(_singleParticleDepositionHandler.GetDepositionDistance(primaryParticle, depositedPrimaryParticles, neighborslist, maxRadius));
                //}

                distance = distances.Min();
            }

            aggregate.MoveBy(new Vector3(0, 0, -1 * distance));

        }

        public void DepositAggregate(Aggregate aggregate, IEnumerable<PrimaryParticle> depositedPrimaryParticles, INeighborslist neighborslist, double maxRadius, double delta)
        {
            var distancesTasks = new List<Task<double>>();
            var primaryParticles = aggregate.Cluster.SelectMany(c => c.PrimaryParticles);

            var distance = aggregate.Cluster.SelectMany(c => c.PrimaryParticles).Select(p => p.Position.Z - p.Radius).Min();
            var tasks = new List<Task<double>>();

            if (depositedPrimaryParticles.Any())
            {
                var distances = new List<double>();
                foreach (var primaryParticle in primaryParticles)
                {
                    distances.Add(_singleParticleDepositionHandler.GetDepositionDistance(primaryParticle, depositedPrimaryParticles, neighborslist, maxRadius, delta));
                }

                distance = distances.Min();
            }

            aggregate.MoveBy(new Vector3(0, 0, -1 * distance));

        }



        /// <summary>
        /// If during deposition any other particle is hit, than the respective value
        /// in the list of deposition distances is smaller than IConfig.LargeNumber.
        /// In that case the deposition length equals the minimum of the deposition distances.
        /// </summary>
        /// <param name="aggregate">Aggregate to be deposited</param>
        /// <param name="distances">List of distances the individual primary particles can move</param>
        public void DepositAtParticle(Aggregate aggregate, double distance)
        {
            aggregate.MoveBy(new Vector3(0, 0, -1 * distance));
        }

        /// <summary>
        /// Evaluate if the aggregate hits any other particle during ballistic deposition.
        /// 
        /// </summary>
        /// <param name="distances">List of the distances the individual primary particles can move
        /// before deposition.</param>
        /// <returns></returns>
        public bool IsWithoutContact(double distance, double largeNumber)
        {
            // If there is no contact, than all values in distance equal IConfig.LargeNumber.
            return (Math.Abs(distance - largeNumber) < 1e-6);
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
