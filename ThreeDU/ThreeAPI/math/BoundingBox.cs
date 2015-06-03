using System;
using OpenTK;

namespace ThreeAPI
{
  public class BoundingBox
  {
    protected internal Vector3 Minimum;
    protected internal Vector3 Maximum;

    public BoundingBox (Vector3 minimum, Vector3 maximum)
    {
      Minimum = minimum;
      Maximum = maximum;
    }

    public static bool Contains(BoundingBox box, Vector3 vector){
      if (box.Minimum.X <= vector.X && vector.X <= box.Maximum.X &&
          box.Minimum.Y <= vector.Y && vector.Y <= box.Maximum.Y &&
          box.Minimum.Z <= vector.Z && vector.Z <= box.Maximum.Z) {
        return true;
      } else {
        return false;
      }
    }

    public static bool Intersects(BoundingBox box1, BoundingBox box2){
      if (box1.Minimum.X > box2.Maximum.X || box1.Maximum.X < box2.Minimum.X) {
        return false;
      }
      if (box1.Minimum.Y > box2.Maximum.Y || box1.Maximum.Y < box2.Minimum.Y) {
        return false;
      }

      return (box1.Minimum.Z <= box2.Maximum.Z && box1.Maximum.Z >= box2.Minimum.Z);
    }

    public static bool Intersects(BoundingBox box, BoundingSphere sphere){
      // get closet point in cube
      Vector3 clamped = Vector3.Clamp (sphere.Center, box.Minimum, box.Maximum);
      float dist = (sphere.Center - clamped).Length;
      return dist <= sphere.Radius;

    }

    public static bool Intersects(BoundingBox box, Plane plane){
      return Plane.Intersect (plane, box);
    }

    public static bool Intersects(BoundingBox box, Ray ray, out float distance){
      return Ray.Intersect (ray, box, out distance);
      
    }

  }
}

