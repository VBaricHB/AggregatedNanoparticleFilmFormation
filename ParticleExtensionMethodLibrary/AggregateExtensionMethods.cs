using CommonLibrary;
using System.Linq;

namespace ParticleExtensionMethodLibrary
{
    public static class AggregateExtensionMethods
    {
        public static void MoveBy(this Aggregate aggregate, Vector3 vector)
        {
            foreach (var cluster in aggregate.Cluster)
            {
                cluster.MoveBy(vector);
            }
        }

        public static void MoveTo(this Aggregate aggregate, Vector3 vector)
        {
            var moveBy = vector - aggregate.Cluster.GetCenterOfMass();

            foreach (var particle in aggregate.Cluster.SelectMany(c => c.PrimaryParticles))
            {
                particle.MoveBy(moveBy);
            }
        }
    }
}
