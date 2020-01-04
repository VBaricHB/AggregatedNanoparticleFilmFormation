using System.Collections.Generic;
using System.Linq;
using Common;
namespace Export
{
    public class ExportToLAMMPS
    {
        private List<PrimaryParticle> PrimaryParticles { get; }

        public ExportToLAMMPS(Cluster cluster)
        {
            PrimaryParticles = cluster.PrimaryParticles.ToList();

        }

        public void WriteToFile(string file)
        {
            var text = "ITEM: TIMESTEP\n0\n";
            text += $"ITEM: NUMBER OF ATOMS\n{PrimaryParticles.Count}\n";
            text += "ITEM: BOX BOUNDS pp pp pp\n0 0\n0 0\n0 0\n";
            text += "ITEM ATOMS id type x y z Radius\n";
            var index = 1;
            foreach (var particle in PrimaryParticles)
            {
                text += $"{index++} 1 {particle.Position.X} {particle.Position.Y} {particle.Position.Z} {particle.Radius}\n";
            }
            System.IO.File.WriteAllText(file, text);
        }
    }
}
