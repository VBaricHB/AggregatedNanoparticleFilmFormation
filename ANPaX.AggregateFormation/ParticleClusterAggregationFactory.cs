using ANPaX.Collection;
using ANPaX.AggregateFormation.interfaces;
using System.Collections.Generic;
using System;
using System.Linq;
using ANPaX.Collection.interfaces;
using System.Diagnostics;
using NLog;
using ANPaX.Extensions;

namespace ANPaX.AggregateFormation
{
    internal static class ParticleClusterAggregationFactory
    {
        public static Cluster Build(int size, ISizeDistribution<double> psd, Random rndGen, IConfig config, ILogger logger)
        {
            var cluster = Procedure(size, psd, rndGen, config, logger);
            while (cluster is null)
            {
                cluster = Procedure(size, psd, rndGen, config, logger);
            }

            return cluster;
        }

        public static Cluster Procedure(int size, ISizeDistribution<double> psd, Random rndGen, IConfig config, ILogger logger)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var primaryParticles = new List<PrimaryParticle>();
            primaryParticles = SetFirstPrimaryParticle(primaryParticles, psd);
            primaryParticles = SetSecondPrimaryParticle(primaryParticles, psd, rndGen, config);
            while (primaryParticles.Count < size)
            {
                if (!AddNextPrimaryParticle(primaryParticles, stopwatch, size, psd, rndGen, config, logger))
                {
                    return null;
                }
            }

            return new Cluster(0, primaryParticles);
        }

        public static List<PrimaryParticle> SetFirstPrimaryParticle(List<PrimaryParticle> primaryParticles, ISizeDistribution<double> psd)
        {
            var radius = psd.GetRandomSize();
            primaryParticles.Add(new PrimaryParticle(0, new Vector3(0, 0, 0), radius));
            return primaryParticles;
        }

        public static List<PrimaryParticle> SetSecondPrimaryParticle(List<PrimaryParticle> primaryParticles, ISizeDistribution<double> psd, Random rndGen, IConfig config)
        {
            var radius = psd.GetRandomSize();
            // Distance of the CenterOfMass (com) of the second pp from the com
            // of the first pp
            var particle = new PrimaryParticle(0, radius);
            var ppDistance = config.Epsilon * (primaryParticles[0].Radius + particle.Radius);
            var rndPosition = ParticleFormationService.GetRandomPosition(rndGen, ppDistance);
            particle.MoveTo(rndPosition);
            primaryParticles.Add(particle);
            return primaryParticles;
        }

        public static bool AddNextPrimaryParticle(List<PrimaryParticle> primaryParticles, Stopwatch stopwatch, int size,
                                                  ISizeDistribution<double> psd, Random rndGen, IConfig config,
                                                  ILogger logger)
        {
            var com = primaryParticles.GetCenterOfMass();
            var ppDistance = GetNextPrimaryParticleDistance(primaryParticles, psd, config);
            var particle = InitializeNewPrimaryParticle(psd.GetRandomSize());

            var found = false;
            Vector3 rndPosition = new Vector3();
            var count = 0;
            while (!found)
            {
                rndPosition = ParticleFormationService.GetRandomPosition(rndGen, ppDistance) + com;
                found = TrySetPrimaryParticle(particle, rndPosition, primaryParticles, config);
                if (stopwatch.ElapsedMilliseconds > config.MaxTimePerClusterMilliseconds)
                {
                    logger.Debug("Resetting cluster generation. Time limit exceeded.");
                    stopwatch.Restart();
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
        
        internal static bool TrySetPrimaryParticle(PrimaryParticle particle, Vector3 rndPosition, List<PrimaryParticle> primaryParticles, IConfig config)
        {
            var tree = primaryParticles.ToNeighborsList();
            var searchRadius = (particle.Radius + primaryParticles.Max(p => p.Radius)) * config.Delta;
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
                var (nearby, feasible) = ParticleFormationService.IsValidPosition(neigh, primaryParticles, particle.Radius, config);
                anyNearby = anyNearby || nearby;
                allFeasible = allFeasible && feasible;
            }
            return anyNearby && allFeasible;
        }

        private static double GetNextPrimaryParticleDistance(List<PrimaryParticle> primaryParticles, ISizeDistribution<double> psd, IConfig config)
        {
            var n = primaryParticles.Count + 1;
            var rsq = Math.Pow(n, 2) * Math.Pow(psd.Mean, 2) / (n - 1)
                    * Math.Pow(n / config.Kf, 2 / config.Df)
                    - n * Math.Pow(psd.Mean, 2) / (n - 1)
                    - n * Math.Pow(psd.Mean, 2) * Math.Pow((n - 1) / config.Kf, 2 / config.Df);
            return Math.Sqrt(rsq);
        }
      
    }
}



