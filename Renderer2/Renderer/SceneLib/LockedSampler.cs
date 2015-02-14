using System.Drawing;

namespace SceneLib
{
  public abstract class LockedSampler : ITextureSampler
  {
    private static readonly object _lockObject = new object();

    public Vector SampleNormal(Bitmap texture, Vector textureCoords)
    {
      lock (_lockObject)
      {
        return InnerSampleNormal(texture, textureCoords);
      }
    }

    public Vector Sample2D(Bitmap texture, Vector textureCoords)
    {
      lock (_lockObject)
      {
        return InnerSample2D(texture, textureCoords);
      }
    }

    public Vector SampleCube(Bitmap texture, Vector direction)
    {
      lock (_lockObject)
      {
        return InnerSampleCube(texture, direction);
      }
    }

    protected abstract Vector InnerSample2D(Bitmap texture, Vector textureCoords);
    protected abstract Vector InnerSampleCube(Bitmap texture, Vector direction);
    protected abstract Vector InnerSampleNormal(Bitmap texture, Vector textureCoords);
  }
}