using System;
using SceneLib;

namespace Renderer
{
  public class Sphere : SphereBase, IHaveTextureSampler
  {
    private ITextureSampler _textureSampler;

    public ITextureSampler TextureSampler
    {
      get { return _textureSampler; }
      set { _textureSampler = value; }
    }

    public Sphere(MeshBase sphereMesh, ITextureSampler textureSampler)
      : base(sphereMesh)
    {
      _textureSampler = textureSampler;
    }

    public override bool Intersect(Ray ray)
    {
      var a = Vector.Dot3(ray.Direction, ray.Direction);
      var b = 2 * Vector.Dot3((ray.Position - Center), ray.Direction);
      var c = Vector.Dot3((ray.Position - Center), (ray.Position - Center)) - (Radius * Radius);

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

    public override Vector GetNormal(Vector point)
    {
      var currentPosition = Center;
      return (point - currentPosition).Normalize3();
    }

    public override Material GetMaterial(Vector point)
    {
      var material = Material.Clone();
      if (material.HasDiffuseTexture)
      {
        var textureCoords = GetTextureCoords(point);
        material.Diffuse = material.Diffuse * _textureSampler.Sample(textureCoords, material.GetDiffuseTexturePixel,
          material.GetDiffuseTextureWidth, material.GetDiffuseTextureHeight);
      }
      return material;
    }


    private Vector GetTextureCoords(Vector point)
    {
      var theta = Math.Acos((point.Y - Center.Y) / Radius);
      var phi = Math.Atan2(point.X - Center.X, point.Z - Center.Z);

      var u = (float)((phi < 0 ? phi + 2 * Math.PI : phi) / (2 * Math.PI));
      var v = 1 - (float)((Math.PI - theta) / Math.PI);

      if (u < 0)
        u = 0;
      if (u > 1)
        u = 1;
      if (v < 0)
        v = 0;
      if (v > 1)
        v = 1;


      var textureCoords = new Vector(u, v);
      return textureCoords;
    }

    
  }
}