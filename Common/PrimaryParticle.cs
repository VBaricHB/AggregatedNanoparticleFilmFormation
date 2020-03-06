
namespace Common
{
    public class PrimaryParticle
    {
        public int Id { get; }
        public Vector3 Position { get; private set; }

        public double Radius { get; set; }

        public int Type { get; set; }

        public PrimaryParticle(int id, Vector3 position, double radius)
        {
            Id = id;
            Position = position;
            Radius = radius;
            Type = 1;
        }

        public PrimaryParticle(int id, Vector3 position, double radius, int type)
            : this(id, position, radius)
        {
            Type = type;
        }

        public PrimaryParticle(int id, double radius)
            : this(id, new Vector3(0, 0, 0), radius)
        {
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
