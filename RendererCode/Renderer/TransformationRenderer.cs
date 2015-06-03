using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using Renderer.Base;
using SceneLib;

namespace Renderer
{
  public class TransformationRenderer : BaseRenderer
  {
    private bool _usePerspective = true;
    private bool _showWireframe;
    private bool _correctPerspective = true;
    private bool _enableDepthTest = true;
    private bool _enableClipping = false;

    private bool _perPixelShading = false;
    private bool _enableShadowMapping = false;
    private bool _enableShadowMappingBias = false;
    private bool _enableReflectionMapping = false;

    private readonly float[,] _zBuffer;
    private readonly float[,] _shadowMap;
    private Matrix _shadowViewProjection = Matrix.Identity();

    public TransformationRenderer(Scene scene)
      : base(scene)
    {
      _zBuffer = new float[scene.Width, scene.Height];
      _shadowMap = new float[scene.Width, scene.Height];
    }

    public override void OnKeyPress(KeyPressEventArgs e)
    {
      if (e.KeyChar == 'c')
        _enableClipping = !_enableClipping;
      if (e.KeyChar == 'p')
        _correctPerspective = !_correctPerspective;
      if (e.KeyChar == 'y')
        _scene.NextCamera();
      //if (e.KeyChar == 'o')
      //  _showWireframe = !_showWireframe;
      //if (e.KeyChar == 'p')
      //  _perPixelShading = !_perPixelShading;
      if (e.KeyChar == 'n')
        _scene.NextCamera();
      if (e.KeyChar == 'm')
        _enableShadowMapping = !_enableShadowMapping;
      if (e.KeyChar == 'r')
        _enableReflectionMapping = !_enableReflectionMapping;
      if (e.KeyChar == 'b')
        _enableShadowMappingBias = !_enableShadowMappingBias;
   
    }

    public override void Render()
    {
      if(_enableShadowMapping)
        GenerateShadowMap();
      ClearScreen();

      var triangles = VertexProcessing();

      TriangleProcessing(triangles);
    }

    private void GenerateShadowMap()
    {
      ClearScreen();

      _scene.NextCamera();
      _scene.Camera.Position = _scene.Lights.First().Position;

      var triangles = Viewing(true);
      TriangleProcessing(triangles, false);
      SaveShadowMap();

      _scene.NextCamera();
    }

    private void TriangleProcessing(IEnumerable<Triangle> triangles, bool updateFrameBuffer = true)
    {
      if (_enableClipping)
        triangles = ClippingAndCulling(triangles);

      Rasterization(triangles, updateFrameBuffer);
    }

    private IEnumerable<Triangle> VertexProcessing()
    {
      if (!_perPixelShading)
        VertexShading();

      var triangles = Viewing();
      return triangles;
    }

    private void SaveShadowMap()
    {
      for (int i = 0; i < _scene.Width; i++)
      {
        for (int j = 0; j < _scene.Height; j++)
        {
          _shadowMap[i, j] = _zBuffer[i, j];
        }
      }
    }

    private void ClearScreen()
    {
      for (int i = 0; i < _scene.Width; i++)
      {
        for (int j = 0; j < _scene.Height; j++)
        {
          PaintPixel(i, j, _scene.BackgroundColor);
          _zBuffer[i, j] = -1;
        }
      }
    }



    private void VertexShading()
    {
      foreach (var renderObject in _scene.Objects)
      {
        var model = ModelToWorld(renderObject);
        var inverseTranspose = model.Inverse().Transpose();
        if (renderObject is IHavePolygons)
        {
          var triangleContainer = renderObject as IHavePolygons;
          foreach (var triangle in triangleContainer.Polygons)
          {
            foreach (var vertex in triangle.Vertices)
            {
              var outputVertexPosition = model * vertex.Position;
              var normal = vertex.Normal;
              normal.W = 0;
              var outputNormal = inverseTranspose * normal;
              outputNormal.Normalize3();

              var color = RenderingParameters.Instance.EnableAmbient ? _scene.AmbientLight * vertex.Material.Diffuse : new Vector(0, 0, 0);
              foreach (var light in _scene.Lights)
              {
                color += CalculateBlinnPhongIllumination((_scene.Camera.Position - outputVertexPosition).Normalize3(),
                  (light.Position - outputVertexPosition).Normalize3(), light.Color, outputNormal, vertex.Material);
              }
              vertex.Color = color;
            }
          }
        }
      }
    }

    private IEnumerable<Triangle> Viewing(bool generatingShadowMap = false)
    {
      var outputTriangles = new List<Triangle>();

      var projection = !_usePerspective ? OrtographicProjection() : PerspectiveProjection();
      var viewprojectionimage = ViewPort() * projection * WorldToCamera();
      if (generatingShadowMap)
      {
        _shadowViewProjection = viewprojectionimage;
      }

      foreach (var renderObject in _scene.Objects)
      {
        var model = ModelToWorld(renderObject);
        var inverseTranspose = model.Inverse().Transpose();
        if (renderObject is IHavePolygons)
        {
          var triangleContainer = renderObject as IHavePolygons;
          foreach (var triangle in triangleContainer.Polygons)
          {
            var outputTriangle = new Triangle();
            foreach (var vertex in triangle.Vertices)
            {
              var outputVertex = new Vertex();
              var outputVertexPosition = model * vertex.Position;
              outputVertex.WorldPosition = outputVertexPosition;
              var normal = vertex.Normal;
              normal.W = 0;
              var outputNormal = inverseTranspose * normal;
              outputNormal.Normalize3();
              outputVertex.Normal = outputNormal;

              var d = (outputVertex.WorldPosition - _scene.Camera.Position).Normalize3();
              var r = d - 2*(Vector.Dot3(d, outputVertex.Normal))*outputVertex.Normal;
              r.Normalize3();
              outputVertex.ReflectionDirection = r;
              if (_enableShadowMapping && !generatingShadowMap)
              {
                var shadowPosition = _shadowViewProjection * outputVertexPosition;
                outputVertex.ShadowPosition = shadowPosition * (1.0f / shadowPosition.W);
              }
              outputVertexPosition = viewprojectionimage * outputVertexPosition;
              outputVertex.Position = outputVertexPosition * (1.0f / outputVertexPosition.W);
              outputVertex.PositionHomogeneus = outputVertexPosition;
              outputVertex.Color = vertex.Color;
              outputVertex.Material = vertex.Material;
              outputVertex.TextureCoordinates = vertex.TextureCoordinates;

              

              outputTriangle.AddVertex(outputVertex);
            }
            outputTriangles.Add(outputTriangle);
          }
        }
      }
      return outputTriangles;
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

    private IEnumerable<Triangle> ClippingAndCulling(IEnumerable<Triangle> triangles)
    {
      var clippedTriangles = new List<Triangle>();
      foreach (var triangle in triangles)
      {
        var numberOfVerticesBehindNearPlane = 0;
        foreach (var vertex in triangle.Vertices)
        {
          if (vertex.PositionHomogeneus.Z > vertex.PositionHomogeneus.W)
            numberOfVerticesBehindNearPlane++;
        }
        if (numberOfVerticesBehindNearPlane == 0)
          clippedTriangles.Add(triangle);
      }
      return clippedTriangles;
    }

    private void Rasterization(IEnumerable<Triangle> triangles, bool updateFrameBuffer = true)
    {
      foreach (var triangle in triangles)
      {
        if (_showWireframe)
          RasterizeLines(triangle);
        else
          RasterizeTriangle(triangle, updateFrameBuffer);
      }
    }

    private void RasterizeLines(Triangle triangle)
    {
      RasterizeLine(triangle.Vertices[0], triangle.Vertices[1]);
      RasterizeLine(triangle.Vertices[1], triangle.Vertices[2]);
      RasterizeLine(triangle.Vertices[2], triangle.Vertices[0]);
    }

    private void RasterizeLine(Vertex v1, Vertex v2)
    {
      var minV = v1.Position.X < v2.Position.X ? v1 : v2;
      var maxV = v1.Position.X >= v2.Position.X ? v1 : v2;
      var dy = (maxV.Position.Y - minV.Position.Y);
      var dx = (maxV.Position.X - minV.Position.X);

      if (Math.Abs(dy) < Math.Abs(dx))
        RasterizeLineInX(v1, v2);
      else
        RasterizeLineInY(v1, v2);
    }

    private void RasterizeLineInX(Vertex v1, Vertex v2)
    {
      var minV = v1.Position.X < v2.Position.X ? v1 : v2;
      var maxV = v1.Position.X >= v2.Position.X ? v1 : v2;
      var minC = minV.Color;
      var maxC = maxV.Color;

      var m = (maxV.Position.Y - minV.Position.Y) / (maxV.Position.X - minV.Position.X);
      var length = (maxV.Position - minV.Position).Magnitude3();

      for (int x = (int)(minV.Position.X + 0.5); x < (int)(maxV.Position.X + 0.5); x++)
      {
        int y = (int)(m * (x - minV.Position.X) + minV.Position.Y + 0.5);
        var t = (new Vector(x, y, 0) - minV.Position).Magnitude3() / length;
        var color = t * maxC + (1 - t) * minC;
        PaintPixel(x, y, color);
      }
    }

    private void RasterizeLineInY(Vertex v1, Vertex v2)
    {
      var minV = v1.Position.Y < v2.Position.Y ? v1 : v2;
      var maxV = v1.Position.Y >= v2.Position.Y ? v1 : v2;

      var minC = minV.Color;
      var maxC = maxV.Color;

      var m = (maxV.Position.X - minV.Position.X) / (maxV.Position.Y - minV.Position.Y);
      var length = (maxV.Position - minV.Position).Magnitude3();

      for (int y = (int)(minV.Position.Y + 0.5); y < (int)(maxV.Position.Y + 0.5); y++)
      {
        int x = (int)(m * (y - minV.Position.Y) + minV.Position.X + 0.5);
        var t = (new Vector(x, y, 0) - minV.Position).Magnitude3() / length;
        var color = t * maxC + (1 - t) * minC;
        PaintPixel(x, y, color);
      }
    }

    private void RasterizeTriangle(Triangle triangle, bool updateFrameBuffer)
    {
      var xBounds = triangle.GetXBounds();
      var yBounds = triangle.GetYBounds();

      for (var x = Math.Max(0, (int)(xBounds.Item1 + 0.5)); x <= Math.Min((int)(xBounds.Item2 + 0.5), _scene.Width - 1); x++)
      {
        for (var y = Math.Max(0, (int)(yBounds.Item1 + 0.5)); y <= Math.Min((int)(yBounds.Item2 + 0.5), _scene.Height - 1); y++)
        {
          var p = new Vector(x, y, 0);
          var bar = triangle.GetBarycentricCoordinates(p);
          if (_correctPerspective)
            bar = PerspectiveCorrect(triangle, bar);
          if (bar.X >= 0 && bar.X <= 1 && bar.Y >= 0 && bar.Y <= 1 && bar.Z >= 0 && bar.Z <= 1)
          {
            var z = triangle.InterpolateProperty(v => v.Position.Z, bar);
            var zTest = !_enableDepthTest || z > _zBuffer[x, y];
            if (zTest)
            {
              _zBuffer[x, y] = z;
              if (updateFrameBuffer)
              {
                var worldP = triangle.InterpolateProperty(v => v.WorldPosition, bar);
                var shadowP = _enableShadowMapping ? triangle.InterpolateProperty(v => v.ShadowPosition, bar) : new Vector();
                var color = _perPixelShading
                  ? PhongPixelShading(triangle, bar, worldP, shadowP)
                  : GouraudPixelShading(triangle, bar, p, shadowP);
                PaintPixel(x, y, color);
              }
            }
          }
        }
      }
    }

    private Vector GouraudPixelShading(Triangle triangle, Vector bar, Vector p, Vector shadowP)
    {
      var color = triangle.InterpolateProperty(v => v.Color, bar);
      if (_enableShadowMapping)
      {
        var shadowX = (int)(shadowP.X + 0.5);
        var shadowY = (int)(shadowP.Y + 0.5);
        var bias = _enableShadowMappingBias ? 0.05 : 0.0;
        var inShadow = shadowX >= 0 && shadowX < _scene.Width && shadowY >= 0 && shadowY < _scene.Height &&
                       shadowP.Z + bias < _shadowMap[shadowX, shadowY];
        if (inShadow)
        {
          color = _scene.AmbientLight * triangle.InterpolateProperty(v => v.Material.Diffuse, bar);
        }
      }
      var material = triangle.GetMaterial(p);
      if (material.HasDiffuseTexture)
      {
        var texCoords = triangle.InterpolateProperty(v => v.TextureCoordinates, bar);
        var textureColor = material.SampleDiffuseTexture(texCoords);
        color = textureColor*color;
      }
      if (material.HasEnvironmentMap && _enableReflectionMapping)
      {
        var reflectionDirection = triangle.InterpolateProperty(v => v.Normal, bar).Normalize3();
        var mapColor = material.SampleEnvironmentMap(reflectionDirection);
        color = material.ReflectivityAttenuation*mapColor + color;
      }
      return color;
    }

    private Vector PhongPixelShading(Triangle triangle, Vector bar, Vector p, Vector shadowP)
    {
      var normal = triangle.InterpolateProperty(v => v.Normal, bar).Normalize3();
      var material = triangle.GetMaterial(p);
      var color = RenderingParameters.Instance.EnableAmbient ? _scene.AmbientLight * material.Diffuse : new Vector(0, 0, 0);
      foreach (var light in _scene.Lights)
      {
        color += CalculateBlinnPhongIllumination((_scene.Camera.Position - p).Normalize3(),
          (light.Position - p).Normalize3(), light.Color, normal, material);
      }
      if (material.HasEnvironmentMap && _enableReflectionMapping)
      {
        var view = (_scene.Camera.Position - p).Normalize3();
        var r = view*-1 + 2*(Vector.Dot3(view, normal))*normal;
        r.Normalize3();
        var mapColor = material.SampleEnvironmentMap(r);
        color = material.ReflectivityAttenuation * mapColor + color;
      }
      if (_enableShadowMapping)
      {
        var l = (_scene.Lights.First().Position - p).Normalize3();
        var shadowX = (int)(shadowP.X + 0.5);
        var shadowY = (int)(shadowP.Y + 0.5);
        var bias = _enableShadowMappingBias ? 0.1 * Math.Tan(Math.Acos(Math.Max(0, Vector.Dot3(l, normal)))) : 0;
        var inShadow = shadowX >= 0 && shadowX < _scene.Width && shadowY >= 0 && shadowY < _scene.Height &&
                       shadowP.Z + bias < _shadowMap[shadowX, shadowY];
        if (inShadow)
        {
          color = _scene.AmbientLight * triangle.InterpolateProperty(v => v.Material.Diffuse, bar);
        }
      }

      return color;
    }

    private Vector PerspectiveCorrect(Triangle triangle, Vector barycentricCoordinates)
    {
      var d = triangle.Vertices[1].PositionHomogeneus.W * triangle.Vertices[2].PositionHomogeneus.W +
              triangle.Vertices[2].PositionHomogeneus.W * barycentricCoordinates.Y *
              (triangle.Vertices[0].PositionHomogeneus.W - triangle.Vertices[1].PositionHomogeneus.W) +
              triangle.Vertices[1].PositionHomogeneus.W * barycentricCoordinates.Z *
              (triangle.Vertices[0].PositionHomogeneus.W - triangle.Vertices[2].PositionHomogeneus.W);
      var correctedBarycentricCoordinates = new Vector();
      correctedBarycentricCoordinates.Y = triangle.Vertices[0].PositionHomogeneus.W * triangle.Vertices[2].PositionHomogeneus.W *
                                          barycentricCoordinates.Y / d;
      correctedBarycentricCoordinates.Z = triangle.Vertices[0].PositionHomogeneus.W * triangle.Vertices[1].PositionHomogeneus.W *
                                          barycentricCoordinates.Z / d;
      correctedBarycentricCoordinates.X = 1 - correctedBarycentricCoordinates.Y - correctedBarycentricCoordinates.Z;
      return correctedBarycentricCoordinates;
    }
  }
}