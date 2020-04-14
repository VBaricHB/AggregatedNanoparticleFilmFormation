using ANPaX.Collection;
using ANPaX.AggregateFormation.interfaces;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Diagnostics;
using NLog;
using ANPaX.Extensions;

namespace ANPaX.AggregateFormation
{
    internal class ParticleClusterAggregationFactory : IParticleFactory<Cluster>
    {
        private readonly ISizeDistribution<double> _psd;
        private readonly Random _rndGen;
        private readonly IAggregateFormationConfig _config;
        private readonly ILogger _logger;

        public ParticleClusterAggregationFactory(
            ISizeDistribution<double> psd,
            Random rndGen,
            IAggregateFormationConfig config,
            ILogger logger)
        {
            _psd = psd;
            _rndGen = rndGen;
            _config = config;
            _logger = logger;
        }

        public Cluster Build(int size)
        {
            var cluster = Procedure(size);
            while (cluster is null)
            {
                cluster = Procedure(size);
            }

            return cluster;
        }

        /// <summary>
        /// The processing is decoupled from the building method to allow for restarting the building
        /// if it takes too long.
        /// This can happen mostly for polydisperse primary particle size distributions when the unsuitable sizes were
        /// selected for the fist few primary particles. Since the distance from COM for the next primary particle only depends
        /// on the mean primary particle size, this distance can be infeasible for the cluster. In that case the whole cluster formation is restarted.
        /// </summary>
        public Cluster Procedure(int size)
        {
            var count = 0;
            var primaryParticles = new List<PrimaryParticle>();
            primaryParticles = SetFirstPrimaryParticle(primaryParticles);
            primaryParticles = SetSecondPrimaryParticle(primaryParticles);
            while (primaryParticles.Count < size)
            {
                if (!AddNextPrimaryParticle(primaryParticles, count))
                {
                    return null;
                }
            }

            return new Cluster(0, primaryParticles);
        }

        public List<PrimaryParticle> SetFirstPrimaryParticle(List<PrimaryParticle> primaryParticles)
        {
            var radius = _psd.GetRandomSize();
            primaryParticles.Add(new PrimaryParticle(0, new Vector3(0, 0, 0), radius));
            return primaryParticles;
        }

        public List<PrimaryParticle> SetSecondPrimaryParticle(List<PrimaryParticle> primaryParticles)
        {
            var radius = _psd.GetRandomSize();
            // Distance of the CenterOfMass (com) of the second pp from the com
            // of the first pp
            var particle = new PrimaryParticle(0, radius);
            var ppDistance = _config.Epsilon * (primaryParticles[0].Radius + particle.Radius);
            var rndPosition = ParticleFormationUtil.GetRandomPosition(_rndGen, ppDistance);
            particle.MoveTo(rndPosition);
            primaryParticles.Add(particle);
            return primaryParticles;
        }

        public bool AddNextPrimaryParticle(List<PrimaryParticle> primaryParticles, int count)
        {
            var com = primaryParticles.GetCenterOfMass();
            var ppDistance = GetNextPrimaryParticleDistance(primaryParticles);
            var particle = InitializeNewPrimaryParticle(_psd.GetRandomSize());

            var found = false;
            Vector3 rndPosition = new Vector3();
            while (!found)
            {
                rndPosition = ParticleFormationUtil.GetRandomPosition(_rndGen, ppDistance) + com;
                found = TrySetPrimaryParticle(particle, rndPosition, primaryParticles);
                if (count > _config.MaxAttemptsPerCluster)
                {
                    _logger.Debug("Resetting cluster generation. Time limit exceeded.");
                    return false;
                }
                count++;
            }
            particle.MoveTo(rndPosition);
            primaryParticles.Add(particle);
            return true;
        }

        public static PrimaryParticle InitializeNewPrimaryParticle(double radius)
        {
            return new PrimaryParticle(0, radius);
        }
        
        internal bool TrySetPrimaryParticle(PrimaryParticle particle, Vector3 rndPosition, List<PrimaryParticle> primaryParticles)
        {
            var tree = primaryParticles.ToNeighborsList();
            var searchRadius = (particle.Radius + primaryParticles.Max(p => p.Radius)) * _config.Delta;
            var neighbors = tree.Nearest(rndPosition.ToArray(),
                radius: searchRadius);

            bool anyNearby = false;
            bool allFeasible = true;
            if (!neighbors.Any())
            {
                return anyNearby && allFeasible;
            }
            foreach (var neigh in neighbors)
            {
                var (nearby, feasible) = ParticleFormationUtil.IsValidPosition(neigh, primaryParticles, particle.Radius, _config);
                anyNearby = anyNearby || nearby;
                allFeasible = allFeasible && feasible;
            }
            return anyNearby && allFeasible;
        }

        private double GetNextPrimaryParticleDistance(List<PrimaryParticle> primaryParticles)
        {
            var n = primaryParticles.Count + 1;
            var rsq = Math.Pow(n, 2) * Math.Pow(_psd.Mean, 2) / (n - 1)
                    * Math.Pow(n / _config.Kf, 2 / _config.Df)
                    - n * Math.Pow(_psd.Mean, 2) / (n - 1)
                    - n * Math.Pow(_psd.Mean, 2) * Math.Pow((n - 1) / _config.Kf, 2 / _config.Df);
            return Math.Sqrt(rsq);
        }
      
    }
}



