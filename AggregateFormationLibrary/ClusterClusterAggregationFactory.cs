using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Accord.Collections;
using AggregateFormation.interfaces;
using CommonLibrary;
using CommonLibrary.interfaces;
using NLog;
using ParticleExtensionMethodLibrary;

namespace AggregateFormation
{
    internal class ClusterClusterAggregationFactory : IParticleFactory<Aggregate>
    {
        private readonly ISizeDistribution<double> _primaryParticleSizeDistribution;
        private readonly IParticleFactory<Cluster> _clusterFactory;
        private readonly IConfig _config;
        private readonly ILogger _logger;

        private readonly Random _rndGen;
        private int _seed = -1;

        private int TargetClusterSize { get; }
        internal double MeanRadius => _primaryParticleSizeDistribution.Mean;

        public ClusterClusterAggregationFactory( int targetClusterSize,
            ISizeDistribution<double> primaryParticleSizeDistribution, 
            IConfig config, 
            ILogger logger)
        {
            TargetClusterSize = targetClusterSize;
            _primaryParticleSizeDistribution = primaryParticleSizeDistribution;
            _config = config;
            _rndGen = new Random();
            _logger = logger;
            _clusterFactory = new ParticleClusterAggregationFactory(_primaryParticleSizeDistribution, _config, logger, _seed);
        }

        public ClusterClusterAggregationFactory
            (
            int targetClusterSize,
            ISizeDistribution<double> primaryParticleSizeDistribution, 
            IConfig config,
            ILogger logger,
            int seed)
            : this(targetClusterSize, primaryParticleSizeDistribution, config, logger)
        {
            _rndGen = new Random(seed);
            _seed = seed;
        }

        public Aggregate Build(int size)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var clusterToSet = new List<Cluster>();
            var depositedCluster = new List<Cluster>();
            var clusterSizes = GetClusterSizes(size);           

            foreach (var cSize in clusterSizes)
            {
                clusterToSet.Add(_clusterFactory.Build(cSize));
            }
            foreach(var cluster in clusterToSet)
            {
                SetCluster(cluster, depositedCluster, stopwatch, size);
            }
  
            return new Aggregate(depositedCluster);
        }

        private void SetCluster(Cluster cluster, List<Cluster> depositedCluster, Stopwatch stopwatch, int size)
        {
            if (!depositedCluster.Any())
            {
                depositedCluster.Add(cluster);
            }
            else
            {
                SetNextCluster(cluster, depositedCluster, stopwatch, size);
            }
        }

        private void SetNextCluster(Cluster nextCluster, List<Cluster> depositedCluster, Stopwatch stopwatch, int size)
        {
            var distance = GetDistanceFromCOMForNextCluster(nextCluster, depositedCluster);
            var com = depositedCluster.GetCenterOfMass();
            foreach(var cluster in depositedCluster)
            {
                cluster.MoveBy(-1 * com);
            }
            var tree = depositedCluster.ToNeighborsList();
            Vector3 rndPosition = new Vector3();
            var found = false;
            while (!found)
            {
                rndPosition = ParticleFormationService.GetRandomPosition(_rndGen, distance);
                found = TrySetCluster(nextCluster, rndPosition, tree, depositedCluster);
                if (stopwatch.ElapsedMilliseconds > _config.MaxTimeMilliseconds)
                {
                    _logger.Debug("Resetting aggregate generation. Time limit exceeded.");
                    Build(size);
                }
            }

            depositedCluster.Add(nextCluster);
       
        }

        internal bool TrySetCluster(Cluster nextCluster, Vector3 rndPosition, KDTree<double> tree, List<Cluster> depositedCluster)
        {
            bool isValid = false;
            nextCluster.MoveTo(rndPosition);
            foreach (var particle in nextCluster.PrimaryParticles)
            {
                var (anyNearby, allFeasible) = IsPrimaryParticleValid(tree, particle, depositedCluster);
                if (!allFeasible)
                {
                    return false;
                }
                // any two primary particles must be close enough to be in contact
                isValid = isValid || anyNearby;
                
            }

            return isValid;
        }

        internal (bool anyNearby, bool allFeasible) IsPrimaryParticleValid(KDTree<double> tree, PrimaryParticle particle, List<Cluster> depositedCluster)
        {
            var neighbors = tree.Nearest
            (
                position: particle.Position.ToArray(),
                radius: (particle.Radius + depositedCluster.SelectMany(c => c.PrimaryParticles).Max(p => p.Radius))
                          * _config.Delta
            );
            bool anyNearby = false;
            bool allFeasible = true;
            if (!neighbors.Any())
            {
                return (anyNearby, allFeasible);
            }
            foreach (var neigh in neighbors)
            {
                var (valid, feasible) = ParticleFormationService.IsValidPosition(neigh, depositedCluster.SelectMany(c => c.PrimaryParticles), particle.Radius, _config);
                anyNearby = anyNearby || valid;
                allFeasible = allFeasible && feasible;
            }
            return (anyNearby, allFeasible);
        }

        internal List<int> GetClusterSizes(int targetAggregateSize)
        {
            var nCluster = GetNumberOfCluster(targetAggregateSize);
            var clusterSizes = new List<int>();
            for(var i = 0; i < nCluster; i++)
            {
                clusterSizes.Add(0);
            }
            var number = 0;
            while (number < targetAggregateSize)
            {
                var index = 0;
                while (index < nCluster)
                {
                    clusterSizes[index++] += 1;
                    number++;
                    if (number == targetAggregateSize)
                    {
                        break;
                    }
                }
            }

            return clusterSizes;
        }

        internal int GetNumberOfCluster(int targetAggregateSize)
        {
            return Convert.ToInt32(Math.Ceiling(targetAggregateSize / Convert.ToDouble(TargetClusterSize)));
        }

        internal double GetDistanceFromCOMForNextCluster(Cluster nextCluster, List<Cluster> depositedCluster)
        {
            var rgNext = nextCluster.GetGyrationRadius();
            var rgExist = depositedCluster.GetGyrationRadius();
            var nNext = nextCluster.NumberOfPrimaryParticles;
            var nExist = depositedCluster.Sum(c => c.NumberOfPrimaryParticles);

            return Math.Sqrt(
                (Math.Pow(MeanRadius, 2) * Math.Pow(nNext + nExist, 2) / (nExist * nNext))
                * Math.Pow((nExist + nNext) / _config.Kf, 2 / _config.Df)
                - ((nExist + nNext) / nExist) * Math.Pow(rgNext, 2)
                - ((nExist + nNext) / nNext) * Math.Pow(rgExist, 2));
        }
    }
}
