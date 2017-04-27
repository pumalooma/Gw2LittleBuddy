
using System;

public class Vector3 {
    public float x;
    public float y;
    public float z;

    public Vector3(float x, float y, float z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public Vector3 (float [] arr) {
        x = arr[0];
        y = arr[1];
        z = arr[2];
    }

    public static Vector3 operator + (Vector3 a, Vector3 b) {
        return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public static Vector3 operator - (Vector3 a, Vector3 b) {
        return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
    }

	public override string ToString() {
		return string.Format("{0}, {1}, {2}", x, y, z);
	}

	public Vector3 Cross(Vector3 vec)
	{
		return new Vector3(y* vec.z - z* vec.y, z* vec.x - x* vec.z, x* vec.y - y* vec.x);
	}

	public static Vector3 Up() {
		return new Vector3(0.0f, 1.0f, 0.0f);
	}
}

public static class Vec
{
    public static float Distance (Vector3 p1, Vector3 p2) {
        return Magnitude(p1 - p2);
    }

    public static float Magnitude(Vector3 v) {
        return (float)Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
    }

    public static float DotProduct(Vector3 a, Vector3 b) {
        return a.x * b.x + a.y * b.y + a.z * b.z;
    }
	
    public static Vector3 Normalize(Vector3 v) {
        float magnitude = Magnitude(v);
        if (magnitude < float.Epsilon)
            return new Vector3(0.0f, 0.0f, 0.0f);

        float invLen = 1.0f / magnitude;
        return new Vector3(v.x * invLen, v.y * invLen, v.z * invLen);
    }
}
