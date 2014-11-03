using System;
using System.Collections.Generic;
using SceneLib;

namespace Renderer
{
  public class RegularGridMultiSampler : MultiPixelSampler
  {
    

    public RegularGridMultiSampler(int width, int height, int numberOfSamplesPerPixel)
      : base(width, height, numberOfSamplesPerPixel)
    {
    }

    protected override void GenerateSamplesForPixel(int i, int j, int numberOfSamplesPerPixel)
    {
      var numberOfSampleInX = Math.Sqrt(numberOfSamplesPerPixel);
      var numberOfSampleInY = numberOfSampleInX;

      var xDelta = 1.0f / (float)(numberOfSampleInX + 1);
      var yDelta = 1.0f / (float)(numberOfSampleInY + 1);

      var pixelSamples = new List<Sample>();
      for (float x = i - 0.5f + xDelta; x <= i + 0.5 - xDelta; x += xDelta)
      {
        for (float y = j - 0.5f + yDelta; y <= j + 0.5 - yDelta; y += yDelta)
        {
          var sample = new Sample { Color = new Vector(), X = x, Y = y };
          _allSamples.Add(sample);
          pixelSamples.Add(sample);
        }
      }
      _allPixelSamples.Add(pixelSamples);
    }
  }
}