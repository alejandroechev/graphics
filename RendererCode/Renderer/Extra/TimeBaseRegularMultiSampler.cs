using System;
using System.Collections.Generic;
using SceneLib;

namespace Renderer
{
  public class TimeBaseRegularMultiSampler : MultiPixelSampler
  {
    private readonly Random _randomizer = new Random();
    private readonly float _minimumTime;
    private readonly float _maximumTime;

    public TimeBaseRegularMultiSampler(int width, int height, int numberOfSamplesPerPixel, float minimumTime, float maximumTime)
      : base(width, height, numberOfSamplesPerPixel)
    {
      _minimumTime = minimumTime;
      _maximumTime = maximumTime;
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
          var randomValueX = (float)(2 * xDelta * _randomizer.NextDouble() - xDelta);
          var randomValueY = (float)(2 * yDelta * _randomizer.NextDouble() - yDelta);
          var time = TriangleSampleTime();
          var sample = new Sample { Color = new Vector(), X = x + randomValueX, Y = y + randomValueY, Time = (float)time };
          _allSamples.Add(sample);
          pixelSamples.Add(sample);
        }
      }
      _allPixelSamples.Add(pixelSamples);
    }

    private double UniformSampleTime()
    {
      var time = _minimumTime + (_maximumTime - _minimumTime) * (_randomizer.NextDouble());
      return time;
    }

    private double TriangleSampleTime()
    {
      var time = _minimumTime + (_maximumTime - _minimumTime) * Math.Sqrt(_randomizer.NextDouble());
      return time;
    }
  }
}