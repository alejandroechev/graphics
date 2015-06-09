using System;
using System.Collections.Generic;

namespace SceneLib
{
  /// <summary>
  /// Correspond to a mesh object
  /// </summary>
  public abstract class MeshBase : RenderObject, IHaveTriangles
  {
    private readonly IMeshLoader _meshLoader;
    private readonly IRenderObjectFactory _renderObjectFactory;
    protected readonly List<TriangleBase> _triangles = new List<TriangleBase>(); 

    public string FilePath { get; set; }
    public Material Material { get; set; }
   
    public int NumPolygons { get { return Triangles == null ? 0 : Triangles.Count; } }

    public List<TriangleBase> Triangles
    {
      get { return _triangles; }
    }

    protected MeshBase(string name, string filename, IMeshLoader meshLoader, IRenderObjectFactory renderObjectFactory)
    {
      if (meshLoader == null) throw new ArgumentNullException("meshLoader");
      if (renderObjectFactory == null) throw new ArgumentNullException("renderObjectFactory");
      _meshLoader = meshLoader;
      _renderObjectFactory = renderObjectFactory;
      Name = name;
      FilePath = filename;
    }

    public void Load()
    {
      _meshLoader.Load(this);
    }

    public TriangleBase GetPolygon(int index)
    {
      return Triangles == null || index < 0 || index > Triangles.Count ? null : Triangles[index];
    }

    public void AddPolygon(TriangleBase polygon)
    {
      _triangles.Add(polygon);
    }

    public virtual TriangleBase AddPolygon(List<Vertex> vertices )
    {
      var polygon = _renderObjectFactory.CreateTriangle();
      vertices.ForEach(v => v.Material = Material);
      vertices.ForEach(polygon.AddVertex);
      polygon.Position = Position;
      polygon.Scale = Scale;
      polygon.Rotation = Rotation;
      _triangles.Add(polygon);
      return polygon;
    }

  }
}