using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using ANPaX.Core;
using ANPaX.Core.Neighborslist;
using ANPaX.Core.Extensions;
using ANPaX.FilmFormation.interfaces;
using System.Threading.Tasks;
using System.Threading;

namespace ANPaX.FilmFormation
{
    internal class BallisticAggregateDepositionHandler : IAggregateDepositionHandler
    {
        private readonly IFilmFormationConfig _filmFormationConfig;
        private readonly ISingleParticleDepositionHandler _singleParticleDepositionHandler;

        public BallisticAggregateDepositionHandler(ISingleParticleDepositionHandler singleParticleDepositionHandler, IFilmFormationConfig filmFormationConfig)
        {
            _singleParticleDepositionHandler = singleParticleDepositionHandler;
            _filmFormationConfig = filmFormationConfig;
        }

        public async Task DepositAggregate_Async(Aggregate aggregate, IEnumerable<PrimaryParticle> depositedPrimaryParticles, INeighborslist neighborslist, double maxRadius, int maxCPU, CancellationToken ct)
        {
            var distancesTasks = new List<Task<double>>();
            var primaryParticles = aggregate.Cluster.SelectMany(c => c.PrimaryParticles);

            var distance = _filmFormationConfig.LargeNumber;
            var distances = new List<double>();
            var opt = new ParallelOptions
            {
                MaxDegreeOfParallelism = maxCPU,
                CancellationToken = ct
            };
            if (depositedPrimaryParticles.Any())
            {
                try
                {
                    await Task.Run(() =>
                        Parallel.ForEach<PrimaryParticle>(primaryParticles, opt, primaryParticle =>
                        {
                            distances.Add(_singleParticleDepositionHandler.GetMinDepositionDistance(primaryParticle, depositedPrimaryParticles, neighborslist, maxRadius));

                            opt.CancellationToken.ThrowIfCancellationRequested();
                        }
                    ));
                }
                catch(OperationCanceledException e)
                {
                    //_logger.Warn($"Operation canceled: {e.Message}");
                }

                //foreach (var primaryParticle in primaryParticles)
                //{
                //    distancesTasks.Add(Task.Run(() =>_singleParticleDepositionHandler.GetMinDepositionDistance(primaryParticle, depositedPrimaryParticles, neighborslist, maxRadius)));
                //}
                //var distances = await Task.WhenAll(distancesTasks);
                //distance = distances.Min();
            }
                      

            if (IsWithoutContact(distance))
            {
                DepositOnGround(aggregate);
                return;
            }

            distance = InterceptAtGround(aggregate, distance);

            DepositAtParticle(aggregate, distance);
        }

        /// <summary>
        /// If one primary particle of the aggregate hits another primary particle, it might
        /// be that other primary particles end up below zero height
        /// This is prevented here
        /// </summary>
        /// <param name="aggregate"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        private double InterceptAtGround(Aggregate aggregate, double distance)
        {
            var minZ = aggregate.Cluster.SelectMany(c => c.PrimaryParticles).Select(p => p.Position.Z - p.Radius).Min();
            return Math.Min(minZ, distance);
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
        public bool IsWithoutContact(double distance)
        {
            // If there is no contact, than all values in distance equal IConfig.LargeNumber.
            return (Math.Abs(distance - _filmFormationConfig.LargeNumber) < 1e-6);
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
