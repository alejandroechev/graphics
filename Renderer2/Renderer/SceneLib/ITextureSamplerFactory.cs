namespace SceneLib
{
  public interface ITextureSamplerFactory
  {
    ITextureSampler CreateTextureSampler(bool multiSample);
  }
}
