using System.Collections.Generic;
using SceneLib;

namespace Renderer
{
  public abstract class MultiPixelSampler : IPixelSampler
  {
    protected readonly List<Sample> _allSamples = new List<Sample>();
    protected readonly List<List<Sample>> _allPixelSamples = new List<List<Sample>>();

    private readonly int _width;
    private readonly int _height;
    private readonly int _numberOfSamplesPerPixel;

    public IEnumerable<Sample> Samples
    {
      get
      {
        return _allSamples;
      }
    }

    protected MultiPixelSampler(int width, int height, int numberOfSamplesPerPixel)
    {
      _width = width;
      _height = height;
      _numberOfSamplesPerPixel = numberOfSamplesPerPixel;
    }

    public void Initialize()
    {
      for (int i = 0; i < _width; i++)
      {
        for (int j = 0; j < _height; j++)
        {
          GenerateSamplesForPixel(i, j, _numberOfSamplesPerPixel);
        }
      }
    }

    public Vector Integrate(int i, int j)
    {
      var pixelSamples = _allPixelSamples[i * _height + j];
      var integratedColor = new Vector();
      foreach (var pixelSample in pixelSamples)
      {
        integratedColor = integratedColor + pixelSample.Color;
      }
      return integratedColor / pixelSamples.Count;

    }

    protected abstract void GenerateSamplesForPixel(int i, int j, int numberOfSamplesPerPixel);
  }
}