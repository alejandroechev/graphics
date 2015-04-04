using System.Collections.Generic;
using SceneLib;

namespace Renderer
{
  public class Box
  {
    private readonly Vector _minPoint;
    private readonly Vector _maxPoint;

    private readonly List<Triangle> _triangles = new List<Triangle>();

    public Box(Vector minPoint, Vector maxPoint)
    {
      _minPoint = minPoint;
      _maxPoint = maxPoint;

      var boxPoints = new List<Vector>();
      for (int i = 0; i < 8; i++)
      {
        boxPoints.Add(_minPoint.Clone());
      }
      boxPoints[1].Y = _maxPoint.Y;
      boxPoints[2].X = _maxPoint.X;
      boxPoints[3].Y = _maxPoint.Y;
      boxPoints[3].X = _maxPoint.X;

      boxPoints[4].Z = _maxPoint.Z;
      boxPoints[5].Z = _maxPoint.Z;
      boxPoints[5].Y = _maxPoint.Y;
      boxPoints[6].Z = _maxPoint.Z;
      boxPoints[6].X = _maxPoint.X;
      boxPoints[7] = _maxPoint.Clone();

      var triangle1 = new Triangle(null);
      triangle1.AddVertex(new Vertex() { Position = boxPoints[0] });
      triangle1.AddVertex(new Vertex() { Position = boxPoints[1] });
      triangle1.AddVertex(new Vertex() { Position = boxPoints[2] });

      var triangle2 = new Triangle(null);
      triangle2.AddVertex(new Vertex() { Position = boxPoints[2] });
      triangle2.AddVertex(new Vertex() { Position = boxPoints[1] });
      triangle2.AddVertex(new Vertex() { Position = boxPoints[3] });

      var triangle3 = new Triangle(null);
      triangle3.AddVertex(new Vertex() { Position = boxPoints[4] });
      triangle3.AddVertex(new Vertex() { Position = boxPoints[5] });
      triangle3.AddVertex(new Vertex() { Position = boxPoints[6] });

      var triangle4 = new Triangle(null);
      triangle4.AddVertex(new Vertex() { Position = boxPoints[6] });
      triangle4.AddVertex(new Vertex() { Position = boxPoints[5] });
      triangle4.AddVertex(new Vertex() { Position = boxPoints[7] });

      var triangle5 = new Triangle(null);
      triangle5.AddVertex(new Vertex() { Position = boxPoints[0] });
      triangle5.AddVertex(new Vertex() { Position = boxPoints[4] });
      triangle5.AddVertex(new Vertex() { Position = boxPoints[5] });

      var triangle6 = new Triangle(null);
      triangle6.AddVertex(new Vertex() { Position = boxPoints[0] });
      triangle6.AddVertex(new Vertex() { Position = boxPoints[5] });
      triangle6.AddVertex(new Vertex() { Position = boxPoints[1] });

      var triangle7 = new Triangle(null);
      triangle7.AddVertex(new Vertex() { Position = boxPoints[2] });
      triangle7.AddVertex(new Vertex() { Position = boxPoints[6] });
      triangle7.AddVertex(new Vertex() { Position = boxPoints[7] });

      var triangle8 = new Triangle(null);
      triangle8.AddVertex(new Vertex() { Position = boxPoints[2] });
      triangle8.AddVertex(new Vertex() { Position = boxPoints[7] });
      triangle8.AddVertex(new Vertex() { Position = boxPoints[3] });

      var triangle9 = new Triangle(null);
      triangle9.AddVertex(new Vertex() { Position = boxPoints[0] });
      triangle9.AddVertex(new Vertex() { Position = boxPoints[4] });
      triangle9.AddVertex(new Vertex() { Position = boxPoints[6] });

      var triangle10 = new Triangle(null);
      triangle10.AddVertex(new Vertex() { Position = boxPoints[0] });
      triangle10.AddVertex(new Vertex() { Position = boxPoints[6] });
      triangle10.AddVertex(new Vertex() { Position = boxPoints[2] });

      var triangle11 = new Triangle(null);
      triangle11.AddVertex(new Vertex() { Position = boxPoints[1] });
      triangle11.AddVertex(new Vertex() { Position = boxPoints[5] });
      triangle11.AddVertex(new Vertex() { Position = boxPoints[7] });

      var triangle12 = new Triangle(null);
      triangle12.AddVertex(new Vertex() { Position = boxPoints[1] });
      triangle12.AddVertex(new Vertex() { Position = boxPoints[7] });
      triangle12.AddVertex(new Vertex() { Position = boxPoints[3] });

      _triangles.Add(triangle1);
      _triangles.Add(triangle2);
      _triangles.Add(triangle3);
      _triangles.Add(triangle4);
      _triangles.Add(triangle5);
      _triangles.Add(triangle6);
      _triangles.Add(triangle7);
      _triangles.Add(triangle8);
      _triangles.Add(triangle9);
      _triangles.Add(triangle10);
      _triangles.Add(triangle11);
      _triangles.Add(triangle12);


    }

    public Vector MinPoint
    {
      get { return _minPoint; }
    }

    public Vector MaxPoint
    {
      get { return _maxPoint; }
    }

    public bool Intersect(Ray ray)
    {
      foreach (var triangle in _triangles)
      {
        if (triangle.Intersect(ray))
          return true;
      }
      return false;
    }
  }
}