using System.Collections.Generic;
using OpenTK;

namespace ThreeAPI.scene
{
  public class Triangle : IPolygon
  {
    private readonly List<IVertex> _vertices = new List<IVertex>();
 
    public IEnumerable<IVertex> Vertices
    {
      get { return _vertices; }
    }

    public Triangle(List<IVertex> vertices)
    {
      _vertices = vertices;
    }

    public Vector3 GetFaceNormal()
    {
      var v10 = _vertices[1].Position - _vertices[0].Position;
      var v20 = _vertices[2].Position - _vertices[0].Position;
      var cross = Vector3.Cross(v10, v20);
      cross.Normalize();
      return cross;
    }
  }
}