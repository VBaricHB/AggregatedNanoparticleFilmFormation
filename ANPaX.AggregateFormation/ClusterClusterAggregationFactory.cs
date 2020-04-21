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
    /// <summary>
    /// This class is an implementation of the Cluster-Cluster-Aggregation (CCA) proposed in
    /// Filippov et al. (2000), Journal of Colloid and Interface Science 229, 261-273.
    /// </summary>
    /// 
    /// <remarks>
    /// It assumes that aggregates are formed from clusters of defined size.
    /// The paper derives an equation <see cref="GetDistanceFromCOMForNextCluster"/> determining
    /// the distance of the center of mass from the cumulated clusters and the next cluster.
    /// This class formes a sphere with this radius around the cumulated clusters and
    /// searches for a suitable position for the next cluster on this sphere.
    ///
    /// This class uses the Particle-Cluster-Aggregation (PCA) <see cref="ParticleClusterAggregationFactory"/>
    /// from the same paper for the formation of the clusters.
    /// </remarks>
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

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="numberOfPrimaryParticles">Number of primary particles in this aggregate</param>
        /// <returns></returns>
        public Aggregate Build(int numberOfPrimaryParticles)
        {
            var aggregate = Process(numberOfPrimaryParticles);
            // If no suitable position is found within the time limit,
            // null is returned and the search is restarted.
            while (aggregate is null)
            {
                Debug.WriteLine($"Restarting Aggregate: Exceeded FormationTime");
                aggregate = Process(numberOfPrimaryParticles);
            }

            return aggregate;
        }

        /// <summary>
        /// The processing is decoupled from the building method to allow for restarting the building
        /// if it takes too long.
        /// This can happen mostly for polydisperse primary particle size distributions when the unsuitable sizes were
        /// selected for the fist few primary particles. 
        /// </summary>
        public Aggregate Process(int numberOfPrimaryParticles)
        {
            // initialize a new random generator.
            // For some reason the calculation often fails if
            // the random generator is injected into this class
            var rndGen = new Random();

            // -1 is the seed that denotes current time dependent random generation
            // without defined seed (to disable randomness)
            if (_seed != -1)
            {
                rndGen = new Random(_seed);
            }

            var clusterToSet = GenerateCluster(numberOfPrimaryParticles, rndGen);

            var depositedCluster = CombineCluster(clusterToSet, rndGen);
            if (depositedCluster is null)
            {
                return null;
            }

            return new Aggregate(depositedCluster);
        }

        /// <summary>
        /// Form the individual (non-connected) cluster based on the Particle-Cluster-Aggregation (PCA).
        /// </summary>
        /// <param name="rndGen">Pseudo-Random Number Generator</param>
        /// <param name="numberOfPrimaryParticles"> Total size (number of primary particles) of the final aggregate.</param>
        /// <returns>List of individual (non-connected) cluster.</returns>
        internal IEnumerable<Cluster> GenerateCluster(int numberOfPrimaryParticles, Random rndGen)
        {
            // instantiate the ParticleClusterAggregationFactory to generate clusters
            var pca = new ParticleClusterAggregationFactory(_psd, rndGen, _config, _logger);

            // determine the size of the clusters dependent on the total number of primary particles
            // and the given cluster size
            var clusterSizes = GetClusterSizes(numberOfPrimaryParticles, _config);

            // form the individual cluster
            var clusterToSet = clusterSizes.Select(s => pca.Build(s));

            return clusterToSet;
        }

        /// <summary>
        /// Use the equation of the CCA to subsequently combine the clusters to an aggregate.
        /// </summary>
        /// <param name="clusterToSet">Individual non-connected cluster</param>
        /// <param name="rndGen">Pseudo-Random Number Generator</param>
        /// <returns>List of individual (but connected) cluster.</returns>
        internal IEnumerable<Cluster> CombineCluster(IEnumerable<Cluster> clusterToSet, Random rndGen)
        {
            var count = 0;

            var depositedCluster = new List<Cluster>();

            foreach (var cluster in clusterToSet)
            {
                if (!PositionCluster(cluster, depositedCluster, count, rndGen, _config, _psd, _logger))
                {
                    // When the algorithm fails to position the cluster return null to
                    // restart the algorithm
                    return null;
                }
            }

            return depositedCluster;
        }

        /// <summary>
        /// Position the next cluster depending on the already positioned cluster.
        /// If it is the first cluster, just set it where it is.
        /// </summary>
        /// <param name="cluster">The next cluster to position.</param>
        /// <param name="depositedCluster"></param>
        /// <param name="count">Current tries to determine if the algorithm fails to form the aggregate using the current primary particle sizes.</param>
        /// <param name="rndGen">Pseudo-Random Number Generator</param>
        /// <param name="config">Parameters that determine the generation of the aggregate</param>
        /// <param name="primaryParticleSizeDistribution">The primary particle size distribution providing the mean radius for the calculation of the position of the next cluster.</param>
        /// <param name="logger"></param>
        /// <returns></returns>
        private static bool PositionCluster(
            Cluster cluster,
            List<Cluster> depositedCluster,
            int count,
            Random rndGen,
            IAggregateFormationConfig config,
            ISizeDistribution<double> primaryParticleSizeDistribution,
            ILogger logger)
        {
            if (!depositedCluster.Any())
            {
                // It is the first cluster so just leave where it is.
                depositedCluster.Add(cluster);
                return true;
            }
            else
            {
                // Try to position the next cluster based on CCA. If it fails return false to invoke the restart of the formation procedure.
                return PositionNextCluster(cluster, depositedCluster, count, rndGen, config, primaryParticleSizeDistribution, logger);
            }
        }

        /// <summary>
        /// Position the next cluster depending on the already positioned cluster.
        /// </summary>
        private static bool PositionNextCluster(Cluster nextCluster,
                                           List<Cluster> depositedCluster,
                                           int count,
                                           Random rndGen,
                                           IAggregateFormationConfig config,
                                           ISizeDistribution<double> psd,
                                           ILogger logger)
        {
            // Compute the distance of the center of mass (COM) of next cluster to the
            // COM of the already depositioned cluster based on the equation of the CCA
            var distance = GetDistanceFromCOMForNextCluster(nextCluster, depositedCluster, config, psd);
            MoveClusterToCOM(depositedCluster);

            // build the neigborlist to speed up the search for the correct position
            var tree = depositedCluster.ToNeighborsList();
            var isValid = false;
            while (!isValid)
            {
                isValid = IsRandomPositionValid(nextCluster, depositedCluster, rndGen, config, distance, tree);

                // With the current configuration (primary particle sizes) no suitable position of the cluster is found
                // therefore, a restart is invoked.
                if (count > config.MaxAttemptsPerAggregate)
                {
                    logger.Debug("Resetting aggregate generation. Time limit exceeded.");
                    return false;
                }
                count++;
            }

            // suitable position, add cluster to the existing cluster.
            depositedCluster.Add(nextCluster);
            return true;

        }

        private static bool IsRandomPositionValid(
            Cluster nextCluster,
            List<Cluster> depositedCluster,
            Random rndGen,
            IAggregateFormationConfig config,
            double distance,
            KDTree<double> tree)
        {
            bool found;
            // Get a new random position for the COM on the sphere around the deposited cluster
            var rndPosition = ParticleFormationUtil.GetRandomPosition(rndGen, distance);

            // Move the cluster to that position on the sphere
            nextCluster.MoveTo(rndPosition);

            // Check if that random position is valid (no overlap, but connection between nextCluster an the deposited cluster)
            found = IsClusterPositionValid(nextCluster, tree, depositedCluster, config);
            return found;
        }

        private static void MoveClusterToCOM(List<Cluster> depositedCluster)
        {
            // move the deposted cluster so that the common COM is at (0,0,0) 
            var com = depositedCluster.GetCenterOfMass();
            foreach (var cluster in depositedCluster)
            {
                cluster.MoveBy(-1 * com);
            }
        }

        /// <summary>
        /// Check if the current position is valid:
        /// (1) no overlapping primary particles,
        /// (2) at least two primary (from nextCluster and depositedCluster) are in contact.
        /// </summary>
        /// <param name="nextCluster"></param>
        /// <param name="tree"></param>
        /// <param name="depositedCluster"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static bool IsClusterPositionValid(
            Cluster nextCluster,
            KDTree<double> tree,
            List<Cluster> depositedCluster,
            IAggregateFormationConfig config)
        {

            var isValid = false;

            foreach (var particle in nextCluster.PrimaryParticles)
            {
                var (anyNearby, allFeasible) = IsPrimaryParticlePositionValid(tree, particle, depositedCluster.GetPrimaryParticles(), config);
                if (!allFeasible)
                {
                    return false;
                }
                // any two primary particles must be close enough to be in contact
                isValid = isValid || anyNearby;
            }

            return isValid;
        }

        /// <summary>
        /// (1) Check for the specific primary particles if the position is valid: no overlap
        /// (2) Check if any other primary particle is in contact
        /// </summary>
        /// <param name="tree">neighborListTree</param>
        /// <param name="primaryParticle">primary particle of interest</param>
        /// <param name="otherPrimaryParticles">all other fixed primary particles</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static (bool isInContact, bool hasNoOverlap) IsPrimaryParticlePositionValid(
            KDTree<double> tree,
            PrimaryParticle primaryParticle,
            IEnumerable<PrimaryParticle> otherPrimaryParticles,
            IAggregateFormationConfig config)
        {
            var neighbors = ParticleFormationUtil.GetPossibleNeighbors(primaryParticle, primaryParticle.Position, tree, otherPrimaryParticles, config);

            if (!neighbors.Any())
            {
                return (isInContact: false, hasNoOverlap: true);
            }

            return ParticleFormationUtil.IsAnyNeighborInContactOrOverlapping(primaryParticle, otherPrimaryParticles, config, neighbors);
        }

        /// <summary>
        /// Distribute the total aggregate size to (preferably) homogeneous cluster.
        /// The number of cluster is determined by the cluster size defined in the config.
        /// </summary>
        /// <param name="targetAggregateSize"></param>
        /// <param name="config"></param>
        /// <returns></returns>
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

        /// <summary>
        /// The number of cluster is determined by the aggregate size and the cluster size
        /// </summary>
        /// <param name="targetAggregateSize"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static int GetNumberOfCluster(int targetAggregateSize, IAggregateFormationConfig config)
        {
            return Convert.ToInt32(Math.Ceiling(targetAggregateSize / Convert.ToDouble(config.ClusterSize)));
        }

        /// <summary>
        /// The CCA equation from Filippov et al. (2000), Journal of Colloid and Interface Science 229, 261-273.
        /// </summary>
        /// <param name="nextCluster"></param>
        /// <param name="depositedCluster"></param>
        /// <param name="config"></param>
        /// <param name="psd"></param>
        /// <returns></returns>
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
