using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Collections;
using AggregateFormation.interfaces;
using Common;
using Common.interfaces;

namespace AggregateFormation
{
    public class ClusterClusterAggregation : IParticleFactory<Aggregate>
    {
        private readonly IPrimaryParticleSizeDistribution _primaryParticleSizeDistribution;
        private readonly IConfig _config;
        private List<Cluster> _clustersToSet;
        private readonly Random _rndGen;
        private int _seed = -1;

        private int TargetClusterSize { get; }

        private List<Cluster> Cluster { get; set; }

        public ClusterClusterAggregation( int targetClusterSize,
            IPrimaryParticleSizeDistribution primaryParticleSizeDistribution, IConfig config)
        {
            TargetClusterSize = targetClusterSize;
            _primaryParticleSizeDistribution = primaryParticleSizeDistribution;
            _config = config;
            Cluster = new List<Cluster>();
            _clustersToSet = new List<Cluster>();
            _rndGen = new Random();
        }

        public ClusterClusterAggregation(int targetClusterSize,
            IPrimaryParticleSizeDistribution primaryParticleSizeDistribution, IConfig config, int seed)
            : this(targetClusterSize, primaryParticleSizeDistribution, config)
        {
            _rndGen = new Random(seed);
            _seed = seed;
        }

        public Aggregate Build(int size)
        {
            var clusterSizes = GetClusterSizes(size);
            var tmpClusters = new List<Cluster>();
            ParticleClusterAggregation clusterFactory;
            if (_seed == -1)
            {
                clusterFactory = new ParticleClusterAggregation(_primaryParticleSizeDistribution, _config);
            }
            else
            {
                clusterFactory = new ParticleClusterAggregation(_primaryParticleSizeDistribution, _config, _seed);
            }

            foreach (var cSize in clusterSizes)
            {
                _clustersToSet.Add(clusterFactory.Build(cSize));
            }
            foreach(var cluster in _clustersToSet)
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
            var com = Utility.GetCenterOfMass(Cluster.SelectMany(c => c.PrimaryParticles));
            var distance = GetDistanceForNextCluster(nextCluster);
            var found = false;
            var tree = Utility.BuildNeighborsList(Cluster.SelectMany(c => c.PrimaryParticles));
            Vector3 rndPosition = new Vector3();
            while (!found)
            {
                rndPosition = Utility.GetRandomPosition(_rndGen, distance) + com;
                found = TrySetCluster(nextCluster, rndPosition, tree);
            }
            nextCluster.MoveTo(rndPosition);
            Cluster.Add(nextCluster);
       
        }

        private bool TrySetCluster(Cluster nextCluster, Vector3 rndPosition, KDTree<double> tree)
        {
            double[] query = rndPosition.ToArray();
            bool globalIsValid = true;

            foreach (var particle in nextCluster.PrimaryParticles)
            {
                var valid = IsPrimaryParticleValid(tree, particle);
                globalIsValid = globalIsValid && valid;
            }

            return globalIsValid;
        }

        internal bool IsPrimaryParticleValid(KDTree<double> tree, PrimaryParticle particle)
        {
            var neighbors = tree.Nearest(particle.Position.ToArray(),
                  radius: (particle.Radius + Cluster.SelectMany(c=> c.PrimaryParticles).Max(p => p.Radius))
                          * _config.Delta);
            bool isValid;
            if (neighbors.Count() > 0)
            {
                isValid = true;
            }
            else
            {
                isValid = false;
            }
            foreach (var neigh in neighbors)
            {
                var valid = Utility.IsValidPosition(neigh, Cluster.SelectMany(c => c.PrimaryParticles), particle.Radius, _config);
                if (valid && isValid)
                {
                    isValid = true;
                }
                else
                {
                    isValid = false;
                }
            }
            return isValid;
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

        internal double GetDistanceForNextCluster(Cluster nextCluster)
        {
            var rgNext = Utility.GetRadiusOfGyration(nextCluster);
            var rgExist = Utility.GetRadiusOfGyration(Cluster);
            var nNext = nextCluster.NumberOfPrimaryParticles;
            var nExist = Cluster.Sum(c => c.NumberOfPrimaryParticles);

            return Math.Sqrt(
                (Math.Pow(MeanRadius, 2) * Math.Pow(nNext + nExist, 2) / (nExist * nNext))
                * Math.Pow((nExist + nNext) / _config.Kf, 2 / _config.Df)
                - ((nExist + nNext) / nExist) * Math.Pow(rgNext, 2)
                - ((nExist + nNext) / nNext) * Math.Pow(rgExist, 2));
        }

        internal double MeanRadius => _primaryParticleSizeDistribution.MeanRadius;
    }
}
