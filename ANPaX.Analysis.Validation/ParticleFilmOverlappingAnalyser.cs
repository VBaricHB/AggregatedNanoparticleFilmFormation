using System.Collections.Generic;
using System.Threading.Tasks;

using Accord.Math;

using ANPaX.Core;
using ANPaX.Core.interfaces;

namespace ANPaX.Analysis.Validation
{
    public static class ParticleFilmOverlappingAnalyser
    {
        public static async Task<int> GetNumberOfOverlappingPrimaryParticles_Async(IParticleFilm<Aggregate> particleFilm, bool highlightPrimaryParticle)
        {
            var neighborslistFactory = new AccordNeighborslistFactory();
            var neighborslist = await neighborslistFactory.Build3DNeighborslist_Async(particleFilm.PrimaryParticles);
            var tasks = new List<Task<int>>();
            foreach (var primaryParticle in particleFilm.PrimaryParticles)
            {
                tasks.Add(Task.Run(() => GetPrimaryParticleOverlaps(primaryParticle, particleFilm, neighborslist, highlightPrimaryParticle)));
            }
            var counts = await Task.WhenAll(tasks);

            return counts.Sum();
        }

        private static int GetPrimaryParticleOverlaps(PrimaryParticle primaryParticle, IParticleFilm<Aggregate> particleFilm, INeighborslist neighborslist, bool highlightPrimaryParticle)
        {
            var neighbors = neighborslist.GetPrimaryParticlesAndDistanceWithinRadius(primaryParticle.Position, primaryParticle.Radius + particleFilm.PrimaryParticles.GetMaxRadius());
            var count = 0;
            foreach (var neigh in neighbors)
            {
                if (primaryParticle == neigh.Item1)
                {
                    continue;
                }
                if (primaryParticle.Radius + neigh.Item1.Radius - neigh.Item2 > 1e-6)
                {
                    count++;
                    if (highlightPrimaryParticle)
                    {
                        primaryParticle.Type = 2;
                    }

                }
            }
            return count;
        }
    }
}
