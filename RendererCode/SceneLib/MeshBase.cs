using System;
using System.Collections.Generic;

namespace SceneLib
{
  /// <summary>
  /// Correspond to a mesh object
  /// </summary>
  public abstract class MeshBase : RenderObject, IHavePolygons
  {
    private readonly IMeshLoader _meshLoader;
    private readonly IRenderObjectFactory _renderObjectFactory;
    private readonly List<PolygonBase> _polygons = new List<PolygonBase>(); 

    public string FilePath { get; set; }
    public Material Material { get; set; }
   
    public int NumPolygons { get { return Polygons == null ? 0 : Polygons.Count; } }

    public List<PolygonBase> Polygons
    {
      get { return _polygons; }
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

    public PolygonBase GetPolygon(int index)
    {
      return Polygons == null || index < 0 || index > Polygons.Count ? null : Polygons[index];
    }

    public void AddPolygon(PolygonBase polygon)
    {
      _polygons.Add(polygon);
    }

    public PolygonBase AddPolygon(List<Vertex> vertices )
    {
      var polygon = _renderObjectFactory.CreateTriangle();
      vertices.ForEach(v => v.Material = Material);
      vertices.ForEach(polygon.AddVertex);
      polygon.Position = Position;
      polygon.Scale = Scale;
      polygon.Rotation = Rotation;
      _polygons.Add(polygon);
      return polygon;
    }

  }
}