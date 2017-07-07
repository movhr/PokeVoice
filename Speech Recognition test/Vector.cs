using System;

namespace Speech_Recognition_test
{
    public class Vector : IComparable<Vector>
    {
        public int X;
        public int Y;
        public Vector()
        {
            X = Y = 0;
        }

        public Vector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int CompareTo(Vector other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var xComparison = X.CompareTo(other.X);
            if (xComparison != 0) return xComparison;
            return Y.CompareTo(other.Y);
        }

        public void Reset()
        {
            X = Y = 0;
        }
    }
}