using System;
using System.Collections.Generic;
using System.Linq;
using SceneLib;

namespace Renderer
{
  public class Rasterizer : AbstractRenderer
  {
    class VertexOut
    {
      public Triangle Triangle { get; set; }
      public Vector Color { get; set; }
      public Vector WorldPosition { get; set; }
      public Vector PositionHomogeneous { get; set; }
      public Vector ImagePosition { get; set; }
    }

    class PixelIn
    {
      public Vector Position { get; set; }
      public Vector Color { get; set; }
      public Vector TextureCoordinates { get; set; }
    }

    public override string Name
    {
      get { return "Rasterizer"; }
    }

    private readonly float[,] _zBuffer;

    public Rasterizer(Scene scene, IDisplay display)
      : base(scene, display)
    {
      _zBuffer = new float[_scene.Width, _scene.Height];
    }

    public override void Render()
    {
      var meshes = _scene.Objects.Where(o => o is IHaveTriangles).Cast<IHaveTriangles>().ToList();
      var triangles = new List<Triangle>();
      meshes.ForEach(m => m.Triangles.Cast<Triangle>().ToList().ForEach(t => triangles.Add(t)));
      var vertices = new List<VertexOut>();
      triangles.ForEach(t => t.Vertices.ForEach(v => vertices.Add(ProcessVertex(v, t))));
      //vertices.GroupBy(v => v.Triangle).ToList().ForEach(g => RasterizeTriangle());


      //var vertices = triangles.Select(t => t.)
      /*
      vertices' = vertices.AsParallel.Process
      triangles = vertices.Map
      triangles' = triangles.ClippAndCull
      pixels = triangles.AsParallel.Rasterize
      pixels' = pixels.AsParallel.Process
      pixels'' = pixels'.Map
      pixels''' = pizels''.Blend
      pixels'''.Paint*/
    }

    private VertexOut ProcessVertex(Vertex vertex, Triangle triangle)
    {
      var vertexOut = new VertexOut();
      vertexOut.Triangle = triangle;
      var worldToClipping = PerspectiveProjection() * WorldToCamera();
      vertexOut.WorldPosition = vertex.Position;
      vertexOut.PositionHomogeneous = worldToClipping * vertex.Position;
      vertexOut.ImagePosition = ViewPort()*vertexOut.PositionHomogeneous;
      vertexOut.ImagePosition /= vertexOut.ImagePosition.W;

      vertexOut.Color = vertex.Material.Diffuse * _scene.AmbientLight;
      foreach (var light in _scene.Lights)
      {
        var viewDirection = (vertex.Position - _scene.Camera.Position).Normalize3();
        var lightPosition = light.Position;
        var lightDirection = (lightPosition - vertex.Position).Normalize3();
        var shadedColor = CalculateBlinnPhongIllumination(viewDirection, lightDirection, light.Color, vertex.Normal,
          vertex.Material);
        vertexOut.Color += shadedColor;
      }
      return vertexOut;
    }

    private void RasterizeTriangle(Triangle triangle, List<VertexOut> vertices)
    {
      var xBounds = triangle.GetXBounds();
      var yBounds = triangle.GetYBounds();

      //for (var x = Math.Max(0, (int)(xBounds.Item1 + 0.5)); x <= Math.Min((int)(xBounds.Item2 + 0.5), _scene.Width - 1); x++)
      //{
      //  for (var y = Math.Max(0, (int)(yBounds.Item1 + 0.5)); y <= Math.Min((int)(yBounds.Item2 + 0.5), _scene.Height - 1); y++)
      //  {
      //    var p = new Vector(x, y, 0);
      //    var bar = triangle.GetBarycentricCoordinates(p);
      //    bar = PerspectiveCorrect(triangle, bar);
      //    if (bar.X >= 0 && bar.X <= 1 && bar.Y >= 0 && bar.Y <= 1 && bar.Z >= 0 && bar.Z <= 1)
      //    {
      //      var z = triangle.InterpolateProperty(v => v.Position.Z, bar);
      //      var zTest = z > _zBuffer[x, y];
      //      if (zTest)
      //      {
      //        _zBuffer[x, y] = z;
      //        var worldP = triangle.InterpolateProperty(v => v.WorldPosition, bar);
      //        var color = _perPixelShading
      //          ? PhongPixelShading(triangle, bar, worldP, shadowP)
      //          : GouraudPixelShading(triangle, bar, p, shadowP);
      //        PaintPixel(x, y, color);

      //      }
      //    }
      //  }
      //}
    }

    //private Vector PerspectiveCorrect(Triangle triangle, Vector barycentricCoordinates)
    //{
    //  var d = triangle.Vertices[1].PositionHomogeneus.W * triangle.Vertices[2].PositionHomogeneus.W +
    //          triangle.Vertices[2].PositionHomogeneus.W * barycentricCoordinates.Y *
    //          (triangle.Vertices[0].PositionHomogeneus.W - triangle.Vertices[1].PositionHomogeneus.W) +
    //          triangle.Vertices[1].PositionHomogeneus.W * barycentricCoordinates.Z *
    //          (triangle.Vertices[0].PositionHomogeneus.W - triangle.Vertices[2].PositionHomogeneus.W);
    //  var correctedBarycentricCoordinates = new Vector();
    //  correctedBarycentricCoordinates.Y = triangle.Vertices[0].PositionHomogeneus.W * triangle.Vertices[2].PositionHomogeneus.W *
    //                                      barycentricCoordinates.Y / d;
    //  correctedBarycentricCoordinates.Z = triangle.Vertices[0].PositionHomogeneus.W * triangle.Vertices[1].PositionHomogeneus.W *
    //                                      barycentricCoordinates.Z / d;
    //  correctedBarycentricCoordinates.X = 1 - correctedBarycentricCoordinates.Y - correctedBarycentricCoordinates.Z;
    //  return correctedBarycentricCoordinates;
    //}

    private Matrix WorldToCamera()
    {
      var uvw = _scene.Camera.GetCameraCoordinatesBasis();
      var worldToCamera = MatrixExtensions.BasisChangeFromCanonicalFrame(uvw[0], uvw[1], uvw[2], _scene.Camera.Position);
      return worldToCamera;
    }

    private Matrix OrtographicProjection()
    {
      var cameraBounds = _scene.Camera.FrustumBounds(_scene.Width, _scene.Height);
      var t1 =
        MatrixExtensions.Translation(new Vector(-cameraBounds[Bounds.Left], -cameraBounds[Bounds.Bottom],
          _scene.Camera.FarClip));
      var s =
        MatrixExtensions.Scaling(new Vector(2.0f / (cameraBounds[Bounds.Right] - cameraBounds[Bounds.Left]), 2.0f / (cameraBounds[Bounds.Top] - cameraBounds[Bounds.Bottom]),
          2.0f / (_scene.Camera.FarClip - _scene.Camera.NearClip)));
      var t2 =
        MatrixExtensions.Translation(new Vector(-1, -1, -1));
      return t2 * s * t1;
    }

    private Matrix PerspectiveProjection()
    {
      var orto = OrtographicProjection();
      var p = Matrix.Identity();
      p.M33 = (-_scene.Camera.NearClip + -_scene.Camera.FarClip) / -_scene.Camera.NearClip;
      p.M34 = _scene.Camera.FarClip;
      p.M44 = 0;
      p.M43 = -1 / _scene.Camera.NearClip;
      return orto * p;
    }

    private Matrix ViewPort()
    {
      var t1 =
        MatrixExtensions.Translation(new Vector(1, 1, 1));
      var s =
        MatrixExtensions.Scaling(new Vector(_scene.Width / 2.0f, _scene.Height / 2.0f, 1));
      var t2 =
        MatrixExtensions.Translation(new Vector(-0.5f, -0.5f, 0));
      return t2 * s * t1;
    }
  }
}