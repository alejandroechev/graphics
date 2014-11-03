using System;
using System.Linq;
using OpenTK;
using OpenTK.Input;
using SceneLib;

namespace Renderer.Base
{
  public abstract class BaseOpenGLManager
  {
    protected Scene _scene;

    protected BaseOpenGLManager(Scene scene)
    {
      if (scene == null) throw new ArgumentNullException("scene");
      _scene = scene;
    }

    public abstract void Load();
    public abstract void Render(double time);
    public abstract void Update(double time);

    public virtual void OnKeyPress(KeyPressEventArgs e)
    {
      float cameraSpeed = (float)(_scene.Camera.FarClip - _scene.Camera.NearClip)/50.0f;
      if (e.KeyChar == 'w')
        _scene.Camera.MoveForward(cameraSpeed);
      else if (e.KeyChar == 's')
        _scene.Camera.MoveForward(-cameraSpeed);
      else if (e.KeyChar == 'd')
        _scene.Camera.MoveSideways(cameraSpeed);
      else if (e.KeyChar == 'a')
        _scene.Camera.MoveSideways(-cameraSpeed);
      if (_scene.Lights.Any())
      {
        if (e.KeyChar == 'i')
          _scene.Lights.First().Position += cameraSpeed * new Vector(0, 0, 1);
        else if (e.KeyChar == 'k')
          _scene.Lights.First().Position -= cameraSpeed * new Vector(0, 0, 1);
        else if (e.KeyChar == 'l')
          _scene.Lights.First().Position -= cameraSpeed * new Vector(1, 0, 0);
        else if (e.KeyChar == 'j')
          _scene.Lights.First().Position += cameraSpeed * new Vector(1, 0, 0);
      }
      if (_scene.Objects.Any(o => o is Sphere))
      {
        if (e.KeyChar == '8')
          _scene.Objects.First(o => o is Sphere).Position += cameraSpeed * new Vector(0, 0, 1);
        else if (e.KeyChar == '5')
          _scene.Objects.First(o => o is Sphere).Position -= cameraSpeed * new Vector(0, 0, 1);
        else if (e.KeyChar == '6')
          _scene.Objects.First(o => o is Sphere).Position -= cameraSpeed * new Vector(1, 0, 0);
        else if (e.KeyChar == '4')
          _scene.Objects.First(o => o is Sphere).Position += cameraSpeed * new Vector(1, 0, 0);
        (_scene.Objects.First(o => o is Sphere) as Sphere).Center = _scene.Objects.First(o => o is Sphere).Position;
      }
    }

    public virtual void OnMouseMoved(MouseMoveEventArgs mouseMoveEventArgs)
    {
    }



  }
}