
namespace Common
{
    public class PrimaryParticle
    {
        public Vector3 Position { get; private set; }

        public double Radius { get; set; }

        public PrimaryParticle(Vector3 position, double radius)
        {
            Position = position;
            Radius = radius;
        }

        public PrimaryParticle(double radius)
        {
            Position = new Vector3(0,0,0);
            Radius = radius;
        }

        public void MoveBy(Vector3 vector)
        {
            Position += vector;
        }

        public void MoveTo(Vector3 vector)
        {
            Position = vector;
        }
    }
}
