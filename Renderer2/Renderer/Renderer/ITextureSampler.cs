using System;
using SceneLib;

namespace Renderer
{
  public interface ITextureSampler
  {
    Vector Sample(Vector textureCoords, Func<int, int, Vector> getTexturePixel, Func<int> getTextureWidth, Func<int> getTextureHeight);
  }
}