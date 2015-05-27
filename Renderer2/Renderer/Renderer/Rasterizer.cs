using System;
using System.Collections.Generic;
using System.Linq;
using SceneLib;

namespace Renderer
{
  public enum RasterMode
  {
    Fill,
    Wireframe,
    Points
  }

  public class PointRasterizer : Rasterizer
  {
    public override string Name
    {
      get { return "Rasterizer: Point"; }
    }

    public PointRasterizer(Scene scene, IDisplay display)
      : base(scene, display)
    {
      _rasterMode = RasterMode.Points;
    }
  }

  public class WireFrameRasterizer : Rasterizer
  {
    public override string Name
    {
      get { return "Rasterizer: Wireframe"; }
    }

    public WireFrameRasterizer(Scene scene, IDisplay display)
      : base(scene, display)
    {
      _rasterMode = RasterMode.Wireframe;
    }
  }

  public class Rasterizer : AbstractRenderer
  {


    class VertexOut
    {
      public TriangleBase Triangle { get; set; }
      public Vector Color { get; set; }
      public Vector WorldPosition { get; set; }
      public Vector PositionHomogeneus { get; set; }
      public Vector ImagePosition { get; set; }
      public Vector TextureCoordinates { get; set; }
    }

    class Fragment
    {
      public Vector Position { get; set; }
      public Vector Color { get; set; }
      public Vector TextureCoordinates { get; set; }
      public float Z { get; set; }
    }

    public override string Name
    {
      get { return "Rasterizer: Fill"; }
    }

    private readonly float[,] _zBuffer;
    protected RasterMode _rasterMode = RasterMode.Fill;

    public Rasterizer(Scene scene, IDisplay display)
      : base(scene, display)
    {
      _zBuffer = new float[_scene.Width, _scene.Height];
    }

    public override void Render()
    {
      ClearBuffers();
      var meshes = _scene.Objects.Where(o => o is IHaveTriangles).Cast<IHaveTriangles>().ToList();
      var triangles = new List<Triangle>();
      var objectToWorldMatrices = new Dictionary<IHaveTriangles, Matrix>();
      meshes.ForEach(m => objectToWorldMatrices.Add(m, ObjectToWorld(m)));
      var triangleMatricesDict = new Dictionary<TriangleBase, Matrix>();
      meshes.ForEach(m => m.Triangles.ForEach(t => triangleMatricesDict.Add(t, objectToWorldMatrices[m])));
      meshes.ForEach(m => triangles.AddRange(m.Triangles.Cast<Triangle>().ToList()));

      var projection = RenderingParameters.Instance.UsePerspectiveProjection ? PerspectiveProjection() : OrtographicProjection();
      var view = WorldToCamera();
      var worldToClipping = projection * view;
      var viewPort = ViewPort();

      var originalVertices = new List<Vertex>();
      triangles.ForEach(t => originalVertices.AddRange(t.Vertices));
      var vertices =
        originalVertices
          .Select(v => ProcessVertex(v, v.ParentTriangle, viewPort, worldToClipping, triangleMatricesDict[v.ParentTriangle])).ToList();

      var fragments = new List<Fragment>();
      var triangleGroups = vertices.GroupBy(v => v.Triangle, v => v, (key, g) => g.ToList()).ToList();
      triangleGroups.ForEach(g => fragments.AddRange(Rasterize(g)));

      fragments.ForEach(ProcessFragment);
    }

    private void ClearBuffers()
    {
      for (int i = 0; i < _scene.Width; i++)
      {
        for (int j = 0; j < _scene.Height; j++)
        {
          _zBuffer[i, j] = 0;
          _display.SetPixel(i, j, _scene.BackgroundColor.R, _scene.BackgroundColor.G, _scene.BackgroundColor.B);
        }
      }
    }

    private void ProcessFragment(Fragment fragment)
    {
      if (fragment.Position.X < 0 || fragment.Position.X > _scene.Width - 1 || fragment.Position.Y < 0 ||
          fragment.Position.Y > _scene.Height - 1)
        return;
      if (fragment.Z > _zBuffer[(int)fragment.Position.X, (int)fragment.Position.Y] || !RenderingParameters.Instance.EnableDepthBuffer)
      {
        _zBuffer[(int)fragment.Position.X, (int)fragment.Position.Y] = fragment.Z;
        _display.SetPixel((int)fragment.Position.X, (int)fragment.Position.Y, fragment.Color.R, fragment.Color.G,
          fragment.Color.B);
      }

    }

    private VertexOut ProcessVertex(Vertex vertex, TriangleBase triangle, Matrix viewPort, Matrix worldToClipping, Matrix objectToWorldTransform)
    {
      var vertexOut = new VertexOut();
      vertexOut.Triangle = triangle;
      vertexOut.WorldPosition = objectToWorldTransform * vertex.Position;
      vertex.Normal.W = 0;
      var normal = objectToWorldTransform * vertex.Normal;
      vertexOut.PositionHomogeneus = worldToClipping * vertexOut.WorldPosition;
      vertexOut.ImagePosition = viewPort * vertexOut.PositionHomogeneus;
      vertexOut.ImagePosition /= vertexOut.ImagePosition.W;

      vertexOut.Color = vertex.Material.Diffuse * _scene.AmbientLight;
      foreach (var light in _scene.Lights)
      {
        var viewDirection = (_scene.Camera.Position - vertexOut.WorldPosition).Normalize3();
        var lightPosition = light.Position;
        var lightDirection = (lightPosition - vertexOut.WorldPosition).Normalize3();
        var shadedColor = CalculateBlinnPhongIllumination(viewDirection, lightDirection, light.Color, normal.Normalize3(), vertex.Material);
        vertexOut.Color += shadedColor;
      }
      vertexOut.Color = vertexOut.Color.Clamp3();
      return vertexOut;
    }

    private List<Fragment> Rasterize(List<VertexOut> vertices)
    {
      if (_rasterMode == RasterMode.Wireframe)
        return RasterizeLines(vertices);
      if (_rasterMode == RasterMode.Points)
        return RasterizePoints(vertices);
      return RasterizeTriangle(vertices);
    }

    private List<Fragment> RasterizePoints(List<VertexOut> vertices)
    {
      var fragments = new List<Fragment>();
      foreach (var vertex in vertices)
      {
        fragments.Add(new Fragment() { Color = vertex.Color, Position = new Vector((int)(vertex.ImagePosition.X + 0.5), (int)(vertex.ImagePosition.Y + 0.5)), Z = 1 });
      }
      return fragments;
    }

    private List<Fragment> RasterizeLines(List<VertexOut> vertices)
    {
      var fragments = new List<Fragment>();
      fragments.AddRange(RasterizeLine(vertices[0], vertices[1]));
      fragments.AddRange(RasterizeLine(vertices[1], vertices[2]));
      fragments.AddRange(RasterizeLine(vertices[2], vertices[0]));
      return fragments;
    }

    private List<Fragment> RasterizeLine(VertexOut v1, VertexOut v2)
    {
      var minV = v1.ImagePosition.X < v2.ImagePosition.X ? v1 : v2;
      var maxV = v1.ImagePosition.X >= v2.ImagePosition.X ? v1 : v2;
      var dy = (maxV.ImagePosition.Y - minV.ImagePosition.Y);
      var dx = (maxV.ImagePosition.X - minV.ImagePosition.X);

      if (Math.Abs(dy) < Math.Abs(dx))
        return RasterizeLineInX(v1, v2);
      return RasterizeLineInY(v1, v2);
    }

    private List<Fragment> RasterizeLineInX(VertexOut v1, VertexOut v2)
    {
      var fragments = new List<Fragment>();
      var minV = v1.ImagePosition.X < v2.ImagePosition.X ? v1 : v2;
      var maxV = v1.ImagePosition.X >= v2.ImagePosition.X ? v1 : v2;
      var minC = minV.Color;
      var maxC = maxV.Color;

      var m = (maxV.ImagePosition.Y - minV.ImagePosition.Y) / (maxV.ImagePosition.X - minV.ImagePosition.X);
      var length = (maxV.ImagePosition - minV.ImagePosition).Magnitude3();

      for (int x = Math.Max(0, (int)(minV.ImagePosition.X + 0.5)); x < Math.Min((int)(maxV.ImagePosition.X + 0.5), _scene.Width - 1); x++)
      {
        int y = (int)(m * (x - minV.ImagePosition.X) + minV.ImagePosition.Y + 0.5);
        var t = (new Vector(x, y, 0) - minV.ImagePosition).Magnitude3() / length;
        var color = t * maxC + (1 - t) * minC;
        fragments.Add(new Fragment { Color = color, Position = new Vector(x, y), Z = 1 });
      }
      return fragments;
    }

    private List<Fragment> RasterizeLineInY(VertexOut v1, VertexOut v2)
    {
      var fragments = new List<Fragment>();
      var minV = v1.ImagePosition.Y < v2.ImagePosition.Y ? v1 : v2;
      var maxV = v1.ImagePosition.Y >= v2.ImagePosition.Y ? v1 : v2;

      var minC = minV.Color;
      var maxC = maxV.Color;

      var m = (maxV.ImagePosition.X - minV.ImagePosition.X) / (maxV.ImagePosition.Y - minV.ImagePosition.Y);
      var length = (maxV.ImagePosition - minV.ImagePosition).Magnitude3();

      for (int y = Math.Max(0, (int)(minV.ImagePosition.Y + 0.5)); y < Math.Min((int)(maxV.ImagePosition.Y + 0.5), _scene.Height - 1); y++)
      {
        int x = (int)(m * (y - minV.ImagePosition.Y) + minV.ImagePosition.X + 0.5);
        var t = (new Vector(x, y, 0) - minV.ImagePosition).Magnitude3() / length;
        var color = t * maxC + (1 - t) * minC;
        fragments.Add(new Fragment { Color = color, Position = new Vector(x, y), Z = 1 });
      }
      return fragments;
    }

    private List<Fragment> RasterizeTriangle(List<VertexOut> vertices)
    {
      var fragments = new List<Fragment>();

      var xMin = Math.Min(vertices[0].ImagePosition.X,
        Math.Min(vertices[1].ImagePosition.X, vertices[2].ImagePosition.X));
      var xMax = Math.Max(vertices[0].ImagePosition.X,
        Math.Max(vertices[1].ImagePosition.X, vertices[2].ImagePosition.X));
      var yMin = Math.Min(vertices[0].ImagePosition.Y,
        Math.Min(vertices[1].ImagePosition.Y, vertices[2].ImagePosition.Y));
      var yMax = Math.Max(vertices[0].ImagePosition.Y,
        Math.Max(vertices[1].ImagePosition.Y, vertices[2].ImagePosition.Y));

      for (var x = Math.Max(0, (int)(xMin + 0.5)); x <= Math.Min((int)(xMax + 0.5), _scene.Width - 1); x++)
      {
        for (var y = Math.Max(0, (int)(yMin + 0.5)); y <= Math.Min((int)(yMax + 0.5), _scene.Height - 1); y++)
        {
          var p = new Vector(x, y, 0);
          var bar = GetBarycentricCoordinates(vertices, p);
          if (bar.X >= 0 && bar.X <= 1 && bar.Y >= 0 && bar.Y <= 1 && bar.Z >= 0 && bar.Z <= 1)
          {
            var z = InterpolateProperty(vertices, v => v.ImagePosition.Z, bar);
            var color = InterpolateProperty(vertices, v => v.Color, bar);
            //var textureCoordinate = InterpolateProperty(vertices, v => v.TextureCoordinates, bar);
            var fragment = new Fragment { Color = color, Position = p, Z = z };//, TextureCoordinates = textureCoordinate};
            fragments.Add(fragment);
          }
        }
      }
      return fragments;
    }

   

    private Matrix ObjectToWorld(IHaveTriangles mesh)
    {
      var s = MatrixExtensions.Scaling(mesh.Scale);
      var r = MatrixExtensions.Rotation(mesh.Rotation);
      var t = MatrixExtensions.Translation(mesh.Position);
      return t * r * s;
    }

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
          _scene.Camera.NearClip));
      var s =
        MatrixExtensions.Scaling(new Vector(2.0f / (cameraBounds[Bounds.Right] - cameraBounds[Bounds.Left]), 2.0f / (cameraBounds[Bounds.Top] - cameraBounds[Bounds.Bottom]),
          2.0f / (_scene.Camera.FarClip - _scene.Camera.NearClip)));
      var t2 =
        MatrixExtensions.Translation(new Vector(-1, -1, 1));
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
      var t1 = MatrixExtensions.Translation(new Vector(1, 1, 1));
      var s = MatrixExtensions.Scaling(new Vector(_scene.Width / 2.0f, _scene.Height / 2.0f, 1.0f / 2.0f));
      var t2 = MatrixExtensions.Translation(new Vector(-0.5f, -0.5f, 0));
      return t2 * s * t1;
    }

    private Vector GetBarycentricCoordinates(List<VertexOut> vertices, Vector p)
    {
      var a = vertices[0].ImagePosition;
      var b = vertices[1].ImagePosition;
      var c = vertices[2].ImagePosition;
      var n = Vector.Cross3(b - a, c - a);
      var na = Vector.Cross3(c - b, p - b);
      var nb = Vector.Cross3(a - c, p - c);
      var nc = Vector.Cross3(b - a, p - a);
      var alpha = Vector.Dot3(n, na) / Vector.Dot3(n, n);
      var beta = Vector.Dot3(n, nb) / Vector.Dot3(n, n);
      var gamma = Vector.Dot3(n, nc) / Vector.Dot3(n, n);
      return PerspectiveCorrect(vertices, new Vector(alpha, beta, gamma));
    }

    private Vector PerspectiveCorrect(List<VertexOut> vertices, Vector barycentricCoordinates)
    {
      var d = vertices[1].PositionHomogeneus.W * vertices[2].PositionHomogeneus.W +
              vertices[2].PositionHomogeneus.W * barycentricCoordinates.Y *
              (vertices[0].PositionHomogeneus.W - vertices[1].PositionHomogeneus.W) +
              vertices[1].PositionHomogeneus.W * barycentricCoordinates.Z *
              (vertices[0].PositionHomogeneus.W - vertices[2].PositionHomogeneus.W);
      var correctedBarycentricCoordinates = new Vector();
      correctedBarycentricCoordinates.Y = vertices[0].PositionHomogeneus.W * vertices[2].PositionHomogeneus.W *
                                          barycentricCoordinates.Y / d;
      correctedBarycentricCoordinates.Z = vertices[0].PositionHomogeneus.W * vertices[1].PositionHomogeneus.W *
                                          barycentricCoordinates.Z / d;
      correctedBarycentricCoordinates.X = 1 - correctedBarycentricCoordinates.Y - correctedBarycentricCoordinates.Z;
      return correctedBarycentricCoordinates;
    }

    private Vector InterpolateProperty(List<VertexOut> vertices, Func<VertexOut, Vector> getProperty, Vector barycentricCoordinates)
    {
      return getProperty(vertices[0]) * barycentricCoordinates.X + getProperty(vertices[1]) * barycentricCoordinates.Y +
             getProperty(vertices[2]) * barycentricCoordinates.Z;
    }

    private float InterpolateProperty(List<VertexOut> vertices, Func<VertexOut, float> getProperty, Vector barycentricCoordinates)
    {
      return getProperty(vertices[0]) * barycentricCoordinates.X + getProperty(vertices[1]) * barycentricCoordinates.Y +
             getProperty(vertices[2]) * barycentricCoordinates.Z;
    }
  }
}