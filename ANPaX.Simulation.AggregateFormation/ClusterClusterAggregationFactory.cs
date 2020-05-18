using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using ANPaX.Core;
using ANPaX.Core.interfaces;
using ANPaX.Simulation.AggregateFormation.interfaces;

using NLog;

namespace ANPaX.Simulation.AggregateFormation
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
        private readonly INeighborslistFactory _neighborslistFactory;

        public ClusterClusterAggregationFactory(
            ISizeDistribution<double> primaryParticleSizeDistribution,
            IAggregateFormationConfig config,
            ILogger logger,
            INeighborslistFactory neighborslistFactory,
            int seed = -1)
        {
            _psd = primaryParticleSizeDistribution;
            _config = config;
            _logger = logger;
            _seed = seed;
            _neighborslistFactory = neighborslistFactory;
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
            var pca = new ParticleClusterAggregationFactory(_psd, rndGen, _config, _neighborslistFactory, _logger);

            // determine the size of the clusters dependent on the total number of primary particles
            // and the given cluster size
            var clusterSizes = GetClusterSizes(numberOfPrimaryParticles);

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
                if (!PositionCluster(cluster, depositedCluster, count, rndGen))
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
        private bool PositionCluster(
            Cluster cluster,
            List<Cluster> depositedCluster,
            int count,
            Random rndGen)
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
                return PositionNextCluster(cluster, depositedCluster, count, rndGen);
            }
        }

        /// <summary>
        /// Position the next cluster depending on the already positioned cluster.
        /// </summary>
        private bool PositionNextCluster(Cluster nextCluster,
                                           List<Cluster> depositedCluster,
                                           int count,
                                           Random rndGen)
        {
            // Compute the distance of the center of mass (COM) of next cluster to the
            // COM of the already depositioned cluster based on the equation of the CCA
            var distance = GetDistanceFromCOMForNextCluster(nextCluster, depositedCluster);
            MoveClusterToCOM(depositedCluster);

            // build the neigborlist to speed up the search for the correct position
            var neighborslist = _neighborslistFactory.Build3DNeighborslist(depositedCluster);
            var isValid = false;
            while (!isValid)
            {
                isValid = IsRandomPositionValid(nextCluster, depositedCluster, rndGen, distance, neighborslist);

                // With the current configuration (primary particle sizes) no suitable position of the cluster is found
                // therefore, a restart is invoked.
                if (count > _config.MaxAttemptsPerAggregate)
                {
                    _logger.Debug("Resetting aggregate generation. Time limit exceeded.");
                    return false;
                }
                count++;
            }

            // suitable position, add cluster to the existing cluster.
            depositedCluster.Add(nextCluster);
            return true;

        }

        private bool IsRandomPositionValid(
            Cluster nextCluster,
            List<Cluster> depositedCluster,
            Random rndGen,
            double distance,
            INeighborslist neighborslist)
        {
            bool found;
            // Get a new random position for the COM on the sphere around the deposited cluster
            var rndPosition = ParticleFormationUtil.GetRandomPosition(rndGen, distance);

            // Move the cluster to that position on the sphere
            nextCluster.MoveTo(rndPosition);

            // Check if that random position is valid (no overlap, but connection between nextCluster an the deposited cluster)
            found = IsClusterPositionValid(nextCluster, neighborslist, depositedCluster);
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
        public bool IsClusterPositionValid(
            Cluster nextCluster,
            INeighborslist neighborslist,
            List<Cluster> depositedCluster)
        {

            var isValid = false;

            foreach (var particle in nextCluster.PrimaryParticles)
            {
                var (anyNearby, allFeasible) = IsPrimaryParticlePositionValid(neighborslist, particle, depositedCluster.GetPrimaryParticles());
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
        public (bool isInContact, bool hasNoOverlap) IsPrimaryParticlePositionValid(
            INeighborslist neighborslist,
            PrimaryParticle primaryParticle,
            IEnumerable<PrimaryParticle> otherPrimaryParticles)
        {
            //var neighbors = ParticleFormationUtil.GetPossibleNeighbors(primaryParticle, primaryParticle.Position, tree, otherPrimaryParticles, config);
            var searchRadius = (primaryParticle.Radius + otherPrimaryParticles.GetMaxRadius()) * _config.Delta;
            var neighborsWithDistance = neighborslist.GetPrimaryParticlesAndDistanceWithinRadius(primaryParticle.Position, searchRadius);

            if (!neighborsWithDistance.Any())
            {
                return (isInContact: false, hasNoOverlap: true);
            }

            return ParticleFormationUtil.IsAnyNeighborInContactOrOverlapping(primaryParticle, primaryParticle.Position, _config, neighborsWithDistance);
        }

        /// <summary>
        /// Distribute the total aggregate size to (preferably) homogeneous cluster.
        /// The number of cluster is determined by the cluster size defined in the config.
        /// </summary>
        /// <param name="targetAggregateSize"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public List<int> GetClusterSizes(int targetAggregateSize)
        {
            var nCluster = GetNumberOfCluster(targetAggregateSize);
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
        public int GetNumberOfCluster(int targetAggregateSize)
        {
            return Convert.ToInt32(Math.Ceiling(targetAggregateSize / Convert.ToDouble(_config.ClusterSize)));
        }

        /// <summary>
        /// The CCA equation from Filippov et al. (2000), Journal of Colloid and Interface Science 229, 261-273.
        /// </summary>
        /// <param name="nextCluster"></param>
        /// <param name="depositedCluster"></param>
        /// <param name="config"></param>
        /// <param name="psd"></param>
        /// <returns></returns>
        public double GetDistanceFromCOMForNextCluster(
            Cluster nextCluster,
            List<Cluster> depositedCluster)
        {
            var rgNext = nextCluster.GetGyrationRadius();
            var rgExist = depositedCluster.GetGyrationRadius();
            var nNext = nextCluster.NumberOfPrimaryParticles;
            var nExist = depositedCluster.Sum(c => c.NumberOfPrimaryParticles);

            return Math.Sqrt(
                (Math.Pow(_psd.Mean, 2) * Math.Pow(nNext + nExist, 2) / (nExist * nNext))
                * Math.Pow((nExist + nNext) / _config.Kf, 2 / _config.Df)
                - ((nExist + nNext) / nExist) * Math.Pow(rgNext, 2)
                - ((nExist + nNext) / nNext) * Math.Pow(rgExist, 2));
        }
    }
}
