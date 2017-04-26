
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
		return new Vector3(y* vec.z - z* vec.y,
						   z* vec.x - x* vec.z,
						   x* vec.y - y* vec.x);
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


    public static float DotProductXZ (Vector3 a, Vector3 b) {
        return a.x * b.x + a.y * b.y + a.z * b.z;
    }

    public static Vector3 Normalize(Vector3 v) {
        float magnitude = Magnitude(v);
        if (magnitude < float.Epsilon)
            return new Vector3(0.0f, 0.0f, 0.0f);

        float invLen = 1.0f / magnitude;
        return new Vector3(v.x * invLen, v.y * invLen, v.z * invLen);
    }

/*	public static float DistanceXZ( Vector3 a, Vector3 b )
	{
		return Vector2.Distance( new Vector2( a.x, a.z ), new Vector2( b.x, b.z ) );
	}

	public static float DistanceSqr( Vector3 a, Vector3 b )
	{
		return Vector3.SqrMagnitude( b - a );
	}

	public static float DistanceSqrSkewed(Vector3 a, Vector3 b, float strength)
	{
		return Vector3.SqrMagnitude( new Vector3(b.x,b.y*strength,b.z) - new Vector3(a.x, a.y * strength, a.z) );
	}

	public static Vector3 VectorToTargetXZ( Vector3 a, Vector3 b )
	{
		return new Vector3( b.x - a.x, 0.0f, b.z - a.z );
	}

	public static Vector3 VectorToTarget( Vector3 a, Vector3 b )
	{
		return b - a;
	}

	public static Vector3 VectorXZ( Vector3 v )
	{
		return (new Vector3( v.x, 0.0f, v.z )).normalized;
	}

	// takes in a number from -1 to 1 and returns back the angle equivalent
	public static float SafeAcos( float d )
	{
		return Mathf.Acos( Mathf.Clamp( d, -1.0f, 1.0f) ) * Mathf.Rad2Deg;
	}
	
	// return back the angle between the transform and another direction vector
	public static float AngleBetweenVectors( Transform transform, Vector3 vec )
	{
		float angle = SafeAcos( Vector3.Dot( transform.forward, vec ) );
		return Vector3.Dot( transform.right, vec ) >= 0.0f ? angle : -angle;
	}
	
	// return back the angle between the transform and another direction vector
	public static float AngleBetweenVectors2( Transform transform, Vector3 vec ) 		 // currently unused
	{
		float x = Vector3.Dot( transform.forward, vec );
		float z = Vector3.Dot( transform.right, vec );
		
		return Mathf.Atan2( z, x ) * Mathf.Rad2Deg;
	}
	
	// given a direction vector, this will return the world-oriented angle representing it
	public static float GlobalAngle( Vector3 vec )										 // currently unused
	{
		return Mathf.Atan2( vec.x, vec.z );
	}*/
}
