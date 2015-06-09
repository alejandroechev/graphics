namespace SceneLib
{
  public interface IPixelSamplerFactory
  {
    IPixelSampler CreateSampler(int width, int height, int numberOfPixelSamples, float minimumTume, float maximumTime);
    
  }
}