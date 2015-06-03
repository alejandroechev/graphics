using System;

using OpenTK;

namespace ThreeAPI
{
  public class Plane
  {
    public Plane ()
    {
    }

    public static bool Intersect(Plane plane, BoundingBox box){
      return false;
    }

    public static bool Intersect(Plane plane, BoundingSphere sphere){
      return false;
    }

    public static float Distance(Plane plane, Vector3 point){
      return 1.0f;
    } 
  }
}

