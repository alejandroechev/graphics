using SceneLib;

namespace Renderer
{
  public class PixelSamplerFactory : IPixelSamplerFactory
  {

    public IPixelSampler CreateSampler(int width, int height)
    {
      var sampler = new SimpleSampler(width, height);
      sampler.Initialize();
      return sampler;
    }

   
  }
}