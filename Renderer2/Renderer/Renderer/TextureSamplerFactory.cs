using SceneLib;

namespace Renderer
{
  public class TextureSamplerFactory : ITextureSamplerFactory
  {
    public ITextureSampler CreateTextureSampler(bool multiSample)
    {
      return new NearestNeighbourTextureSampler();
    }
  }
}
