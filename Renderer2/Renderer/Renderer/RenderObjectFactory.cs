using System;
using SceneLib;

namespace Renderer
{
  public class RenderObjectFactory : IRenderObjectFactory
  {
    private readonly IMeshLoader _meshLoader;
    private const string SphereMeshPath = "Meshes/sphere.obj";

    public RenderObjectFactory(IMeshLoader meshLoader)
    {
      if (meshLoader == null) throw new ArgumentNullException("meshLoader");
      _meshLoader = meshLoader;
    }


    public TriangleBase CreateTriangle()
    {
      return new Triangle();
    }

    public SphereBase CreateSphere()
    {
      var mesh = CreateMesh(string.Empty, SphereMeshPath);
      return new Sphere(mesh);
    }

    public MeshBase CreateMesh(string name, string filePath)
    {
      return new Mesh(name, filePath, _meshLoader, this);
    }
  }
}