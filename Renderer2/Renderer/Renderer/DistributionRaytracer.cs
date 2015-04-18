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
    private float[,] _gaussianKernel;

    public override string Name
    {
      get { return "Distribution Raytracer"; }
    }

    public DistributionRaytracer(Scene scene, IDisplay display, int numberOfSamplesPerPixel)
      : base(scene, display)
    {
      _numberOfSamplesPerPixel = numberOfSamplesPerPixel;
      var gridSize = (int)Math.Sqrt(_numberOfSamplesPerPixel);
      _gaussianKernel = GetGaussianKernel(gridSize);
    }

    protected override Ray CreateEyeRay(float screenX, float screenY, Vector cameraPosition)
    {
      var lensSize = _scene.Camera.LensSize;
      cameraPosition = cameraPosition + new Vector((float)(_randomizer.NextDouble() - 0.5) * lensSize * 2, (float)(_randomizer.NextDouble() - 0.5) * lensSize * 2, 0);
      var ray = base.CreateEyeRay(screenX, screenY, cameraPosition);
      return ray;
    }

    protected override Vector GetLightPosition(Light light)
    {
      var randomX = (float)(2.0f * (_randomizer.NextDouble() - 0.5));
      var randomY = (float)(2.0f * (_randomizer.NextDouble() - 0.5));
      var newLightPosition = light.Position +
             new Vector(randomX * light.Size, 0, randomY * light.Size);
      return newLightPosition;
    }

    protected override Vector GetReflectionDirection(Ray ray, Vector normal, Material material)
    {
      var baseRef = base.GetReflectionDirection(ray, normal, material);
      var t = new Vector(baseRef.X, baseRef.Y, baseRef.Z);
      if (t.X < t.Y && t.X < t.Z)
        t.X = 1;
      else if (t.Y < t.X && t.Y < t.Z)
        t.Y = 1;
      else
        t.Z = 1;
      t = t.Normalize3();
      var u = Vector.Cross3(baseRef, t).Normalize3();
      var v = Vector.Cross3(baseRef, u).Normalize3();

      var randomX = (float)(2.0f * (_randomizer.NextDouble() - 0.5));
      var randomY = (float)(2.0f * (_randomizer.NextDouble() - 0.5));

      var newRef = baseRef + randomX * material.Glossy * u + randomY * material.Glossy * v;
      return newRef;
    }

    protected float[,] GetGaussianKernel(int size)
    {
      var sigma = 1.0f;
      var mean = size / 2.0f;
      var kernel = new float[size, size];
      var sum = 0.0f;

      for (int i = 0; i < size; i++)
      {
        for (int j = 0; j < size; j++)
        {
          kernel[i, j] = (float)
            Math.Exp(-0.5 * (Math.Pow((i - mean) / sigma, 2) + Math.Pow((j - mean) / sigma, 2)) / (2 * Math.PI * sigma * sigma));
          sum += kernel[i, j];
        }
      }

      for (int i = 0; i < size; i++)
      {
        for (int j = 0; j < size; j++)
        {
          kernel[i, j] /= sum;
        }
      }
      return kernel;
    }

    protected override Vector GetSampleColor(float screenX, float screenY)
    {
      var sumOfColor = new Vector();
      var gridSize = (int)Math.Sqrt(_numberOfSamplesPerPixel);
      var deltaSize = 1.0f / (gridSize + 1);
      var initialGridX = (screenX - 0.5f);
      var initialGridY = (screenY - 0.5f);
      var gaussianKernel = _gaussianKernel;
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
            var eyeRay = CreateEyeRay(gridX, gridY, _scene.Camera.Position);
            time += deltaTime;
            eyeRay.Time = time;
            sumOfColor += gaussianKernel[i, j] * RayTrace(eyeRay, 0).Clamp3();
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
          var eyeRay = CreateEyeRay(gridX, gridY, _scene.Camera.Position);
          eyeRay.Time = (float)pixel / (float)(gridSize * gridSize);
          lock (_syncObject)
            sumOfColor += gaussianKernel[i, j] * RayTrace(eyeRay, 0).Clamp3();

        });
      }
      //var averageColor = sumOfColor / (gridSize * gridSize);
      return sumOfColor;
    }
  }
}