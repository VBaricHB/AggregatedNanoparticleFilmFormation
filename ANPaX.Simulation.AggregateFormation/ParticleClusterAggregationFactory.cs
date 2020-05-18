using System;
using System.Collections.Generic;
using System.Linq;

using ANPaX.Core;
using ANPaX.Core.interfaces;
using ANPaX.Simulation.AggregateFormation.interfaces;

using NLog;

namespace ANPaX.Simulation.AggregateFormation
{
    /// <summary>
    /// This class is an implementation of the Particle-Cluster-Aggregation (PCA) proposed in
    /// Filippov et al. (2000), Journal of Colloid and Interface Science 229, 261-273.
    /// In that paper this algorithm is also called Sequential Algorithm (SA)
    /// </summary>
    /// 
    /// <remarks>
    /// The paper derives an equation <see cref="GetNextPrimaryParticleDistance"/> determining
    /// the distance of the center of mass from the cumulated primary particles and the next primary particle.
    /// This class formes a sphere with this radius around the cumulated primary particle and
    /// searches for a suitable position for the next primary particle on this sphere.
    ///
    /// This class is utilized in the Cluster-Cluster-Aggregation (CCA) in <see cref="ClusterClusterAggregationFactory"/>.
    /// </remarks>
    internal class ParticleClusterAggregationFactory : IParticleFactory<Cluster>
    {
        private readonly ISizeDistribution<double> _psd;
        private readonly Random _rndGen;
        private readonly IAggregateFormationConfig _config;
        private readonly ILogger _logger;
        private readonly INeighborslistFactory _neighborslistFactory;

        public ParticleClusterAggregationFactory(
            ISizeDistribution<double> psd,
            Random rndGen,
            IAggregateFormationConfig config,
            INeighborslistFactory neighborslistFactory,
            ILogger logger)
        {
            _psd = psd;
            _rndGen = rndGen;
            _config = config;
            _logger = logger;
            _neighborslistFactory = neighborslistFactory;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
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

            SetFirstPrimaryParticle(primaryParticles);
            SetSecondPrimaryParticle(primaryParticles);

            // the neighborslist speeds up the search
            var neighborslist = _neighborslistFactory.Build3DNeighborslist(primaryParticles);
            while (primaryParticles.Count < size)
            {
                if (!AddNextPrimaryParticle(primaryParticles, count, neighborslist))
                {
                    return null;
                }
            }

            return new Cluster(0, primaryParticles);
        }

        /// <summary>
        /// The first primary particle of a cluster is always at (0,0,0)
        /// </summary>
        /// <param name="primaryParticles"></param>
        /// <returns></returns>
        public void SetFirstPrimaryParticle(List<PrimaryParticle> primaryParticles)
        {
            var radius = _psd.GetRandomSize();
            primaryParticles.Add(new PrimaryParticle(0, new Vector3(0, 0, 0), radius));
        }

        /// <summary>
        /// The second primary particle position is determined by its and the radius of the initial primary particle
        /// </summary>
        /// <param name="primaryParticles"></param>
        public void SetSecondPrimaryParticle(List<PrimaryParticle> primaryParticles)
        {
            var radius = _psd.GetRandomSize();
            // Distance of the CenterOfMass (com) of the second pp from the com
            // of the first pp
            var particle = new PrimaryParticle(0, radius);

            // Position the second primary particle directly in contact with the first
            var ppDistance = _config.Epsilon * (primaryParticles[0].Radius + particle.Radius);

            // Get any random position withon that distance
            var rndPosition = ParticleFormationUtil.GetRandomPosition(_rndGen, ppDistance);

            particle.MoveTo(rndPosition);
            primaryParticles.Add(particle);
        }

        /// <summary>
        /// The position of the third (and follwing) primary particle is determined by the PCA:
        /// The radius determined by the geometrical properties is computed and the
        /// primary particle is positioned in that distance to the existing ones.
        /// If that primary particle is in contact with another one but not
        /// overlapping with any other, it may reside there.
        /// </summary>
        /// <param name="primaryParticles"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool AddNextPrimaryParticle(List<PrimaryParticle> primaryParticles, int count, INeighborslist neighborslist)
        {
            var com = primaryParticles.GetCenterOfMass();

            // compute the distance for the next primary particle to fulfill fractal dimension
            var ppDistance = GetNextPrimaryParticleDistance(primaryParticles);

            // get a new primary particle without any position yet.
            var particle = InitializeNewPrimaryParticle(_psd.GetRandomSize());

            var found = false;
            var rndPosition = new Vector3();

            while (!found)
            {
                // get a new random position on the sphere of allowed positions
                rndPosition = ParticleFormationUtil.GetRandomPosition(_rndGen, ppDistance) + com;

                // check if that position is valid
                found = IsPrimaryParticlePositionValid(particle, rndPosition, neighborslist, primaryParticles, _config);
                if (count > _config.MaxAttemptsPerCluster)
                {
                    _logger.Debug("Resetting cluster generation. Time limit exceeded.");
                    return false;
                }
                count++;
            }
            particle.MoveTo(rndPosition);
            primaryParticles.Add(particle);
            neighborslist.AddParticlesToNeighborsList(particle);
            return true;
        }

        /// <summary>
        /// A primary particle positon is valid if:
        /// (1) there is at least 1 contact with other primary particles
        /// (2) there are no overlaps.
        /// </summary>
        /// <param name="particle"></param>
        /// <param name="rndPosition"></param>
        /// <param name="tree"></param>
        /// <param name="primaryParticles"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool IsPrimaryParticlePositionValid(
            PrimaryParticle particle,
            Vector3 rndPosition,
            INeighborslist neighborslist,
            IEnumerable<PrimaryParticle> primaryParticles,
            IAggregateFormationConfig config)
        {
            // Get all neighbors within potential reach of the primary particle
            var searchRadius = (particle.Radius + primaryParticles.GetMaxRadius()) * config.Delta;
            var neighborsWithDistance = neighborslist.GetPrimaryParticlesAndDistanceWithinRadius(rndPosition, searchRadius);

            // no neighbor: invalid position
            if (!neighborsWithDistance.Any())
            {
                return false;
            }
            return IsAnyNeighborPositionValid(particle, rndPosition, config, neighborsWithDistance);
        }

        /// <summary>
        /// Since there is a nearby primary particle, check if the position is valid:
        /// (1) there is at least 1 contact with other primary particles
        /// (2) there are no overlaps.
        /// </summary>
        /// <param name="particle"></param>
        /// <param name="primaryParticles"></param>
        /// <param name="config"></param>
        /// <param name="neighbors"></param>
        /// <returns></returns>
        private static bool IsAnyNeighborPositionValid(
            PrimaryParticle particle,
            Vector3 setToPosition,
            IAggregateFormationConfig config,
            IEnumerable<Tuple<PrimaryParticle, double>> neighborsWithDistance)
        {
            var (isInContact, hasNoOverlap) = ParticleFormationUtil.IsAnyNeighborInContactOrOverlapping(particle, setToPosition, config, neighborsWithDistance);
            return isInContact && hasNoOverlap;
        }

        public static PrimaryParticle InitializeNewPrimaryParticle(double radius)
        {
            return new PrimaryParticle(0, radius);
        }

        /// <summary>
        /// The equation for PCA from Filippov et al. (2000), Journal of Colloid and Interface Science 229, 261-273.
        /// The position of the next primary particle is determined by the fractal dimension Df and the fractal prefactor Kf.
        /// Also, the mean primary particle radius is considered here.
        /// </summary>
        /// <param name="primaryParticles"></param>
        /// <returns></returns>
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



