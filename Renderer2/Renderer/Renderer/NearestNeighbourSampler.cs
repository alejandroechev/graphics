using System;
using SceneLib;

namespace Renderer
{
  public class NearestNeighbourSampler : ITextureSampler
  {
    public Vector Sample(Vector textureCoords, Func<int, int, Vector> getTexturePixel, Func<int> getTextureWidth, Func<int> getTextureHeight)
    {
      return getTexturePixel((int) (textureCoords.X*(getTextureWidth() - 1) + 0.5),
        (int)(textureCoords.Y * (getTextureHeight() - 1) + 0.5));
    }
  }
}