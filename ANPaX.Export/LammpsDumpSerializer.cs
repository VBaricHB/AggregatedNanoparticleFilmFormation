using System;
using System.Globalization;
using System.IO;

using ANPaX.Core;
using ANPaX.FilmFormation.interfaces;

namespace ANPaX.Export
{
    public class LammpsDumpSerializer : ISerializer
    {

        public void Serialize<T>(T output, string filename)
        {
            if (typeof(T) == typeof(IParticleFilm<Aggregate>))
            {
                var film = output as IParticleFilm<Aggregate>;
                SerializeParticleFilm(film, filename);
            }
        }

        private void SerializeParticleFilm(IParticleFilm<Aggregate> particleFilm, string filename)
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

        private string GetPrimaryParticleString(PrimaryParticle particle, Cluster cluster, Aggregate agg)
        {
            return $"{particle.Id} {particle.Type} {agg.Id} {cluster.Id} {DoubleToString(particle.Position.X)} {DoubleToString(particle.Position.Y)} {DoubleToString(particle.Position.Z)} {DoubleToString(particle.Radius)}{Environment.NewLine}";

        }

        private string DoubleToString(double value)
        {
            return Math.Round(value, 6).ToString(CultureInfo.InvariantCulture);
        }

        private string GetBoxDimensionString(IParticleFilm<Aggregate> particleFilm)
        {
            var xdim = $"{particleFilm.SimulationBox.XDim.Lower} {particleFilm.SimulationBox.XDim.Upper}";
            var ydim = $"{particleFilm.SimulationBox.YDim.Lower} {particleFilm.SimulationBox.YDim.Upper}";
            var zdim = $"{particleFilm.SimulationBox.ZDim.Lower} {particleFilm.SimulationBox.ZDim.Upper}";
            return $"ITEM: BOX BOUNDS pp pp pp{Environment.NewLine}{xdim}{Environment.NewLine}{ydim}{Environment.NewLine}{zdim}{Environment.NewLine}";
        }
    }
}
