using System;
using System.Drawing;
using SceneLib;

namespace Renderer
{
  public class BilinearTextureSampler : LockedSampler
  {
    protected override Vector InnerSample2D(Bitmap texture, Vector textureCoords)
    {
      return InnerSample2D(texture, textureCoords, new Vector(), new Vector(texture.Width, texture.Height));
    }

    protected Vector InnerSample2D(Bitmap texture, Vector textureCoords, Vector imageOrigin, Vector imageSize)
    {
      try
      {

        var x = (int)(imageOrigin.X + (imageSize.X - 1) * textureCoords.U);
        var y = (int)(imageOrigin.Y + (imageSize.Y - 1) * textureCoords.V);

        var uPrima = x - (int)x;
        var vPrima = y - (int)y;
        var cij = texture.GetPixel(x, y);
        var ci1j = texture.GetPixel((int)x + 1, (int)y);
        var cij1 = texture.GetPixel((int)x, (int)y + 1);
        var ci1j1 = texture.GetPixel((int)x + 1, (int)y + 1);
        var R = (1 - uPrima) * (1 - vPrima) * cij.R +
                  uPrima * (1 - vPrima) * ci1j.R +
                  (1 - uPrima) * vPrima * cij1.R +
                  uPrima * vPrima * ci1j1.R;
        var G = (1 - uPrima) * (1 - vPrima) * cij.G +
                  uPrima * (1 - vPrima) * ci1j.G +
                  (1 - uPrima) * vPrima * cij1.G +
                  uPrima * vPrima * ci1j1.G;
        var B = (1 - uPrima) * (1 - vPrima) * cij.B +
                  uPrima * (1 - vPrima) * ci1j.B +
                  (1 - uPrima) * vPrima * cij1.B +
                  uPrima * vPrima * ci1j1.B;
        return new Vector(R / 255.0f, G / 255.0f, B / 255.0f, 1.0f);
      }
      catch (Exception)
      {

        return new Vector();
      }

    }

    protected override Vector InnerSampleCube(Bitmap texture, Vector direction)
    {
      var face = GetFace(direction);
      var texCoords = GetTextureCoordinates(face, direction);
      var bounds = GetFaceBounds(texture, face);
      return InnerSample2D(texture, texCoords, bounds.Item1, bounds.Item2);
    }

    protected override Vector InnerSampleNormal(Bitmap texture, Vector textureCoords)
    {
      var x = (int)((texture.Width - 1) * textureCoords.U);
      var y = (int)((texture.Height - 1) * textureCoords.V);
      var c = texture.GetPixel(x, y);
      var color = new Vector(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f, c.A / 255.0f);
      var normal = 2*color - new Vector(1, 1, 1);

      return normal.Normalize3();
    }

    private CubeMapFaces GetFace(Vector direction)
    {

      if (Math.Abs(direction.X) >= Math.Abs(direction.Y) && Math.Abs(direction.X) >= Math.Abs(direction.Z))
      {
        if (direction.X > 0)
          return CubeMapFaces.PositiveX;
        else
          return CubeMapFaces.NegativeX;
      }
      if (Math.Abs(direction.Y) >= Math.Abs(direction.X) && Math.Abs(direction.Y) >= Math.Abs(direction.Z))
      {
        if (direction.Y > 0)
          return CubeMapFaces.PositiveY;
        else
          return CubeMapFaces.NegativeY;
      }
      if (Math.Abs(direction.Z) >= Math.Abs(direction.X) && Math.Abs(direction.Z) >= Math.Abs(direction.Y))
      {
        if (direction.Z > 0)
          return CubeMapFaces.PositiveZ;
        else
          return CubeMapFaces.NegativeZ;
      }
      throw new ApplicationException("shouldnt be here");
    }

    private Vector GetTextureCoordinates(CubeMapFaces face, Vector direction)
    {
      switch (face)
      {
        case CubeMapFaces.PositiveY:
          return new Vector(1.0f - (direction.Y + direction.X) / (2 * direction.Y), (direction.Y + direction.Z) / (2 * direction.Y));
        case CubeMapFaces.NegativeY:
          return new Vector((Math.Abs(direction.Y) + direction.X) / (2 * Math.Abs(direction.Y)), (Math.Abs(direction.Y) + direction.Z) / (2 * Math.Abs(direction.Y)));
        case CubeMapFaces.PositiveZ:
          return new Vector((direction.Z + direction.X) / (2 * direction.Z), 1.0f - (direction.Z + direction.Y) / (2 * direction.Z));
        case CubeMapFaces.NegativeZ:
          return new Vector((Math.Abs(direction.Z) + direction.X) / (2 * Math.Abs(direction.Z)), (Math.Abs(direction.Z) + direction.Y) / (2 * Math.Abs(direction.Z)));
        case CubeMapFaces.PositiveX:
          return new Vector((direction.X + direction.Y) / (2* direction.X), (direction.X + direction.Z) / (2* direction.X)) ;
        case CubeMapFaces.NegativeX:
          return new Vector(1.0f - (Math.Abs(direction.X) + direction.Y) / (2 * Math.Abs(direction.X)), (Math.Abs(direction.X) + direction.Z) / 2 * Math.Abs(direction.X));
        default:
          throw new ArgumentOutOfRangeException("face");
      }
    }

    private Tuple<Vector, Vector> GetFaceBounds(Bitmap texture, CubeMapFaces face)
    {
      var faceWidth = texture.Width / 4;
      var faceHeight = texture.Height / 3;

      switch (face)
      {
        case CubeMapFaces.PositiveY:
          return new Tuple<Vector, Vector>(new Vector(faceWidth, 2 * faceHeight), new Vector(faceWidth, faceHeight));
        case CubeMapFaces.NegativeY:
          return new Tuple<Vector, Vector>(new Vector(faceWidth, 0), new Vector(faceWidth, faceHeight));
        case CubeMapFaces.PositiveZ:
          return new Tuple<Vector, Vector>(new Vector(faceWidth, faceHeight), new Vector(faceWidth, faceHeight));
        case CubeMapFaces.NegativeZ:
          return new Tuple<Vector, Vector>(new Vector(3 * faceWidth, faceHeight), new Vector(faceWidth, faceHeight));
        case CubeMapFaces.PositiveX:
          return new Tuple<Vector, Vector>(new Vector(2 * faceWidth, faceHeight), new Vector(faceWidth, faceHeight));
        case CubeMapFaces.NegativeX:
          return new Tuple<Vector, Vector>(new Vector(0, faceHeight), new Vector(faceWidth, faceHeight));
        default:
          throw new ArgumentOutOfRangeException("face");
      }
    }
  }
}