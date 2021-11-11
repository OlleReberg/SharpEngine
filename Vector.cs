using System;

namespace SharpEngine
{
    public struct Vector
    {
        public float x, y, z;
        public Vector(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public Vector(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0;
        }
        public static Vector operator *(Vector v, float f)
        {
            return new Vector(v.x * f, v.y * f, v.z * f);
        }
        public static Vector operator +(Vector lhs, Vector rhs)
        {
            return new Vector(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
        }
        public static Vector operator -(Vector lhs, Vector rhs)
        {
            return new Vector(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
        }
        public static Vector operator /(Vector v, float f)
        {
            return new Vector(v.x / f, v.y / f, v.z / f);
        }
        // Vectors = Direction and Magnitude
        // Position Vectors = Origin + Vector = Vector
        // Vector Addition and Subtraction: (3   2)  (6   5)   = (3+6     2+5) = (9    7)
        // Vector Magnitude: sqrt(x^2 + y^2 + z^2)
        // Vector Scalar Multiplication: (3    2) * 2 = (3 * 2     2 * 2) = (6    4)
        public static Vector Max(Vector a, Vector b)
        {
            return new Vector(MathF.Max(a.x, b.x), MathF.Max(a.y, b.y), MathF.Max(a.z, b.z));
        }
        public static Vector Min(Vector a, Vector b) 
        {
            return new Vector(MathF.Min(a.x, b.x), MathF.Min(a.y, b.y), MathF.Min(a.z, b.z));
        }
    }
}