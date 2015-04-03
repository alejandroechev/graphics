using System;
using System.Collections.Generic;
using SceneLib;

namespace Renderer
{
  public class Triangle : TriangleBase, IHaveTextureSampler
  {
    private ITextureSampler _textureSampler;
    
    public ITextureSampler TextureSampler
    {
      get { return _textureSampler; }
      set { _textureSampler = value; }
    }


    public Triangle(ITextureSampler textureSampler)
    {
      _textureSampler = textureSampler;
    }

   
    public override bool Intersect(Ray ray)
    {
      float xa = Vertices[0].Position.X;
      float xb = Vertices[1].Position.X;
      float xc = Vertices[2].Position.X;
      float ya = Vertices[0].Position.Y;
      float yb = Vertices[1].Position.Y;
      float yc = Vertices[2].Position.Y;
      float za = Vertices[0].Position.Z;
      float zb = Vertices[1].Position.Z;
      float zc = Vertices[2].Position.Z;
      float xd = ray.Direction.X;
      float yd = ray.Direction.Y;
      float zd = ray.Direction.Z;
      float xe = ray.Position.X;
      float ye = ray.Position.Y;
      float ze = ray.Position.Z;

      float detA = Determinant(xa - xb, xa - xc, xd, ya - yb, ya - yc, yd, za - zb, za - zc, zd);
      float t = Determinant(xa - xb, xa - xc, xa - xe, ya - yb, ya - yc, ya - ye, za - zb, za - zc, za - ze) / detA;
      if (t < 0)
      {
        return false;
      }
      float gamma = Determinant(xa - xb, xa - xe, xd, ya - yb, ya - ye, yd, za - zb, za - ze, zd) / detA;
      if (gamma < 0 || gamma > 1)
        return false;
      float beta = Determinant(xa - xe, xa - xc, xd, ya - ye, ya - yc, yd, za - ze, za - zc, zd) / detA;
      if ((beta < 0) || (beta > (1 - gamma)))
        return false;

      if (t < ray.IntersectionDistance)
      {
        ray.IntersectionDistance = t;
        ray.IntersectedObject = this;
      }
      return true;
    }

    public override Vector GetNormal(Vector point)
    {
      var barycentricCoordinates = GetBarycentricCoordinates(point);
      var interpolatedNormal = InterpolateProperty(v => v.Normal, barycentricCoordinates);
      return interpolatedNormal.Normalize3();
    }

    public override Material GetMaterial(Vector point)
    {
      var barycentricCoordinates = GetBarycentricCoordinates(point);
      var material = Vertices[0].Material.Clone();
      material.Diffuse = InterpolateProperty(v => v.Material.Diffuse, barycentricCoordinates);
      material.Specular = InterpolateProperty(v => v.Material.Specular, barycentricCoordinates);

      material.Shininess = InterpolateProperty(v => v.Material.Shininess, barycentricCoordinates);
      material.ReflectivityAttenuation = InterpolateProperty(v => v.Material.ReflectivityAttenuation, barycentricCoordinates);
      material.RefractiveAttenuation = InterpolateProperty(v => v.Material.RefractiveAttenuation, barycentricCoordinates);
      material.RefractiveIndex = InterpolateProperty(v => v.Material.RefractiveIndex, barycentricCoordinates);

      if (material.HasDiffuseTexture)
      {
        var textureCoords = InterpolateProperty(v => v.TextureCoordinates, barycentricCoordinates);
        material.Diffuse = material.Diffuse * _textureSampler.Sample(textureCoords, material.GetDiffuseTexturePixel, 
          material.GetDiffuseTextureWidth, material.GetDiffuseTextureHeight);
      }
      return material;
    }

    public Vector GetBarycentricCoordinates(Vector p)
    {
      var a = Vertices[0].Position;
      var b = Vertices[1].Position;
      var c = Vertices[2].Position;
      var n = Vector.Cross3(b - a, c - a);
      var na = Vector.Cross3(c - b, p - b);
      var nb = Vector.Cross3(a - c, p - c);
      var nc = Vector.Cross3(b - a, p - a);
      var alpha = Vector.Dot3(n, na) / Vector.Dot3(n, n);
      var beta = Vector.Dot3(n, nb) / Vector.Dot3(n, n);
      var gamma = Vector.Dot3(n, nc) / Vector.Dot3(n, n);
      return new Vector(alpha, beta, gamma);
    }

    public Vector InterpolateProperty(Func<Vertex, Vector> getProperty, Vector barycentricCoordinates)
    {
      return getProperty(Vertices[0]) * barycentricCoordinates.X + getProperty(Vertices[1]) * barycentricCoordinates.Y +
             getProperty(Vertices[2]) * barycentricCoordinates.Z;
    }

    public float InterpolateProperty(Func<Vertex, float> getProperty, Vector barycentricCoordinates)
    {
      return getProperty(Vertices[0]) * barycentricCoordinates.X + getProperty(Vertices[1]) * barycentricCoordinates.Y +
             getProperty(Vertices[2]) * barycentricCoordinates.Z;
    }

    private static float Determinant(float a, float b, float c, float d, float e, float f, float g, float h, float i)
    {
      return a * e * i - a * f * h - b * d * i + c * d * h + b * f * g - c * e * g;
    }

    public Tuple<float, float> GetXBounds()
    {
      var min = (float)Math.Min(Vertices[0].Position.X, Math.Min(Vertices[1].Position.X, Vertices[2].Position.X));
      var max = (float)Math.Max(Vertices[0].Position.X, Math.Max(Vertices[1].Position.X, Vertices[2].Position.X));

      return new Tuple<float, float>(min, max);
    }

    public Tuple<float, float> GetYBounds()
    {
      var min = (float)Math.Min(Vertices[0].Position.Y, Math.Min(Vertices[1].Position.Y, Vertices[2].Position.Y));
      var max = (float)Math.Max(Vertices[0].Position.Y, Math.Max(Vertices[1].Position.Y, Vertices[2].Position.Y));

      return new Tuple<float, float>(min, max);
    }
  }
}