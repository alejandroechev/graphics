using System;
using System.Collections.Generic;
using SceneLib;

namespace Renderer
{
  public class Mesh : MeshBase
  {
    private TriangleBase _currentIntersectedPolygon;
    private Box _boundingBox;

    public Mesh(string name, string filename, IMeshLoader meshLoader, IRenderObjectFactory renderObjectFactory)
      : base(name, filename, meshLoader, renderObjectFactory)
    {
      var minPoint = new Vector(float.MaxValue, float.MaxValue, float.MaxValue);
      var maxPoint = new Vector(-float.MaxValue, -float.MaxValue, -float.MaxValue);
      _boundingBox = new Box(minPoint, maxPoint);
    }

    public override TriangleBase AddPolygon(List<Vertex> vertices)
    {
      foreach (var vertex in vertices)
      {
        if (vertex.Position.X < _boundingBox.MinPoint.X)
          _boundingBox.MinPoint.X = vertex.Position.X;
        if (vertex.Position.Y < _boundingBox.MinPoint.Y)
          _boundingBox.MinPoint.Y = vertex.Position.Y;
        if (vertex.Position.Z < _boundingBox.MinPoint.Z)
          _boundingBox.MinPoint.Z = vertex.Position.Z;
        if (vertex.Position.X > _boundingBox.MaxPoint.X)
          _boundingBox.MaxPoint.X = vertex.Position.X;
        if (vertex.Position.Y > _boundingBox.MaxPoint.Y)
          _boundingBox.MaxPoint.Y = vertex.Position.Y;
        if (vertex.Position.Z > _boundingBox.MaxPoint.Z)
          _boundingBox.MaxPoint.Z = vertex.Position.Z;
      }
      return base.AddPolygon(vertices);
    }

    public override bool Intersect(Ray ray)
    {
      if (!_boundingBox.Intersect(ray))
        return false;

      float tMin = float.MaxValue;
      foreach (var polygon in Triangles)
      {
        if (polygon.Intersect(ray))
        {
          if (ray.IntersectionDistance < tMin)
          {
            tMin = ray.IntersectionDistance;
            _currentIntersectedPolygon = polygon;
          }
        }
      }
      return tMin < float.MaxValue;
    }

    public override Vector GetNormal(Ray ray)
    {
      if (_currentIntersectedPolygon == null) throw new ApplicationException("Missing current polygon intersected. Either Intersect method wasnt called, or it didnt found an intersection with this mesh");
      return _currentIntersectedPolygon.GetNormal(ray);
    }

    public override Material GetMaterial(Ray ray)
    {
      if (_currentIntersectedPolygon == null) throw new ApplicationException("Missing current polygon intersected. Either Intersect method wasnt called, or it didnt found an intersection with this mesh");
      return _currentIntersectedPolygon.GetMaterial(ray);
    }
  }
}
