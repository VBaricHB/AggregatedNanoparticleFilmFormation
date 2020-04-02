﻿using System;

namespace CommonLibrary
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

        public static bool operator ==(Vector3 other, Vector3 self)
        {
            return other.X == self.X && other.Y == self.Y & other.Z == self.Z;
        }

        public static bool operator !=(Vector3 other, Vector3 self)
        {
            return other.X != self.X || other.Y != self.Y || other.Z == self.Z;
        }

        public double[] ToArray()
        {
            return new double[] { X,Y,Z};
        }

        public double[] ToXYArray()
        {
            return new double[] { X, Y };
        }

        public override bool Equals(object obj)
        {
            return obj is Vector3 vector &&
                   X == vector.X &&
                   Y == vector.Y &&
                   Z == vector.Z;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        public override string ToString()
        {
            return $"{Math.Round(X, 3)} {Math.Round(Y, 3)} {Math.Round(Z, 3)}";
        }
    }
}