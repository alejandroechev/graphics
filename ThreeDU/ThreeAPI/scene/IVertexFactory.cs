using OpenTK;

namespace ThreeAPI.scene
{
  public interface IVertexFactory
  {
    IVertex CreateVertex(Vector3 position, Vector3 normal, Vector2 textureCoordinates);
  }
}