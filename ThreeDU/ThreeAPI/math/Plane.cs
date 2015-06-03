using System;

using OpenTK;

namespace ThreeAPI
{
  public class Plane
  {
    protected internal Vector3 Normal;
    protected internal float D; // distance to zero

    public Plane (float a, float b, float c, float d)
    {
      Normal = new Vector3 (a, b, c);
      D = d;
    }
    public Plane(Vector3 point, Vector3 normal){
      Normal = normal;
      D = - Vector3.Dot(normal, point);
    }

    public static bool Intersect(Plane plane, BoundingBox box){
      // source: SlimDX math/Plane.cpp

      Vector3 min, max;
      max.X = (plane.Normal.X >= 0.0f) ? box.Minimum.X : box.Maximum.X;
      max.Y = (plane.Normal.Y >= 0.0f) ? box.Minimum.Y : box.Maximum.Y;
      max.Z = (plane.Normal.Z >= 0.0f) ? box.Minimum.Z : box.Maximum.Z;
      min.X = (plane.Normal.X >= 0.0f) ? box.Maximum.X : box.Minimum.X;
      min.Y = (plane.Normal.Y >= 0.0f) ? box.Maximum.Y : box.Minimum.Y;
      min.Z = (plane.Normal.Z >= 0.0f) ? box.Maximum.Z : box.Minimum.Z;

      return Distance (plane, max) <= 0.0f && Distance (plane, min) >= 0.0f;
    }

    public static bool Intersect(Plane plane, BoundingSphere sphere){
      float d = Distance(plane, sphere.Center);
      return Math.Abs(d) <= sphere.Radius;
    }

    public static float Distance(Plane plane, Vector3 point){
      return Vector3.Dot(plane.Normal, point) + plane.D;
    } 
  }
}

