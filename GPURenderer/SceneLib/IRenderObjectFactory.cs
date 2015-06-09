namespace SceneLib
{
  public interface IRenderObjectFactory
  {
    TriangleBase CreateTriangle();
    SphereBase CreateSphere();
    MeshBase CreateMesh(string name, string filePath);
  }
}