using System;
using SceneLib;

namespace Renderer
{
  public abstract class AbstractRenderer : IRender
  {
    protected readonly Scene _scene;
    protected readonly IDisplay _display;

    protected AbstractRenderer(Scene scene, IDisplay display)
    {
      _scene = scene;
      _display = display;
    }

    public abstract void Render();

    protected Vector CalculateBlinnPhongIllumination(Vector viewDirection, Vector lightDirection, Vector lightColor, Vector normal, Material material)
    {
      var halfDirection = (viewDirection + lightDirection).Normalize3();
      var specular = RenderingParameters.Instance.EnableSpecular
        ? material.Specular * lightColor *
          (float)Math.Pow(Math.Max(0, Vector.Dot3(normal, halfDirection)), material.Shininess)
        : new Vector(0, 0, 0);
      var diffuse = material.Diffuse * lightColor * Math.Max(0, Vector.Dot3(normal, lightDirection));
      return diffuse + specular;
    }
  }
}