using CommonLibrary;
using AggregateFormation.interfaces;
using System.Collections.Generic;
using System;
using System.Linq;
using Accord.Collections;
using CommonLibrary.interfaces;
using System.Diagnostics;
using NLog;
using ParticleExtensionMethodLibrary;

namespace AggregateFormation
{
    internal class ParticleClusterAggregationFactory : IParticleFactory<Cluster>
    {
        private readonly ISizeDistribution<double> _psd;
        private readonly Random _rndGen;
        private readonly IConfig _config;
        private readonly ILogger _logger;

        public int CurrentPrimaryParticleId { get; private set; }
        public int CurrentClusterId { get; private set; }
       
        public ParticleClusterAggregationFactory(ISizeDistribution<double> primaryParticleSizeDistribution, IConfig config, ILogger logger)
        {
            _psd = primaryParticleSizeDistribution;
            _rndGen = new Random();
            _config = config;
            _logger = logger;
            CurrentClusterId = 1;
            CurrentPrimaryParticleId = 1;
        }

        public ParticleClusterAggregationFactory(ISizeDistribution<double> primaryParticleSizeDistribution, IConfig config,ILogger logger,  int seed)
            : this(primaryParticleSizeDistribution, config, logger)
        {
            _rndGen = new Random(seed);
        }

        public Cluster Build(int size)
        {

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var primaryParticles = new List<PrimaryParticle>();
            SetFirstPrimaryParticle(primaryParticles);
            SetSecondPrimaryParticle(primaryParticles);
            while(primaryParticles.Count < size)
            {
                AddNextPrimaryParticle(primaryParticles, stopwatch, size);
            }

            return new Cluster(CurrentClusterId++, primaryParticles);
        }

        private void SetFirstPrimaryParticle(List<PrimaryParticle> primaryParticles)
        {
            var radius = _psd.GetRandomSize();
            primaryParticles.Add(new PrimaryParticle(CurrentPrimaryParticleId++, new Vector3(0, 0, 0), radius));
        }

        private void SetSecondPrimaryParticle(List<PrimaryParticle> primaryParticles)
        {
            var radius = _psd.GetRandomSize();
            // Distance of the CenterOfMass (com) of the second pp from the com
            // of the first pp
            var particle = new PrimaryParticle(CurrentPrimaryParticleId++, radius);
            var ppDistance = _config.Epsilon * (primaryParticles[0].Radius + particle.Radius);
            var rndPosition = ParticleFormationService.GetRandomPosition(_rndGen, ppDistance);
            particle.MoveTo(rndPosition);
            primaryParticles.Add(particle);
        }

        internal void AddNextPrimaryParticle(List<PrimaryParticle> primaryParticles, Stopwatch stopwatch, int size)
        {
            var com = primaryParticles.GetCenterOfMass();
            var ppDistance = GetNextPrimaryParticleDistance(primaryParticles);
            var radius = _psd.GetRandomSize();
            var particle = new PrimaryParticle(CurrentPrimaryParticleId++, radius);
            var tree = primaryParticles.ToNeighborsList();

            var found = false;
            Vector3 rndPosition = new Vector3();
            while (!found)
            {
                rndPosition = ParticleFormationService.GetRandomPosition(_rndGen, ppDistance) + com;
                found = TrySetPrimaryParticle(particle, rndPosition, tree, primaryParticles);
                if (stopwatch.ElapsedMilliseconds > _config.MaxTimeMilliseconds)
                {
                    _logger.Debug("Resetting cluster generation. Time limit exceeded.");
                    Build(size);
                }
            }
            particle.MoveTo(rndPosition);
            primaryParticles.Add(particle);
        }

        internal bool TrySetPrimaryParticle(PrimaryParticle particle, Vector3 rndPosition, KDTree<double> tree, List<PrimaryParticle> primaryParticles)
        {
            
            double[] query = rndPosition.ToArray();

            var neighbors = tree.Nearest(query,
                radius: (particle.Radius + primaryParticles.Max(p => p.Radius))
                        * _config.Delta);

            bool anyNearby = false;
            bool allFeasible = true;
            if (!neighbors.Any())
            {
                return anyNearby && allFeasible;
            }
            foreach (var neigh in neighbors)
            {
                var (nearby, feasible) = ParticleFormationService.IsValidPosition(neigh, primaryParticles, particle.Radius, _config);
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



