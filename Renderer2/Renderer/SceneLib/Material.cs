using System;
using System.Drawing;

namespace SceneLib
{
  /// <summary>
  /// Represents a material
  /// </summary>
  public class Material
  {
    private static readonly object _myLock = new object();

    public string Name { get; set; }
    internal Bitmap DiffuseTexture { get; set; }

    public bool HasDiffuseTexture
    {
      get { return DiffuseTexture != null; }
    }

    public Vector Diffuse { get; set; }
    public Vector Specular { get; set; }
    public float ReflectivityAttenuation { get; set; }
    public float RefractiveIndex { get; set; }
    public float RefractiveAttenuation { get; set; }
    public float Shininess { get; set; }

    public Material()
    {
      Diffuse = new Vector();
      Specular = new Vector();
    }

    public Material Clone()
    {
      var material = new Material();
      material.DiffuseTexture = DiffuseTexture;
      material.Diffuse = Diffuse.Clone();
      material.Specular = Specular.Clone();
      material.ReflectivityAttenuation = ReflectivityAttenuation;
      material.RefractiveIndex = RefractiveIndex;
      material.RefractiveAttenuation = RefractiveAttenuation;
      material.Shininess = Shininess;
      return material;
    }

    public int GetDiffuseTextureWidth()
    {
      lock (_myLock)
      {
        if (!HasDiffuseTexture)
          return 0;
        return DiffuseTexture.Width;
      }
    }

    public int GetDiffuseTextureHeight()
    {
      lock (_myLock)
      {
        if (!HasDiffuseTexture)
          return 0;
        return DiffuseTexture.Height;
      }
    }

    public Vector GetDiffuseTexturePixel(int i, int j)
    {
      lock (_myLock)
      {
        if (!HasDiffuseTexture)
          return new Vector(1, 1, 1);
        if (i < 0 || i >= DiffuseTexture.Width || j < 0 || j >= DiffuseTexture.Height)
          throw new ArgumentException("Pixel has to be inside texture");
        var pixel = DiffuseTexture.GetPixel(i, j);
        return new Vector(pixel.R / 255.0f, pixel.G / 255.0f, pixel.B / 255.0f);
      }

    }

  }
}