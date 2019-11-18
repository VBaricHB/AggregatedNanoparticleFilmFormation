using Common;
using AggregateFormation.interfaces;
using System.Collections.Generic;
using System;
using System.Linq;
using Accord.Collections;

namespace AggregateFormation
{
    public class ParticleClusterAggregation : IParticleFactory<Cluster>
    {
        private readonly IPrimaryParticleSizeDistribution _psd;
        private readonly Random _rndGen;
        private readonly IConfig _config;

        private List<PrimaryParticle> PrimaryParticles;
        private int TargetClusterSize { get; }


        public ParticleClusterAggregation(int targetClusterSize,IPrimaryParticleSizeDistribution primaryParticleSizeDistribution, IConfig config)
        {
            _psd = primaryParticleSizeDistribution;
            _rndGen = new Random();
            _config = config;
            PrimaryParticles = new List<PrimaryParticle>();
            TargetClusterSize = targetClusterSize;
        }

        public Cluster Build()
        {

            SetFirstPrimaryParticle();
            SetSecondPrimaryParticle();
            while(PrimaryParticles.Count < TargetClusterSize)
            {
                AddNextPrimaryParticle();
            }

            return new Cluster(PrimaryParticles);
        }

        private void SetFirstPrimaryParticle()
        {
            var radius = _psd.GetRadiusByProbability(_rndGen.NextDouble());
            PrimaryParticles.Add(new PrimaryParticle(new Vector3(0, 0, 0), radius));
        }

        private void SetSecondPrimaryParticle()
        {
            var radius = _psd.GetRadiusByProbability(_rndGen.NextDouble());
            // Distance of the CenterOfMass (com) of the second pp from the com
            // of the first pp
            var particle = new PrimaryParticle(radius);
            var distance = _config.Epsilon * (PrimaryParticles[0].Radius + particle.Radius);
            var rndPosition = GetRandomPosition(distance);
            particle.MoveTo(rndPosition);
            PrimaryParticles.Add(particle);
        }

        private void AddNextPrimaryParticle()
        {
            var com = GetCenterOfMass();
            var distance = GetNextPrimaryParticleDistance();
            var radius = _psd.GetRadiusByProbability(_rndGen.NextDouble());
            var particle = new PrimaryParticle(radius);
            var tree = BuildNeighborsList();

            var found = false;
            while (!found)
            {
                var rndPosition = GetRandomPosition(distance);
                double[] query = (rndPosition + com).ToArray();

                var neighbors = tree.Nearest(query,
                    radius: (radius + PrimaryParticles.Max(p => p.Radius))
                            * _config.Delta);

                int c = 0;
                int f = 0;
                foreach ( var neigh in neighbors)
                {
                    var r2 = GetRadiusOfPrimaryParticleAtPosition(neigh.Node.Position);
                    if (neigh.Distance )

                }
            }
        }

        for kk in neighbors:
            # --- Calculate the distance
            distance = func.distance(x_c[clus1][kk], xx, y_c[clus1][kk], yy, z_c[clus1][kk], zz)
            # Check whether the radius rrr is within the section defined by epsilon and delta
            if distance > epsilon* (r_c[clus1][k] + r_c[clus1][kk]) and distance<delta * (r_c[clus1][k] + r_c[clus1][kk]):
                c += 1
            # Check whether the radius rrr is too low
            if distance<epsilon* (r_c[clus1][k] + r_c[clus1][kk]):
                f += 1

        if (f == 0 and c>=1):           # Save position of the particle
            x_c[clus1][k] = xx
            y_c[clus1][k] = yy
z_c[clus1][k] = zz
done1 = True
        i += 1                          # Trial Counter
        if i > iteration_limit:         # Too many attempts to set a particle are reached
            PCA = True
            break 

        private double GetRadiusOfPrimaryParticleAtPosition(double[] position)
        {
            return PrimaryParticles.FirstOrDefault(p => Math.Abs(p.Position.X - position[0]) < 0.01
                                                     && Math.Abs(p.Position.Y - position[1]) < 0.01
                                                     && Math.Abs(p.Position.Z - position[2]) < 0.01
                                                  ).Radius;
        }

        private Vector3 GetCenterOfMass()
        {
            var mass = PrimaryParticles.Sum(p => Math.Pow(p.Radius, 3));
            var x = PrimaryParticles.Sum(p => Math.Pow(p.Radius, 3) * p.Position.X) / mass;
            var y = PrimaryParticles.Sum(p => Math.Pow(p.Radius, 3) * p.Position.Y) / mass;
            var z = PrimaryParticles.Sum(p => Math.Pow(p.Radius, 3) * p.Position.Z) / mass;
            return new Vector3(x, y, z);
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

        private Vector3 GetRandomPosition(double distance)
        {
            var z = distance * (1 - 2 * _rndGen.NextDouble());
            var theta = Math.Acos(z / distance);
            var phi = _rndGen.NextDouble() * 2 * Math.PI;

            var x = distance * Math.Cos(phi) * Math.Sin(theta);
            var y = distance * Math.Sin(phi) * Math.Sin(theta);
            return new Vector3(x, y, z);
        }

        private KDTree<double> BuildNeighborsList()
        {
            return KDTree.FromData<double>(GetPositionArray());
        }

        private double[][] GetPositionArray()
        {
            var array = new double[PrimaryParticles.Count][];
            for (var p = 0; p < PrimaryParticles.Count; p++)
            {
                array[p] = PrimaryParticles[p].Position.ToArray();
            }
            return array;
        }
    }
}



