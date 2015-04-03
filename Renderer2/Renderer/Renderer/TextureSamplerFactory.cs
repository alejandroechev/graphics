namespace Renderer
{
  public class TextureSamplerFactory
  {
    public ITextureSampler CreateNearestNeighbourSampler()
    {
      return new NearestNeighbourSampler();
    }

    public ITextureSampler CreateBilinearSampler()
    {
      return new BilinearSampler();
    }
  }
}