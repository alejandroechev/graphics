using System;
using SceneLib;

namespace Renderer
{
  public class RenderObjectFactory : IRenderObjectFactory
  {
    private readonly IMeshLoader _meshLoader;
    private const string SphereMeshPath = "Meshes/sphere.obj";
    private readonly TextureSamplerFactory _textureSamplerFactory;

    public RenderObjectFactory(IMeshLoader meshLoader, TextureSamplerFactory textureSamplerFactory)
    {
      if (meshLoader == null) throw new ArgumentNullException("meshLoader");
      _meshLoader = meshLoader;
      _textureSamplerFactory = textureSamplerFactory;
    }


    public TriangleBase CreateTriangle()
    {
      return new Triangle(_textureSamplerFactory.CreateNearestNeighbourSampler());
    }

    public SphereBase CreateSphere()
    {
      var mesh = CreateMesh(string.Empty, SphereMeshPath);
      return new Sphere(mesh, _textureSamplerFactory.CreateNearestNeighbourSampler());
    }

    public MeshBase CreateMesh(string name, string filePath)
    {
      return new Mesh(name, filePath, _meshLoader, this);
    }
  }
}