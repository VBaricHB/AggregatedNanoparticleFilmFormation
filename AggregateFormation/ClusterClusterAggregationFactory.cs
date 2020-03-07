using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Collections;
using AggregateFormation.interfaces;
using Common;
using Common.interfaces;

namespace AggregateFormation
{
    public class ClusterClusterAggregationFactory : IParticleFactory<Aggregate>
    {
        private readonly IPrimaryParticleSizeDistribution _primaryParticleSizeDistribution;
        private readonly IConfig _config;
        private ParticleClusterAggregationFactory _clusterFactory;
        private List<Cluster> ClusterToSet { get; set; }
        internal List<Cluster> Cluster { get; set; }
        private readonly Random _rndGen;
        private int _seed = -1;

        private int TargetClusterSize { get; }
        internal double MeanRadius => _primaryParticleSizeDistribution.MeanRadius;

        public ClusterClusterAggregationFactory( int targetClusterSize,
            IPrimaryParticleSizeDistribution primaryParticleSizeDistribution, 
            IConfig config)
        {
            TargetClusterSize = targetClusterSize;
            _primaryParticleSizeDistribution = primaryParticleSizeDistribution;
            _config = config;
            _rndGen = new Random();
            _clusterFactory = new ParticleClusterAggregationFactory(_primaryParticleSizeDistribution, _config);
        }

        public ClusterClusterAggregationFactory(int targetClusterSize,
            IPrimaryParticleSizeDistribution primaryParticleSizeDistribution, 
            IConfig config, int seed)
            : this(targetClusterSize, primaryParticleSizeDistribution, config)
        {
            _rndGen = new Random(seed);
            _seed = seed;
            _clusterFactory = new ParticleClusterAggregationFactory(_primaryParticleSizeDistribution, _config,_seed);
        }

        public Aggregate Build(int size)
        {
            ClusterToSet = new List<Cluster>();
            Cluster = new List<Cluster>();
            var clusterSizes = GetClusterSizes(size);           

            foreach (var cSize in clusterSizes)
            {
                ClusterToSet.Add(_clusterFactory.Build(cSize));
            }
            foreach(var cluster in ClusterToSet)
            {
                SetCluster(cluster);
            }
  
            return new Aggregate(Cluster);
        }

        private void SetCluster(Cluster cluster)
        {
            if (!Cluster.Any())
            {
               Cluster.Add(cluster);
            }
            else
            {
                SetNextCluster(cluster);
            }
        }

        private void SetNextCluster(Cluster nextCluster)
        {
            var distance = GetDistanceFromCOMForNextCluster(nextCluster);
            var com = ParticleFormationService.GetCenterOfMass(Cluster.SelectMany(c => c.PrimaryParticles));
            foreach(var cluster in Cluster)
            {
                cluster.MoveBy(-1 * com);
            }
            var tree = ParticleFormationService.BuildNeighborsList(Cluster.SelectMany(c => c.PrimaryParticles));
            Vector3 rndPosition = new Vector3();
            var found = false;
            while (!found)
            {
                rndPosition = ParticleFormationService.GetRandomPosition(_rndGen, distance);
                found = TrySetCluster(nextCluster, rndPosition, tree);
            }
            
            Cluster.Add(nextCluster);
       
        }

        internal bool TrySetCluster(Cluster nextCluster, Vector3 rndPosition, KDTree<double> tree)
        {
            bool isValid = false;
            nextCluster.MoveTo(rndPosition);
            foreach (var particle in nextCluster.PrimaryParticles)
            {
                var (anyNearby, allFeasible) = IsPrimaryParticleValid(tree, particle);
                if (!allFeasible)
                {
                    return false;
                }
                // any two primary particles must be close enough to be in contact
                isValid = isValid || anyNearby;
                
            }

            return isValid;
        }

        internal (bool anyNearby, bool allFeasible) IsPrimaryParticleValid(KDTree<double> tree, PrimaryParticle particle)
        {
            var neighbors = tree.Nearest
                (
                position: particle.Position.ToArray(),
                radius: (particle.Radius + Cluster.SelectMany(c => c.PrimaryParticles).Max(p => p.Radius))
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
                var (valid, feasible) = ParticleFormationService.IsValidPosition(neigh, Cluster.SelectMany(c => c.PrimaryParticles), particle.Radius, _config);
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

        internal double GetDistanceFromCOMForNextCluster(Cluster nextCluster)
        {
            var rgNext = ParticleFormationService.GetRadiusOfGyration(nextCluster);
            var rgExist = ParticleFormationService.GetRadiusOfGyration(Cluster);
            var nNext = nextCluster.NumberOfPrimaryParticles;
            var nExist = Cluster.Sum(c => c.NumberOfPrimaryParticles);

            return Math.Sqrt(
                (Math.Pow(MeanRadius, 2) * Math.Pow(nNext + nExist, 2) / (nExist * nNext))
                * Math.Pow((nExist + nNext) / _config.Kf, 2 / _config.Df)
                - ((nExist + nNext) / nExist) * Math.Pow(rgNext, 2)
                - ((nExist + nNext) / nNext) * Math.Pow(rgExist, 2));
        }
    }
}
