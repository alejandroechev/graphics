using System;
using SceneLib;

namespace Renderer
{
  public class BilinearSampler : ITextureSampler
  {
    public Vector Sample(Vector textureCoords, Func<int, int, Vector> getTexturePixel, Func<int> getTextureWidth, Func<int> getTextureHeight)
    {
      var i = textureCoords.X * (getTextureWidth() - 1);
      var intI = (int)i;
      var deltaI = i - intI;
      var j = textureCoords.Y * (getTextureHeight() - 1);
      var intJ = (int)j;
      var deltaJ = j - intJ;

      var pixelUV = getTexturePixel(intI, intJ);
      var pixelU1V = getTexturePixel(intI + 1, intJ);
      var pixelUV1 = getTexturePixel(intI, intJ + 1);
      var pixelU1V1 = getTexturePixel(intI + 1, intJ + 1);

      var colorA = deltaI * pixelU1V + (1 - deltaI) * pixelUV;
      var colorB = deltaI * pixelU1V1 + (1 - deltaI) * pixelUV1;

      var colorC = deltaJ * colorB + (1 - deltaJ) * colorA;

      return colorC;
    }
  }
}