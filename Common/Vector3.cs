namespace Common
{
    public struct Vector3
    {
        public double X { get; }
        public double Y { get; }
        public double Z { get; }

        public Vector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3 operator +(Vector3 self, Vector3 other)
        {
            return new Vector3(self.X + other.X,
                               self.Y + other.Y,
                               self.Z + other.Z);
        }

        public static Vector3 operator -(Vector3 self, Vector3 other)
        {
            return new Vector3(self.X - other.X,
                               self.Y - other.Y,
                               self.Z - other.Z);
        }

        public static Vector3 operator *(Vector3 self, int other)
        {
            return new Vector3(self.X * other,
                               self.Y * other,
                               self.Z * other);
        }

        public static Vector3 operator *(int other, Vector3 self)
        {
            return self * other;
        }

        public double[] ToArray()
        {
            return new double[] { X,Y,Z};
        }
    }
}
