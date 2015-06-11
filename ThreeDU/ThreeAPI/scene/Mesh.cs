using System;
using System.Collections.Generic;

namespace ThreeAPI.scene
{
  public class Mesh : IMesh
  {
    private readonly IMeshLoader _meshLoader;
    private readonly List<IVertex> _vertices = new List<IVertex>();
    private readonly List<IPolygon> _polygons = new List<IPolygon>();

    public IEnumerable<IVertex> Vertices
    {
      get { return _vertices; }
    }

    public IEnumerable<IPolygon> Polygons
    {
      get { return _polygons; }
    }

    public Mesh(IMeshLoader meshLoader)
    {
      _meshLoader = meshLoader;
    }

    public void Load(string filePath)
    {
      _meshLoader.Load(this, filePath);
    }

    public void Save(string filePath)
    {
      throw new System.NotImplementedException();
    }

    
    public IPolygon AddPolygon(List<IVertex> vertices)
    {
      _vertices.AddRange(vertices);
      if (vertices.Count == 3)
      {
        var triangle = new Triangle(vertices);
        _polygons.Add(triangle);
        return triangle;
      }
      throw new NotImplementedException();
    }
  }
}