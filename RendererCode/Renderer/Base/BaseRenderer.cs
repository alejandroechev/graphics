using System;
using OpenTK;
using SceneLib;

namespace Renderer.Base
{
  public abstract class BaseRenderer
  {
    #region Fields

    protected Scene _scene;
    private readonly Vector[,] _image;

    #endregion

    #region Properties

    public Vector[,] Image
    {
      get { return _image; }
    }

    #endregion

    #region Constructor

    protected BaseRenderer(Scene scene)
    {
      if (scene == null) throw new ArgumentNullException("scene");
      _scene = scene;
      _image = new Vector[_scene.Width, _scene.Height];
      CleanBuffer();
    }

    #endregion

    #region Public Methods

    public abstract void Render();

    public virtual void Update(double time)
    {
      
    }

    public abstract void OnKeyPress(KeyPressEventArgs e);
    #endregion

    #region Private Methods

    protected void CleanBuffer()
    {
      for (int i = 0; i < _scene.Width; i++)
      {
        for (int j = 0; j < _scene.Height; j++)
        {
          _image[i, j] = _scene.BackgroundColor;
        }
      }
    }

    protected void PaintPixel(int x, int y, Vector color)
    {
      if (x < 0 || x >= _scene.Width) return;
      if (y < 0 || y >= _scene.Height) return;

      _image[x, y] = color;
    }

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

    protected Matrix ModelToWorld(RenderObject model)
    {
      var scaling = MatrixExtensions.Scaling(model.Scale);
      var rotation = MatrixExtensions.Rotation(model.Rotation);
      var translation = MatrixExtensions.Translation(model.Position);
      return translation * rotation * scaling;
    }

    
    #endregion

  }
}