using System;

namespace SceneLib
{
  public class Vector
  {
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public float W { get; set; }

    public float R { get { return X; } set { X = value; } }
    public float G { get { return Y; } set { Y = value; } }
    public float B { get { return Z; } set { Z = value; } }
    public float A { get { return W; } set { W = value; } }

    public float U { get { return X; } set { X = value; } }
    public float V { get { return Y; } set { Y = value; } }

    public Vector(float x, float y, float z, float w)
    {
      X = x;
      Y = y;
      Z = z;
      W = w;
    }

    public Vector(float x, float y, float z)
      : this(x, y, z, 1)
    {
    }

    public Vector(float x, float y)
      : this(x, y, 0, 1)
    {
    }

    public Vector()
      : this(0, 0, 0, 1)
    {
    }

    /// <summary>
    /// Returns the magnitude of a 3-dimensional vector
    /// </summary>
    /// <returns></returns>
    public float Magnitude3()
    {
      return (float)Math.Sqrt(X * X + Y * Y + Z * Z);
    }

    /// <summary>
    /// Normalizes a 3-dimensional vector
    /// </summary>
    public Vector Normalize3()
    {
      float currentMagnitude = Magnitude3();
      X /= currentMagnitude;
      Y /= currentMagnitude;
      Z /= currentMagnitude;
      return this;
    }

    public Vector Clamp3()
    {
      X = Math.Min(X, 1);
      Y = Math.Min(Y, 1);
      Z = Math.Min(Z, 1);
      return this;
    }

    public static Vector operator +(Vector v1, Vector v2)
    {
      return new Vector(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z, v1.W + v2.W);
    }

    public static Vector operator -(Vector v1, Vector v2)
    {
      return new Vector(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z, v1.W - v2.W);
    }

    public static Vector operator *(float scalar, Vector v)
    {
      return new Vector(scalar * v.X, scalar * v.Y, scalar * v.Z, scalar * v.W);
    }

    public static Vector operator *(Vector v, float scalar)
    {
      return new Vector(scalar * v.X, scalar * v.Y, scalar * v.Z, scalar * v.W);
    }

    public static Vector operator *(Vector v1, Vector v2)
    {
      return new Vector(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z, v1.W * v2.W);
    }

    public static Vector operator /(Vector v, float scalar)
    {
      return new Vector(v.X / scalar, v.Y / scalar, v.Z / scalar, v.W / scalar);
    }

    public static Vector operator /(Vector v1, Vector v2)
    {
      return new Vector(v1.X / v2.X, v1.Y / v2.Y, v1.Z / v2.Z, v1.W / v2.W);
    }

    public static Vector Cross3(Vector v1, Vector v2)
    {
      var crossVec = new Vector();
      crossVec.X = v1.Y * v2.Z - v2.Y * v1.Z;
      crossVec.Y = v2.X * v1.Z - v1.X * v2.Z;
      crossVec.Z = v1.X * v2.Y - v2.X * v1.Y;
      crossVec.W = 0.0f;
      return crossVec;
    }

    /// <summary>
    /// Dot product of a 3-dimensional vector
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static float Dot3(Vector v1, Vector v2)
    {
      return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
    }

    public float[] ToArray3()
    {
      var array = new float[3];
      array[0] = X;
      array[1] = Y;
      array[2] = Z;
      return array;
    }

    public float[] ToArray4()
    {
      var array = new float[4];
      array[0] = X;
      array[1] = Y;
      array[2] = Z;
      array[3] = W;
      return array;
    }

    public override string ToString()
    {
      return "x: " + X + ", y: " + Y + ", z: " + Z + ", w: " + W;
    }

    public Vector Clone()
    {
      return new Vector(X,Y,Z,W);
    }
  }
}
