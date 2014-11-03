using System.IO;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using SceneLib;

namespace Renderer.Base
{
  public class GPUTextureRenderOpenGlManager : BaseTextureRenderOpenGLManager
  {
    private const string _fragmentShaderFilePath = @"Shaders\CornellBoxRaytracerFragmentShader.fx";
    private const string _vertexShaderFilePath = @"Shaders\TransformationVertexShader2DH.fx";
    private int _timeHandler;
    private int _resolutionHandler;
    private int _mouseHandler;
    private readonly float[] mouse = new float[2];
    private readonly float[] resolution = new float[2];

    public GPUTextureRenderOpenGlManager(Scene scene)
      : base(scene)
    {
      resolution[0] = scene.Width;
      resolution[1] = scene.Height;
    }

    public override void Update(double time)
    {

    }

    protected override void LoadShaders()
    {
      //_vertexShaderSource = File.ReadAllText(_vertexShaderFilePath);
      _fragmentShaderSource = File.ReadAllText(_fragmentShaderFilePath);
    }

    protected override void CreateShaders()
    {
      base.CreateShaders();
      _timeHandler = GL.GetUniformLocation(_shaderProgramHandle, "time");
      _mouseHandler = GL.GetUniformLocation(_shaderProgramHandle, "mouse");
      _resolutionHandler = GL.GetUniformLocation(_shaderProgramHandle, "resolution");
    }

    protected override void InnerRender(double time)
    {
      GL.Uniform1(_timeHandler, (float)time);
      GL.Uniform2(_mouseHandler, 1, mouse);
      GL.Uniform2(_resolutionHandler, 1, resolution);

    }

    public override void OnMouseMoved(MouseMoveEventArgs mouseMoveEventArgs)
    {
      mouse[0] = mouseMoveEventArgs.X;
      mouse[1] = _scene.Height - mouseMoveEventArgs.Y;
    }

  }
}