using System;
using System.Linq;
using SceneLib;

namespace Renderer
{
  public class DistributionRaytracer : Raytracer
  {
    private readonly object _syncObject = new object();
    private readonly int _numberOfSamplesPerPixel;
    private Random _randomizer = new Random(0);

    public override string Name
    {
      get { return "Distribution Raytracer"; }
    }

    public DistributionRaytracer(Scene scene, IDisplay display, int numberOfSamplesPerPixel)
      : base(scene, display)
    {
      _numberOfSamplesPerPixel = numberOfSamplesPerPixel;
    }

    protected override Ray CreateEyeRay(float screenX, float screenY)
    {
      var ray = base.CreateEyeRay(screenX, screenY);
      var lensSize = 2.0f;
      ray.Position += new Vector((float)(_randomizer.NextDouble() - 1) * lensSize * 2, (float)(_randomizer.NextDouble() - 1) * lensSize * 2, 0);
      return ray;
    }

    protected override Vector GetLightPosition(Light light)
    {
      return light.Position +
             new Vector((float)(_randomizer.NextDouble() - 1) * light.Size * 2, 0, (float)(_randomizer.NextDouble() - 1) * light.Size * 2);
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
        float time = 0.0f;
        float deltaTime = 1.0f / _numberOfSamplesPerPixel;
        for (int i = 0; i < gridSize; i++)
        {
          var gridX = initialGridX + (i + 1) * deltaSize + deltaSize * (float)(_randomizer.NextDouble() - 1);
          for (int j = 0; j < gridSize; j++)
          {
            var gridY = initialGridY + (j + 1) * deltaSize + deltaSize * (float)(_randomizer.NextDouble() - 1);
            var eyeRay = CreateEyeRay(gridX, gridY);
            time += deltaTime;
            eyeRay.Time = time;
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
          eyeRay.Time = (float)pixel / (float)(gridSize * gridSize);
          lock (_syncObject)
            sumOfColor += RayTrace(eyeRay, 0).Clamp3();

        });
      }
      var averageColor = sumOfColor / (gridSize * gridSize);
      return averageColor;
    }
  }
}