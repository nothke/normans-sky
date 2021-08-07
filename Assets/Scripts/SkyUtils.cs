using UnityEngine;
using System.Collections;

namespace SkyUtils
{
    public struct Vector3d
    {
        public double x, y, z;

        public Vector3d(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3d(Vector3 v3)
        {
            x = v3.x;
            y = v3.y;
            z = v3.z;
        }

        public static Vector3d operator +(Vector3d a, Vector3d b)
        {
            return new Vector3d(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3d operator +(Vector3d a, Vector3 b)
        {
            return new Vector3d(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3d operator -(Vector3d a, Vector3d b)
        {
            return new Vector3d(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3d operator -(Vector3d a, Vector3 b)
        {
            return new Vector3d(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static bool operator ==(Vector3d a, Vector3d b)
        {
            if (a.x == b.x && a.y == b.y && a.z == b.z)
                return true;
            else
                return false;
        }

        public static bool operator !=(Vector3d a, Vector3d b)
        {
            if (a.x == b.x && a.y == b.y && a.z == b.z)
                return false;
            else
                return true;
        }

        public static Vector3d zero = new Vector3d(0, 0, 0);

        public override bool Equals(object obj)
        {
            return obj is Vector3d d &&
                   x == d.x &&
                   y == d.y &&
                   z == d.z;
        }

        public override int GetHashCode()
        {
            int hashCode = 373119288;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            hashCode = hashCode * -1521134295 + z.GetHashCode();
            return hashCode;
        }

        public static explicit operator Vector3(Vector3d v) => new Vector3((float)v.x, (float)v.y, (float)v.z);
        public static implicit operator Vector3d(Vector3 v) => new Vector3d(v.x, v.y, v.z);
    }
}
