using System;
using UnityEngine;

namespace Utils
{
    // My version of Vector3Int generates 0 garbage when accessing as a key in dictionary, due to IEquatable inteface
    public struct Vector3i : IEquatable<Vector3i>
    {
        public static readonly Vector3i zero = new Vector3i(0, 0, 0);
        public static readonly Vector3i one = new Vector3i(1, 1, 1);
        public static readonly Vector3i up = new Vector3i(0, 1, 0);
        public static readonly Vector3i down = new Vector3i(0, -1, 0);
        public static readonly Vector3i left = new Vector3i(-1, 0, 0);
        public static readonly Vector3i right = new Vector3i(1, 0, 0);
        
        public readonly int x;
        public readonly int y;
        public readonly int z;

        public Vector3i(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public int ManhattanDistance(Vector3i v2)
        {
            return Math.Abs(v2.x - x) + Math.Abs(v2.y - y) + Math.Abs(v2.z - z);
        }

        public static implicit operator Vector3(Vector3i v)
        {
            return new Vector3(v.x, v.y, v.z);
        }

        public static Vector3i FloorToInt(Vector3 v)
        {
            return new Vector3i(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y), Mathf.FloorToInt(v.z));
        }

        public static Vector3i RoundToInt(Vector3 v)
        {
            return new Vector3i(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
        }

        public static Vector3i operator +(Vector3i a, Vector3i b)
        {
            return new Vector3i(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3i operator -(Vector3i a, Vector3i b)
        {
            return new Vector3i(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3i operator *(Vector3i a, Vector3i b)
        {
            return new Vector3i(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static Vector3i operator *(Vector3i a, int b)
        {
            return new Vector3i(a.x * b, a.y * b, a.z * b);
        }

        public static Vector3i operator /(Vector3i a, int b)
        {
            return new Vector3i(a.x / b, a.y / b, a.z / b);
        }

        public static bool operator ==(Vector3i lhs, Vector3i rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
        }

        public static bool operator !=(Vector3i lhs, Vector3i rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object other)
        {
            if (!(other is Vector3i))
                return false;
            return this == (Vector3i) other;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() << 2 ^ z.GetHashCode() >> 2;
        }

        public bool Equals(Vector3i other)
        {
            return this == other;
        }

        public override string ToString()
        {
            return $"({(object) x}, {(object) y}, {(object) z})";
        }
    }
}