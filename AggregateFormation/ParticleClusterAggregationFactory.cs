﻿using Common;
using AggregateFormation.interfaces;
using System.Collections.Generic;
using System;
using System.Linq;
using Accord.Collections;
using Common.interfaces;


namespace AggregateFormation
{
    public class ParticleClusterAggregationFactory : IParticleFactory<Cluster>
    {
        private readonly IPrimaryParticleSizeDistribution _psd;
        private readonly Random _rndGen;
        private readonly IConfig _config;

        public int CurrentPrimaryParticleId { get; private set; }
        public int CurrentClusterId { get; private set; }

        public List<PrimaryParticle> PrimaryParticles { get; set; }
        private int TargetClusterSize { get; }


        public ParticleClusterAggregationFactory(IPrimaryParticleSizeDistribution primaryParticleSizeDistribution, IConfig config)
        {
            _psd = primaryParticleSizeDistribution;
            _rndGen = new Random();
            _config = config;
            CurrentClusterId = 1;
            CurrentPrimaryParticleId = 1;
            
        }

        public ParticleClusterAggregationFactory(IPrimaryParticleSizeDistribution primaryParticleSizeDistribution, IConfig config, int seed)
            : this(primaryParticleSizeDistribution, config)
        {
            _rndGen = new Random(seed);
        }

        public Cluster Build(int size)
        {
            PrimaryParticles = new List<PrimaryParticle>();
            SetFirstPrimaryParticle();
            SetSecondPrimaryParticle();
            while(PrimaryParticles.Count < size)
            {
                AddNextPrimaryParticle();
            }

            return new Cluster(CurrentClusterId++, PrimaryParticles);
        }

        private void SetFirstPrimaryParticle()
        {
            var radius = _psd.GetRadiusByProbability(_rndGen.NextDouble());
            PrimaryParticles.Add(new PrimaryParticle(CurrentPrimaryParticleId++, new Vector3(0, 0, 0), radius));
        }

        private void SetSecondPrimaryParticle()
        {
            var radius = _psd.GetRadiusByProbability(_rndGen.NextDouble());
            // Distance of the CenterOfMass (com) of the second pp from the com
            // of the first pp
            var particle = new PrimaryParticle(CurrentPrimaryParticleId++, radius);
            var ppDistance = _config.Epsilon * (PrimaryParticles[0].Radius + particle.Radius);
            var rndPosition = ParticleFormationService.GetRandomPosition(_rndGen, ppDistance);
            particle.MoveTo(rndPosition);
            PrimaryParticles.Add(particle);
        }

        internal void AddNextPrimaryParticle()
        {
            var com = ParticleFormationService.GetCenterOfMass(PrimaryParticles);
            var ppDistance = GetNextPrimaryParticleDistance();
            var radius = _psd.GetRadiusByProbability(_rndGen.NextDouble());
            var particle = new PrimaryParticle(CurrentPrimaryParticleId++, radius);
            var tree = ParticleFormationService.BuildNeighborsList(PrimaryParticles);

            var found = false;
            Vector3 rndPosition = new Vector3();
            while (!found)
            {
                rndPosition = ParticleFormationService.GetRandomPosition(_rndGen, ppDistance) + com;
                found = TrySetPrimaryParticle(particle, rndPosition, tree);
            }
            particle.MoveTo(rndPosition);
            PrimaryParticles.Add(particle);
        }

        internal bool  TrySetPrimaryParticle(PrimaryParticle particle, Vector3 rndPosition, KDTree<double> tree)
        {
            
            double[] query = rndPosition.ToArray();

            var neighbors = tree.Nearest(query,
                radius: (particle.Radius + PrimaryParticles.Max(p => p.Radius))
                        * _config.Delta);

            bool anyNearby = false;
            bool allFeasible = true;
            if (!neighbors.Any())
            {
                return anyNearby && allFeasible;
            }
            foreach (var neigh in neighbors)
            {
                var (nearby, feasible) = ParticleFormationService.IsValidPosition(neigh, PrimaryParticles, particle.Radius, _config);
                anyNearby = anyNearby || nearby;
                allFeasible = allFeasible && feasible;
            }
            return anyNearby && allFeasible;
        }

        private double GetNextPrimaryParticleDistance()
        {
            var n = PrimaryParticles.Count + 1;
            var rsq = Math.Pow(n, 2) * Math.Pow(_psd.MeanRadius, 2) / (n - 1)
                    * Math.Pow(n / _config.Kf, 2 / _config.Df)
                    - n * Math.Pow(_psd.MeanRadius, 2) / (n - 1)
                    - n * Math.Pow(_psd.MeanRadius, 2) * Math.Pow((n - 1) / _config.Kf, 2 / _config.Df);
            return Math.Sqrt(rsq);
        }
      
    }
}


