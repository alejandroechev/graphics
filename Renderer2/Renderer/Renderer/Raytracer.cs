﻿using System;
using System.Linq;
using SceneLib;

namespace Renderer
{
  public class Raytracer : AbstractRenderer
  {
    public override string Name
    {
      get { return "Raytracer"; }
    }


    public Raytracer(Scene scene, IDisplay display)
      : base(scene, display)
    {

    }

    public override void Render()
    {
      if (!_isParallel)
      {
        for (int i = 0; i < _scene.Width; i++)
        {
          for (int j = 0; j < _scene.Height; j++)
          {
            var color = GetSampleColor(i, j);
            _display.SetPixel(i, j, color.R, color.G, color.B);
          }
        }
      }
      else
      {
        Enumerable.Range(0, _scene.Width*_scene.Height).ToList().AsParallel().ForAll(pixel =>
        {
          var i = pixel%_scene.Width;
          var j = pixel/_scene.Width;
          var color = GetSampleColor(i, j);
          _display.SetPixel(i, j, color.R, color.G, color.B);
        });
      }
    }

    protected virtual Vector GetSampleColor(float screenX, float screenY)
    {
      var eyeRay = CreateEyeRay(screenX, screenY, _scene.Camera.Position);
      return RayTrace(eyeRay, 0).Clamp3();
    }

    protected virtual Ray CreateEyeRay(float screenX, float screenY, Vector cameraPosition)
    {
      var sceneCamera = _scene.Camera;
      var pixelCoords = sceneCamera.PixelToCameraCoordinates(screenX, screenY, _scene.Width, _scene.Height);
      var coordinateBasis = sceneCamera.GetCameraCoordinatesBasis();
      var pixelWorldCoords = sceneCamera.Position + coordinateBasis[0] * (pixelCoords.X) + coordinateBasis[1] * (pixelCoords.Y) +
                             coordinateBasis[2] * (pixelCoords.Z);
      var direction = pixelWorldCoords - cameraPosition;
      direction = direction.Normalize3();
      var eyeRay = new Ray(cameraPosition, direction); // eye rays
      return eyeRay;
    }


    protected Vector RayTrace(Ray ray, int recursion)
    {
      IntersectObjects(ray);
      if (ray.IntersectedObject == null) return _scene.BackgroundColor;
      return Shade(ray, recursion);
    }

    private Vector Shade(Ray ray, int recursion)
    {
      var intersectedDitance = ray.IntersectionDistance;
      var intersectedObject = ray.IntersectedObject;

      var intersectionPoint = ray.Position + (ray.Direction * intersectedDitance);
      var normal = intersectedObject.GetNormal(ray);
      var material = intersectedObject.GetMaterial(ray);

      var shadingColor = _scene.AmbientLight * material.Diffuse;
      shadingColor = DirectIllumination(ray, material, normal, intersectionPoint, shadingColor);
      shadingColor = Reflect(ray, recursion, material, normal, intersectionPoint, shadingColor);
      shadingColor = Refract(ray, recursion, material, normal, intersectionPoint, shadingColor);
      return shadingColor;
    }

    protected virtual Vector GetLightPosition(Light light)
    {
      return light.Position;
    }

    private Vector DirectIllumination(Ray ray, Material material, Vector normal, Vector intersectionPoint, Vector shadingColor)
    {
      const float epsilon = 0.1f;

      foreach (var light in _scene.Lights)
      {
        var viewDirection = ray.Direction * -1;
        var lightPosition = light.Position;
        var lightDirection = (lightPosition - intersectionPoint).Normalize3();
        var lightDistance = (lightPosition - intersectionPoint).Magnitude3();

        var shadowRay = CreateShadowRay(intersectionPoint, light, epsilon);
        shadowRay.Time = ray.Time;
        IntersectObjects(shadowRay);

        if (shadowRay.IntersectedObject == null || shadowRay.IntersectionDistance >= lightDistance)
          shadingColor += CalculateBlinnPhongIllumination(viewDirection, lightDirection, light.Color, normal, material);
      }
      return shadingColor;
    }

    private Vector Refract(Ray ray, int recursion, Material material, Vector normal, Vector intersectionPoint,
      Vector shadingColor)
    {
      if (material.RefractiveIndex > 0 && recursion < RenderingParameters.Instance.NumberOfRecursiveRays)
      {
        const float epsilon = 1f;
        var reflectionDirection = GetReflectionDirection(ray, normal, material);
        var reflectedRay = new Ray(intersectionPoint + reflectionDirection * epsilon, reflectionDirection);
        reflectedRay.Time = ray.Time;
        var dDotN = Vector.Dot3(ray.Direction, normal);
        var nt = material.RefractiveIndex;
        Vector refractionDirection;
        var cosine = 0.0f;
        if (dDotN < 0)
        {
          refractionDirection = GerRefractionDirection(ray, normal, nt);
          cosine = -dDotN;
        }
        else
        {
          refractionDirection = GerRefractionDirection(ray, -1.0f * normal, 1.0f / nt);
          if (refractionDirection != null)
          {
            cosine = Vector.Dot3(refractionDirection, normal);
          }
        }
        if (refractionDirection == null)
        {
          shadingColor += material.RefractiveAttenuation * RayTrace(reflectedRay, recursion + 1);
        }
        else
        {
          var r0 = ((nt - 1) * (nt - 1)) / ((nt + 1) * (nt + 1));
          var r = r0 + (1 - r0) * (float)Math.Pow(1 - cosine, 5);
          var refractedRay = new Ray(intersectionPoint + refractionDirection * epsilon, refractionDirection);
          refractedRay.Time = ray.Time;
          shadingColor += material.RefractiveAttenuation *
                          (r * RayTrace(reflectedRay, recursion + 1) + (1 - r) * RayTrace(refractedRay, recursion + 1));
        }
      }
      return shadingColor;
    }

    private Vector Reflect(Ray ray, int recursion, Material material, Vector normal, Vector intersectionPoint,
      Vector shadingColor)
    {
      if (material.ReflectivityAttenuation > 0 && recursion < RenderingParameters.Instance.NumberOfRecursiveRays)
      {
        const float epsilon = 1f;
        var reflectionDirection = GetReflectionDirection(ray, normal, material);
        var reflectedRay = new Ray(intersectionPoint + reflectionDirection * epsilon, reflectionDirection);
        reflectedRay.Time = ray.Time;
        shadingColor += material.ReflectivityAttenuation * RayTrace(reflectedRay, recursion + 1);
      }
      return shadingColor;
    }

    protected virtual Vector GetReflectionDirection(Ray ray, Vector normal, Material material)
    {
      var reflectionDirection = (ray.Direction - normal * (Vector.Dot3(ray.Direction, normal) * 2)).Normalize3();
      return reflectionDirection;
    }

    private Vector GerRefractionDirection(Ray ray, Vector normal, float nt)
    {
      var dDotN = Vector.Dot3(ray.Direction, normal);
      var sqrtValue = 1 - (1 - dDotN * dDotN) / (nt * nt);
      if (sqrtValue < 0)
        return null;
      var d = (ray.Direction - normal * dDotN) / nt -
                            normal * (float)Math.Sqrt(sqrtValue);
      return d.Normalize3();
    }

    private Ray CreateShadowRay(Vector intersectionPoint, Light light, float epsilon)
    {
      var lightPosition = GetLightPosition(light);
      var lightDirection = (lightPosition - intersectionPoint).Normalize3();
      var shadowRay = new Ray(intersectionPoint + lightDirection * epsilon, lightDirection);
      return shadowRay;
    }

    private void IntersectObjects(Ray ray)
    {
      foreach (var sceneObject in _scene.Objects)
      {
        sceneObject.Intersect(ray);
      }
    }


  }
}