using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using SceneLib;

namespace Renderer.Base
{
  public static class SceneLibExtensions
  {
    public static Vector3 ToVector3(this Vector vector)
    {
      return new Vector3(vector.X, vector.Y, vector.Z);
    }

    public static Color4 ToColor4(this Vector vector)
    {
      return new Color4(vector.R, vector.G, vector.B, vector.A);
    }

    public static List<Vector3> PositionsToVector3List(this PolygonBase polygon)
    {
      var vectorList = new List<Vector3>();
      foreach (var vertex in polygon.Vertices)
      {
        vectorList.Add(vertex.Position.ToVector3());
      }
      return vectorList;
    }

    public static List<Vector3> DiffuseColorsToVector3List(this PolygonBase polygon)
    {
      var vectorList = new List<Vector3>();
      foreach (var vertex in polygon.Vertices)
      {
        vectorList.Add(vertex.Material.Diffuse.ToVector3());
      }
      return vectorList;
    }

    public static List<Vector3> NormalsToVector3List(this PolygonBase polygon)
    {
      var vectorList = new List<Vector3>();
      foreach (var vertex in polygon.Vertices)
      {
        vectorList.Add(vertex.Normal.ToVector3());
      }
      return vectorList;
    }

    public static List<Vector3> SpecularColorsToVector3List(this PolygonBase polygon)
    {
      var vectorList = new List<Vector3>();
      foreach (var vertex in polygon.Vertices)
      {
        vectorList.Add(vertex.Material.Specular.ToVector3());
      }
      return vectorList;
    }

    public static List<Vector3> TextureCoordinatesToVector3List(this PolygonBase polygon)
    {
      var vectorList = new List<Vector3>();
      foreach (var vertex in polygon.Vertices)
      {
        var texCoords = vertex.TextureCoordinates;
        texCoords.Z = vertex.Material.Shininess;
        vectorList.Add(texCoords.ToVector3());
      }
      return vectorList;
    }

    public static float ToRadians(this float angle)
    {
      return (float) (Math.PI*(angle/180.0f));
    }
  }
}
