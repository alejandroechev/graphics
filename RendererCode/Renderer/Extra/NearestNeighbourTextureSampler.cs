using System.Drawing;
using SceneLib;

namespace Renderer
{
  public class NearestNeighbourTextureSampler : LockedSampler
  {
    protected override Vector InnerSample2D(Bitmap texture, Vector textureCoords)
    {
      return InnerSample2D(texture, textureCoords, new Vector(), new Vector(texture.Width, texture.Height));
    }

    protected Vector InnerSample2D(Bitmap texture, Vector textureCoords, Vector imageOrigin, Vector imageSize)
    {
      var x = (int)(imageOrigin.X + (imageSize.X - 1) * textureCoords.U);
      var y = (int)(imageOrigin.Y + (imageSize.Y - 1) * textureCoords.V);
      var c = texture.GetPixel(x, y);
      var color = new Vector(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f, c.A / 255.0f);
      return color;
    }

    protected override Vector InnerSampleCube(Bitmap texture, Vector direction)
    {
      throw new System.NotImplementedException();
    }

    protected override Vector InnerSampleNormal(Bitmap texture, Vector textureCoords)
    {
      throw new System.NotImplementedException();
    }
  }
}