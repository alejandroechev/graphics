namespace SceneLib
{
  public interface IRenderObjectFactory
  {
    PolygonBase CreateTriangle();
    SphereBase CreateSphere();
    MeshBase CreateMesh(string name, string filePath);
  }
}