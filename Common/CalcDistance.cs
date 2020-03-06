using System;

namespace Common
{
    public static class CalcDistance
    {
        public static double DistanceSquared(PrimaryParticle particle1,PrimaryParticle particle2)
        {
            var x = Math.Pow(particle2.Position.X - particle1.Position.X, 2);
            var y = Math.Pow(particle2.Position.Y - particle1.Position.Y, 2);
            var z = Math.Pow(particle2.Position.Z - particle1.Position.Z, 2);
            return x + y + z;
        }

        public static double Distance(PrimaryParticle particle1, PrimaryParticle particle2)
        {
            var x = Math.Pow(particle2.Position.X - particle1.Position.X, 2);
            var y = Math.Pow(particle2.Position.Y - particle1.Position.Y, 2);
            var z = Math.Pow(particle2.Position.Z - particle1.Position.Z, 2);
            return Math.Round(Math.Sqrt(x + y + z),6);
        }
    }
}
