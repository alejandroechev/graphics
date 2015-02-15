using System;
using SceneLib;

namespace Renderer
{
  public class Sphere : SphereBase
  {
    public Sphere(MeshBase sphereMesh) : base(sphereMesh)
    {
    }

    public override bool Intersect(Ray ray)
    {
      var a = Vector.Dot3(ray.Direction, ray.Direction);
      var b = 2 * Vector.Dot3((ray.Position - GetPosition(ray.Time)), ray.Direction);
      var c = Vector.Dot3((ray.Position - GetPosition(ray.Time)), (ray.Position - GetPosition(ray.Time))) - (Radius * Radius);

      var discr = b * b - 4 * a * c;
      if (discr < 0)
        return false;
      discr = (float)Math.Sqrt(discr);
      var t0 = (-b - discr) / (2 * a);
      if (t0 > 0)
      {
        if (t0 < ray.IntersectionDistance)
        {
          ray.IntersectionDistance = t0;
          ray.IntersectedObject = this;
        }
        return true;
      }
      var t1 = (-b + discr) / (2 * a);
      if (t1 > 0)
      {
        if (t1 < ray.IntersectionDistance)
        {
          ray.IntersectionDistance = t1;
          ray.IntersectedObject = this;
        }
      }
      return false;
    }

    public override Vector GetNormal(Vector point, float time)
    {
      var currentPosition = GetPosition(time);
      return (point - currentPosition).Normalize3();
    }

    public override Material GetMaterial(Vector point)
    {
      var clone = Material.Clone();
      var textureCoords = GetTextureCoords(point);

      if (Material.HasDiffuseTexture && RenderingParameters.Instance.EnableTextureMapping)
        clone.Diffuse = clone.Diffuse * Material.SampleDiffuseTexture(textureCoords);
      if (Material.HasSpecularTexture && RenderingParameters.Instance.EnableTextureMapping)
        clone.Specular = clone.Specular * Material.SampleSpecularTexture(textureCoords);

      return clone;
    }

    private Vector GetPosition(float deltaTime)
    {
      return Center + deltaTime * Velocity;
    }

    private Vector GetTextureCoords(Vector point)
    {
      var theta = Math.Acos((point.Y - Center.Y) / Radius);
      var phi = Math.Atan2(point.X - Center.X, point.Z - Center.Z);

      var u = (float)((phi < 0 ? phi + 2 * Math.PI : phi) / (2 * Math.PI));
      var v = 1 - (float)((Math.PI - theta) / Math.PI);

      var textureCoords = new Vector(u, v);
      return textureCoords;
    }
  }
}