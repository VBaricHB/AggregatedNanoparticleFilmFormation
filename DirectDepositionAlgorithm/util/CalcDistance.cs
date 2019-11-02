using DirectDepositionAlgorithm;
using System;

namespace AggregatedNanoparticleFilmFormation.util
{
    public static class CalcDistance
    {
        public static double DistanceSquared(PrimaryParticle particle1,PrimaryParticle particle2)
        {
            var x = Math.Pow(particle2.X - particle1.X, 2);
            var y = Math.Pow(particle2.Y - particle1.Y, 2);
            var z = Math.Pow(particle2.Z - particle1.Z, 2);
            return x + y + z;
        }

        public static double Distance(PrimaryParticle particle1, PrimaryParticle particle2)
        {
            var x = Math.Pow(particle2.X - particle1.X, 2);
            var y = Math.Pow(particle2.Y - particle1.Y, 2);
            var z = Math.Pow(particle2.Z - particle1.Z, 2);
            return Math.Sqrt(x + y + z);
        }
    }
}
