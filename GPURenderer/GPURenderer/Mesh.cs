using System;
using SceneLib;

namespace GPURenderer
{
  public class Mesh : MeshBase
  {
    
    public Mesh(string name, string filename, IMeshLoader meshLoader, IRenderObjectFactory renderObjectFactory)
      : base(name, filename, meshLoader, renderObjectFactory)
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
