using System.Collections.Generic;
using System.Linq;
using CommonLibrary.interfaces;

namespace CommonLibrary
{
    public class Aggregate : IAggregate
    {
        public Aggregate(List<Cluster> cluster)
        {
            Cluster = cluster; 
        }

        public List<Cluster> Cluster { get; private set; }
        public int NumberOfPrimaryParticles => Cluster.Sum(c => c.PrimaryParticles.Count);
        public int NumberOfClusters => Cluster.Count;
        public bool IsDeposited { get; set; }

        public void MoveBy(Vector3 vector)
        {
            foreach(var cluster in Cluster)
            {
                cluster.MoveBy(vector);
            }
        }

        public void MoveTo(Vector3 vector)
        {
            var moveBy = vector - ParticleFormationService.GetCenterOfMass(Cluster.SelectMany(c => c.PrimaryParticles));

            foreach (var particle in Cluster.SelectMany(c => c.PrimaryParticles))
            {
                particle.MoveBy(moveBy);
            }
        }
    }
}
