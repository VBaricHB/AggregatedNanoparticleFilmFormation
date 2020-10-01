using System;
using System.Globalization;
using System.IO;

using ANPaX.Core;
using ANPaX.Core.interfaces;

namespace ANPaX.IO
{
    internal static class LammpsDumpSerializer
    {
        public static void SerializeParticleFilm(IParticleFilm<Aggregate> particleFilm, string filename)
        {
            using var outfile = new StreamWriter(filename);
            outfile.Write($"ITEM: TIMESTEP{Environment.NewLine}0{Environment.NewLine}");
            outfile.Write($"ITEM: NUMBER OF ATOMS{Environment.NewLine}{particleFilm.NumberOfPrimaryParticles}{Environment.NewLine}");
            outfile.Write(GetBoxDimensionString(particleFilm));
            outfile.Write($"ITEM: ATOMS id type aggregate cluster x y z Radius{Environment.NewLine}");
            foreach (var agg in particleFilm.Particles)
            {
                foreach (var cluster in agg.Cluster)
                {
                    foreach (var particle in cluster.PrimaryParticles)
                    {
                        outfile.Write(GetPrimaryParticleString(particle, cluster, agg));
                    }
                }
            }
        }

        private static string GetPrimaryParticleString(PrimaryParticle particle, Cluster cluster, Aggregate agg)
        {
            return $"{particle.Id} {particle.Type} {agg.Id} {cluster.Id} {DoubleToString(particle.Position.X)} {DoubleToString(particle.Position.Y)} {DoubleToString(particle.Position.Z)} {DoubleToString(particle.Radius)}{Environment.NewLine}";

        }

        private static string DoubleToString(double value)
        {
            return Math.Round(value, 6).ToString(CultureInfo.InvariantCulture);
        }

        private static string GetBoxDimensionString(IParticleFilm<Aggregate> particleFilm)
        {
            var xdim = $"{DoubleToString(particleFilm.SimulationBox.XDim.Lower)} {DoubleToString(particleFilm.SimulationBox.XDim.Upper)}";
            var ydim = $"{DoubleToString(particleFilm.SimulationBox.YDim.Lower)} {DoubleToString(particleFilm.SimulationBox.YDim.Upper)}";
            var zdim = $"{DoubleToString(particleFilm.SimulationBox.ZDim.Lower)} {DoubleToString(particleFilm.SimulationBox.ZDim.Upper)}";
            return $"ITEM: BOX BOUNDS pp pp pp{Environment.NewLine}{xdim}{Environment.NewLine}{ydim}{Environment.NewLine}{zdim}{Environment.NewLine}";
        }
    }
}
