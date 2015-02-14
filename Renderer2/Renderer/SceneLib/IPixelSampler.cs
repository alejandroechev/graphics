using System.Collections.Generic;

namespace SceneLib
{
  public interface IPixelSampler
  {
    IEnumerable<Sample> Samples { get; }
    void Initialize();
    Vector Integrate(int i, int j);
  }
}