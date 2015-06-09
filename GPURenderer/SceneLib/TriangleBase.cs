using System.Collections.Generic;

namespace SceneLib
{
  /// <summary>
  /// Corresponds to a triangle geometrical object
  /// </summary>
  public abstract class TriangleBase : RenderObject, IHaveTriangles
  {
    private readonly List<Vertex> _vertices = new List<Vertex>();

    public List<TriangleBase> Triangles
    {
      get { return new List<TriangleBase> { this }; }
    }

    public List<Vertex> Vertices
    {
      get { return _vertices; }
    }

    public void AddVertex(Vertex vertex)
    {
      vertex.ParentTriangle = this;
      _vertices.Add(vertex);
    }

    public Vector GetFaceNormal()
    {
      var v1V0 = _vertices[1].Position - _vertices[0].Position;
      var v2V0 = _vertices[2].Position - _vertices[0].Position;

      var normal = Vector.Cross3(v1V0, v2V0);
      return normal;
    }
  }
}