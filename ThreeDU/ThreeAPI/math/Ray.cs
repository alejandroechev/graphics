using System;

namespace ThreeAPI
{
  public class Ray
  {
    public Ray ()
    {
    }

    public static bool Intersect(Ray ray, BoundingBox box, out float distance){
      distance = 1.0f;
      return false;
    }

    public static bool Intersect(Ray ray, BoundingSphere sphere, out float distance){
      distance = 1.0f;
      return false;
    }

    public static bool Intersect(Ray ray, Plane plane, out float distance){
      distance = 1.0f;
      return false;
    }
  }
}

