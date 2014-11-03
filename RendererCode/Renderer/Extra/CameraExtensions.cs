using System;
using System.Collections.Generic;
using SceneLib;

namespace Renderer.Base
{
  public enum Bounds
  {
    Top,
    Bottom,
    Left,
    Right
  }
  public static class CameraExtensions
  {
    public static Dictionary<Bounds, float> FrustumBounds(this Camera camera, int width, int height)
    {
      var bounds = new Dictionary<Bounds, float>();
      var t = (float)(Math.Abs(camera.NearClip) * Math.Tan(((camera.FieldOfView / 2) / 180.0) * Math.PI));
      var b = -t;
      var r = t * (float)width / (float)height;
      var l = -r;

      bounds.Add(Bounds.Top, t);
      bounds.Add(Bounds.Bottom, b);
      bounds.Add(Bounds.Right, r);
      bounds.Add(Bounds.Left, l);

      return bounds;
    }

    public static Vector PixelToCameraCoordinates(this Camera camera, float x, float y, int width, int height)
    {
      var t = (float)(Math.Abs(camera.NearClip) * Math.Tan(((camera.FieldOfView / 2) / 180.0) * Math.PI));
      var b = -t;
      var r = t * (float)width / (float)height;
      var l = -r;

      var u = l + (r - l) * (x + 0.5f) / width;
      var v = b + (t - b) * (y + 0.5f) / height;
      var w = camera.NearClip;
      var pixelCoords = new Vector(u, v, -w);

      return pixelCoords;
    }

    public static List<Vector> GetCameraCoordinatesBasis(this Camera camera)
    {
      var coordinates = new List<Vector>();
      var w = camera.Target - camera.Position;
      w.Normalize3();
      w *= -1;

      var u = Vector.Cross3(camera.Up, w);
      u.Normalize3();

      var v = Vector.Cross3(w, u);
      v.Normalize3();

      coordinates.Add(u);
      coordinates.Add(v);
      coordinates.Add(w);

      return coordinates;

    }
  }
}
