using System;
using System.Linq;
using SceneLib;

namespace Renderer
{
  public class DistributionRaytracer : Raytracer
  {
    private readonly object _syncObject = new object();
    private readonly int _numberOfSamplesPerPixel;

    public override string Name
    {
      get { return "Distribution Raytracer"; }
    }

    public DistributionRaytracer(Scene scene, IDisplay display, int numberOfSamplesPerPixel)
      : base(scene, display)
    {
      _numberOfSamplesPerPixel = numberOfSamplesPerPixel;
    }

    protected override Vector GetSampleColor(float screenX, float screenY)
    {
      var sumOfColor = new Vector();
      var gridSize = (int)Math.Sqrt(_numberOfSamplesPerPixel);
      var deltaSize = 1.0f / (gridSize + 1);
      var initialGridX = (screenX - 0.5f);
      var initialGridY = (screenY - 0.5f);

      if (!_isParallel)
      {
        for (int i = 0; i < gridSize; i++)
        {
          var gridX = initialGridX + (i + 1) * deltaSize;
          for (int j = 0; j < gridSize; j++)
          {
            var gridY = initialGridY + (j + 1) * deltaSize;
            var eyeRay = CreateEyeRay(gridX, gridY);
            sumOfColor += RayTrace(eyeRay, 0).Clamp3();
          }
        }
      }
      else
      {
        Enumerable.Range(0, gridSize * gridSize).ToList().AsParallel().ForAll(pixel =>
        {
          var i = pixel % _scene.Width;
          var j = pixel / _scene.Width;
          var gridX = initialGridX + (i + 1) * deltaSize;
          var gridY = initialGridY + (j + 1) * deltaSize;
          var eyeRay = CreateEyeRay(gridX, gridY);
          lock (_syncObject)
            sumOfColor += RayTrace(eyeRay, 0).Clamp3();

        });
      }
      var averageColor = sumOfColor / (gridSize * gridSize);
      return averageColor;
    }
  }
}