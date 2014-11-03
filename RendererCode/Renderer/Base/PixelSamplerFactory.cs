using SceneLib;

namespace Renderer.Base
{
  public class PixelSamplerFactory : IPixelSamplerFactory
  {
    public IPixelSampler CreateSampler(int width, int height, int numberOfPixelSamples, float minimumTime, float maximumTime)
    {
      IPixelSampler sampler;
      if (numberOfPixelSamples == 1)
        sampler = CreateSimpleSampler(width, height);
      else
        sampler = CreateTimeBasedRegularMultiSampler(width, height, numberOfPixelSamples, minimumTime, maximumTime);
      sampler.Initialize();
      return sampler;
    }

    public IPixelSampler CreateSimpleSampler(int width, int height)
    {
      var sampler = new SimpleSampler(width, height);
      sampler.Initialize();
      return sampler;
    }

    public IPixelSampler CreateRegularMultiSampler(int width, int height, int numberOfPixelSamples)
    {
      var sampler = new RegularGridMultiSampler(width, height, numberOfPixelSamples);
      sampler.Initialize();
      return sampler;
    }

    public IPixelSampler CreateTimeBasedRegularMultiSampler(int width, int height, int numberOfPixelSamples, float minimumTime, float maximumTime)
    {
      var sampler = new TimeBaseRegularMultiSampler(width, height, numberOfPixelSamples, minimumTime, maximumTime);
      sampler.Initialize();
      return sampler;
    }

    public IPixelSampler CreateJitteredMultiSampler(int width, int height, int numberOfPixelSamples)
    {
      var sampler = new JitteredMultiSampler(width, height, numberOfPixelSamples);
      sampler.Initialize();
      return sampler;
    }
  }
}