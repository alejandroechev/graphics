using OpenTK;
using OpenTK.Graphics;

namespace ThreeAPI.scene
{
  public class Vertex : IVertex
  {
    private Vector3 _position;
    private Vector3 _normal;
    private Vector2 _textureCoords;

    public Vector3 Position
    {
      get { return _position; }
      set { _position = value; }
    }

    public Vector3 Normal
    {
      get { return _normal; }
      set { _normal = value; }
    }

    public Vector2 TextureCoords
    {
      get { return _textureCoords; }
      set { _textureCoords = value; }
    }

    public Vertex(Vector3 position, Vector3 normal, Vector2 textureCoords)
    {
      _position = position;
      _normal = normal;
      _textureCoords = textureCoords;
    }

    public Vertex()
    {
      
    }
  }
}