
namespace Common
{
    public class PrimaryParticle
    {
        public Vector3 Position { get; private set; }

        public double Radius { get; set; }

        public int Type { get; set; }

        public PrimaryParticle(Vector3 position, double radius)
        {
            Position = position;
            Radius = radius;
            Type = 1;
        }

        public PrimaryParticle(Vector3 position, double radius, int type)
            : this(position, radius)
        {
            Type = type;
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

        public override string ToString()
        {
            return $"{Type} {Position.X} {Position.Y} {Position.Z} {Radius}";
        }
        
    }
}
