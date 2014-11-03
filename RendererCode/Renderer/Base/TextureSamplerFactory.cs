using SceneLib;

namespace Renderer.Base
{
  public class TextureSamplerFactory : ITextureSamplerFactory
  {
    public ITextureSampler CreateTextureSampler(bool multiSample)
    {
      if(multiSample)
        return new BilinearTextureSampler();
      return new NearestNeighbourTextureSampler();
    }
  }
}
