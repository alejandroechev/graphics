using System;
using SceneLib;

namespace GPURenderer
{
  public class Sphere : SphereBase
  {
    public Sphere(MeshBase sphereMesh) : base(sphereMesh)
    {
    }

    public override bool Intersect(Ray ray)
    {
      throw new NotImplementedException();
    }

    public override Vector GetNormal(Ray ray)
    {
      throw new NotImplementedException();
    }

    public override Material GetMaterial(Ray ray)
    {
      throw new NotImplementedException();
    }

   
  }
}