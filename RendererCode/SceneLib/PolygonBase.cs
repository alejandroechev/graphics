using System.Collections.Generic;

namespace SceneLib
{
  /// <summary>
  /// Corresponds to a triangle geometrical object
  /// </summary>
  public abstract class PolygonBase : RenderObject, IHavePolygons
  {
    private readonly List<Vertex> _vertices = new List<Vertex>();

    public List<PolygonBase> Polygons
    {
      get { return new List<PolygonBase> { this }; }
    }

    public List<Vertex> Vertices
    {
      get { return _vertices; }
    }

    public void AddVertex(Vertex vertex)
    {
      _vertices.Add(vertex);
    }

    public Vector GetFaceNormal()
    {
      var v1v0 = _vertices[1].Position - _vertices[0].Position;
      var v2v0 = _vertices[2].Position - _vertices[0].Position;

      var normal = Vector.Cross3(v1v0, v2v0);
      return normal;
    }
  }
}