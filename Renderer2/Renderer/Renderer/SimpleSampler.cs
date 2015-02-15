using System.Collections.Generic;
using SceneLib;

namespace Renderer
{
  public class SimpleSampler : IPixelSampler
  {
    private readonly List<Sample> _samples = new List<Sample>();
    private readonly int _width;
    private readonly int _height;

    public IEnumerable<Sample> Samples 
    {
      get
      {
        return _samples;
      }
    }

    public SimpleSampler(int width, int height)
    {
      _width = width;
      _height = height;
      
    }

    public void Initialize()
    {
      for (int i = 0; i < _width; i++)
      {
        for (int j = 0; j < _height; j++)
        {
          _samples.Add(new Sample { Color = new Vector(), X = i, Y = j });
        }
      }
    }

    public Vector Integrate(int i, int j)
    {
      return _samples[i * _height + j].Color;
    }
  }
}