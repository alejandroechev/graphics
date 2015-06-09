using System.Drawing;

namespace SceneLib
{
  public enum CubeMapFaces
  {
    PositiveY,
    NegativeY,
    PositiveZ,
    NegativeZ,
    PositiveX,
    NegativeX
  }

  public interface ITextureSampler
  {
    Vector SampleNormal(Bitmap texture, Vector textureCoords);
    Vector Sample2D(Bitmap texture, Vector textureCoords);
    Vector SampleCube(Bitmap texture, Vector direction);
  }
}