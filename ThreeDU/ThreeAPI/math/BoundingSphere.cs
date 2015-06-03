using System;
using OpenTK;

namespace ThreeAPI
{
  public class BoundingSphere
  {
    protected internal Vector3 Center;
    protected internal float Radius;
    public BoundingSphere (Vector3 center, float radius)
    {
      Center = center;
      Radius = radius;
    }

    public static bool Contains(BoundingSphere sphere, Vector3 vector){
      return false;
    }

    public static bool Intersects(BoundingSphere sphere1, BoundingSphere sphere2){
      return false;
    }
    public static bool Intersects(BoundingSphere sphere, BoundingBox box){
      return BoundingBox.Intersects(box, sphere);
    }

    public static bool Intersects(BoundingSphere sphere, Plane plane){
      return Plane.Intersect (plane, sphere);
    }

    public static bool Intersects(BoundingSphere sphere, Ray ray, out float distance){
      return Ray.Intersect (ray, sphere, out distance);

    }
  }
}

