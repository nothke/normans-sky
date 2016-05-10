using UnityEngine;
using System.Collections;

namespace VectorExtensions
{
    public struct Vector3i
    {
        public int x;
        public int y;
        public int z;

        public Vector3i(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3i(Vector3 vector3f)
        {
            x = Mathf.RoundToInt(vector3f.x);
            y = Mathf.RoundToInt(vector3f.y);
            z = Mathf.RoundToInt(vector3f.z);
        }

        public static Vector3i zero
        {
            get { return new Vector3i(0, 0, 0); }
        }

        public static Vector3i one
        {
            get { return new Vector3i(1, 1, 1); }
        }

        public static Vector3i operator +(Vector3i a, Vector3i b)
        {
            return new Vector3i(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3i operator -(Vector3i a, Vector3i b)
        {
            return new Vector3i(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3i operator *(Vector3i a, int scalar)
        {
            return new Vector3i(a.x * scalar, a.y * scalar, a.z * scalar);
        }

        public static Vector3i operator /(Vector3i a, int divisor)
        {
            return new Vector3i(a.x / divisor, a.y / divisor, a.z / divisor);
        }

        public static bool operator ==(Vector3i a, Vector3i b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public static bool operator !=(Vector3i a, Vector3i b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z ? false : true;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Vector3i v = (Vector3i)obj;
            return x == v.x && y == v.y && z == v.z;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            throw new System.NotImplementedException();
            //return base.GetHashCode();
        }
    }
}
