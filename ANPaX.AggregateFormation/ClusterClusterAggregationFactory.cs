using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Accord.Collections;

using ANPaX.AggregateFormation.interfaces;
using ANPaX.Collection;
using ANPaX.Extensions;

using NLog;

namespace ANPaX.AggregateFormation
{
    internal class ClusterClusterAggregationFactory : IParticleFactory<Aggregate>
    {
        private readonly ISizeDistribution<double> _psd;
        private readonly IAggregateFormationConfig _config;
        private readonly ILogger _logger;
        private readonly int _seed;

        public ClusterClusterAggregationFactory(
            ISizeDistribution<double> primaryParticleSizeDistribution,
            IAggregateFormationConfig config,
            ILogger logger,
            int seed = -1)
        {
            _psd = primaryParticleSizeDistribution;
            _config = config;
            _logger = logger;
            _seed = seed;
        }

        public Aggregate Build(int size)
        {
            var aggregate = Process(size);
            while (aggregate is null)
            {
                Debug.WriteLine($"Restarting Aggregate: Exceeded FormationTime");
                aggregate = Process(size);
            }

            return aggregate;
        }

        /// <summary>
        /// The processing is decoupled from the building method to allow for restarting the building
        /// if it takes too long.
        /// This can happen mostly for polydisperse primary particle size distributions when the unsuitable sizes were
        /// selected for the fist few primary particles. 
        /// </summary>
        public Aggregate Process(
            int size)
        {
            var rndGen = new Random();
            if (_seed != -1)
            {
                rndGen = new Random(_seed);
            }
            var count = 0;
            var clusterToSet = new List<Cluster>();
            var depositedCluster = new List<Cluster>();
            var clusterSizes = GetClusterSizes(size, _config);
            var pca = new ParticleClusterAggregationFactory(_psd, rndGen, _config, _logger);
            foreach (var cSize in clusterSizes)
            {
                var cluster = pca.Build(cSize);
                clusterToSet.Add(cluster);
            }

            foreach (var cluster in clusterToSet)
            {
                if (!SetCluster(cluster, depositedCluster, count, rndGen, _config, _psd, _logger))
                {
                    return null;
                }
            }

            return new Aggregate(depositedCluster);
        }

        private static bool SetCluster(
            Cluster cluster,
            List<Cluster> depositedCluster,
            int count,
            Random rndGen,
            IAggregateFormationConfig config,
            ISizeDistribution<double> psd,
            ILogger logger)
        {
            if (!depositedCluster.Any())
            {
                depositedCluster.Add(cluster);
                return true;
            }
            else
            {
                return SetNextCluster(cluster, depositedCluster, count, rndGen, config, psd, logger);
            }
        }

        private static bool SetNextCluster(Cluster nextCluster,
                                           List<Cluster> depositedCluster,
                                           int count,
                                           Random rndGen,
                                           IAggregateFormationConfig config,
                                           ISizeDistribution<double> psd,
                                           ILogger logger)
        {
            var distance = GetDistanceFromCOMForNextCluster(nextCluster, depositedCluster, config, psd);
            var com = depositedCluster.GetCenterOfMass();
            foreach (var cluster in depositedCluster)
            {
                cluster.MoveBy(-1 * com);
            }
            var tree = depositedCluster.ToNeighborsList();
            var rndPosition = new Vector3();
            var found = false;
            while (!found)
            {
                rndPosition = ParticleFormationUtil.GetRandomPosition(rndGen, distance);
                found = TrySetCluster(nextCluster, rndPosition, tree, depositedCluster, config);
                if (count > config.MaxAttemptsPerAggregate)
                {
                    logger.Debug("Resetting aggregate generation. Time limit exceeded.");
                    return false;
                }
                count++;
            }

            depositedCluster.Add(nextCluster);
            return true;

        }

        public static bool TrySetCluster(
            Cluster nextCluster,
            Vector3 rndPosition,
            KDTree<double> tree,
            List<Cluster> depositedCluster,
            IAggregateFormationConfig config)
        {
            var isValid = false;
            nextCluster.MoveTo(rndPosition);
            foreach (var particle in nextCluster.PrimaryParticles)
            {
                var (anyNearby, allFeasible) = IsPrimaryParticleValid(tree, particle, depositedCluster, config);
                if (!allFeasible)
                {
                    return false;
                }
                // any two primary particles must be close enough to be in contact
                isValid = isValid || anyNearby;

            }

            return isValid;
        }

        public static (bool anyNearby, bool allFeasible) IsPrimaryParticleValid(
            KDTree<double> tree,
            PrimaryParticle particle,
            List<Cluster> depositedCluster,
            IAggregateFormationConfig config)
        {
            var neighbors = tree.Nearest
            (
                position: particle.Position.ToArray(),
                radius: (particle.Radius + depositedCluster.SelectMany(c => c.PrimaryParticles).Max(p => p.Radius))
                          * config.Delta
            );
            var anyNearby = false;
            var allFeasible = true;
            if (!neighbors.Any())
            {
                return (anyNearby, allFeasible);
            }
            foreach (var neigh in neighbors)
            {
                var (valid, feasible) = ParticleFormationUtil.IsValidPosition(neigh, depositedCluster.SelectMany(c => c.PrimaryParticles), particle.Radius, config);
                anyNearby = anyNearby || valid;
                allFeasible = allFeasible && feasible;
            }
            return (anyNearby, allFeasible);
        }

        public static List<int> GetClusterSizes(
            int targetAggregateSize,
            IAggregateFormationConfig config)
        {
            var nCluster = GetNumberOfCluster(targetAggregateSize, config);
            var clusterSizes = new List<int>();
            for (var i = 0; i < nCluster; i++)
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

        public static int GetNumberOfCluster(int targetAggregateSize, IAggregateFormationConfig config)
        {
            return Convert.ToInt32(Math.Ceiling(targetAggregateSize / Convert.ToDouble(config.ClusterSize)));
        }

        public static double GetDistanceFromCOMForNextCluster(
            Cluster nextCluster,
            List<Cluster> depositedCluster,
            IAggregateFormationConfig config,
            ISizeDistribution<double> psd)
        {
            var rgNext = nextCluster.GetGyrationRadius();
            var rgExist = depositedCluster.GetGyrationRadius();
            var nNext = nextCluster.NumberOfPrimaryParticles;
            var nExist = depositedCluster.Sum(c => c.NumberOfPrimaryParticles);

            return Math.Sqrt(
                (Math.Pow(psd.Mean, 2) * Math.Pow(nNext + nExist, 2) / (nExist * nNext))
                * Math.Pow((nExist + nNext) / config.Kf, 2 / config.Df)
                - ((nExist + nNext) / nExist) * Math.Pow(rgNext, 2)
                - ((nExist + nNext) / nNext) * Math.Pow(rgExist, 2));
        }
    }
}
