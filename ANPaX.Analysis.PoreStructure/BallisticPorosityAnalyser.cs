using System;
using System.Globalization;
using System.Linq;

using ANPaX.Core;
using ANPaX.Core.interfaces;

namespace ANPaX.Analysis.PoreStructure
{
    public class BallisticPorosityAnalyser
    {
        private Random _rndGen;
        private readonly INeighborslistFactory _neighborslistFactory;

        public BallisticPorosityAnalyser()
        {
            _rndGen = new Random();
            _neighborslistFactory = new AccordNeighborslistFactory();
        }
        public double GetPorosity<T>(IParticleFilm<T> particleFilm, int numberOfLocations)
        {
            var verticalLimits = GetVerticalFilmLimits(particleFilm);
            var horizontalLimits = GetHorizontalFilmLimits(particleFilm);
            var porosity = ComputePorosityInLimits(particleFilm, verticalLimits, horizontalLimits, numberOfLocations);

            return porosity;
        }

        private double ComputePorosityInLimits<T>(IParticleFilm<T> particleFilm, double[] verticalLimits, double[] horizontalLimits, int numberOfLocations)
        {
            var poreHits = 0;
            var neighborsList = _neighborslistFactory.Build3DNeighborslist(particleFilm.PrimaryParticles);
            var maxRadius = particleFilm.PrimaryParticles.GetMaxRadius();
            for (var i = 0; i < numberOfLocations; i++)
            {
                poreHits += Convert.ToInt32(IsRandomLocationInPore(verticalLimits, horizontalLimits, neighborsList, maxRadius));
            }

            return poreHits / Convert.ToDouble(numberOfLocations, CultureInfo.InvariantCulture);
        }

        private bool IsRandomLocationInPore(double[] vLim, double[] hLim, INeighborslist neighborsList, double maxRadius)
        {
            var randomLoc = GetRandomLocation(vLim, hLim);

            var neighbors = neighborsList.GetPrimaryParticlesAndDistanceWithinRadius(randomLoc, maxRadius);
            if (!neighbors.Any())
            {
                return true;
            }
            else
            {
                // if the distance is smaller than radius: particle hit
                if (neighbors.Count(n => n.Item2 < n.Item1.Radius) > 0)
                {
                    return false;
                }

                // no neighboring particle is closer than its radius
                return true;
            }
        }

        private Vector3 GetRandomLocation(double[] vLim, double[] hLim)
        {
            var randomX = hLim[0] + _rndGen.NextDouble() * (hLim[1] - hLim[0]);
            var randomY = hLim[0] + _rndGen.NextDouble() * (hLim[1] - hLim[0]);
            var randomZ = vLim[0] + _rndGen.NextDouble() * (vLim[1] - vLim[0]);
            var randomLoc = new Vector3(randomX, randomY, randomZ);
            return randomLoc;
        }

        private double[] GetHorizontalFilmLimits<T>(IParticleFilm<T> particleFilm)
        {
            return new double[]
                {
                    0.9 * particleFilm.SimulationBox.XDim.Lower,
                    0.9 * particleFilm.SimulationBox.XDim.Upper
                };
        }

        private double[] GetVerticalFilmLimits<T>(IParticleFilm<T> particleFilm)
        {
            return new[] { 0.1 * particleFilm.SimulationBox.ZDim.Lower, 0.9 * particleFilm.SimulationBox.ZDim.Upper };
        }
    }
}
