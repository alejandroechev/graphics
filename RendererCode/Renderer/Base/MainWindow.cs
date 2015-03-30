using System;
using OpenTK;
using OpenTK.Input;
using SceneLib;
using EnableCap = OpenTK.Graphics.OpenGL.EnableCap;
using GL = OpenTK.Graphics.OpenGL.GL;

namespace Renderer.Base
{
  class MainWindow : GameWindow
  {
    private readonly PolygonRenderOpenGLManager _interactiveTransformationRenderingManager;
    private readonly CPUTextureRenderOpenGlManager _transformationRenderingManager;
    private readonly CPUTextureRenderOpenGlManager _simulationRenderingManager;
    private readonly GPUTextureRenderOpenGlManager _interactiveSimulationRenderingManager;

    private BaseOpenGLManager _manager;
    private readonly Scene _scene;
    public MainWindow(int width, int height)
      : base(width, height,
        new OpenTK.Graphics.GraphicsMode(), "1. CPU Raytracer", GameWindowFlags.Default,
        DisplayDevice.Default, 3, 0,
        OpenTK.Graphics.GraphicsContextFlags.ForwardCompatible | OpenTK.Graphics.GraphicsContextFlags.Debug)
    {

      _scene = new Scene(width, height, DependencyInjectorContainer.Instance.RenderObjectFactory, DependencyInjectorContainer.Instance.TextureSamplerFactory);
      _scene.Load(RenderingParameters.Instance.ScenePath);

      var simpleSampler = DependencyInjectorContainer.Instance.PixelSamplerFactory.CreateSampler(width, height, RenderingParameters.Instance.SamplesPerPixel, RenderingParameters.Instance.MinimumTime, RenderingParameters.Instance.MaximumTime);

      _interactiveTransformationRenderingManager = new PolygonRenderOpenGLManager(_scene);
      _transformationRenderingManager = new CPUTextureRenderOpenGlManager(_scene, simpleSampler, new TransformationRenderer(_scene));
      _simulationRenderingManager = new CPUTextureRenderOpenGlManager(_scene, simpleSampler, new SimulationRenderer(_scene, simpleSampler));
      _interactiveSimulationRenderingManager = new GPUTextureRenderOpenGlManager(_scene);
      _manager = _simulationRenderingManager;

      Mouse.Move += MouseMoved;
    }

    private void MouseMoved(object sender, MouseMoveEventArgs e)
    {
      _manager.OnMouseMoved(e);
    }

    protected override void OnLoad(EventArgs e)
    {
      VSync = VSyncMode.On;
      GL.Enable(EnableCap.DepthTest);
      GL.Enable(EnableCap.Texture2D);
      GL.ClearColor(_scene.BackgroundColor.ToColor4());
      _manager.Load();

    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      _manager.Update(e.Time);
    }

    private float time = 0;
    protected override void OnRenderFrame(FrameEventArgs e)
    {
      time += 0.1f;
      _manager.Render(time);
      SwapBuffers();
    }


    protected override void OnKeyPress(KeyPressEventArgs e)
    {
      if (e.KeyChar == '1')
      {
        _simulationRenderingManager.Load();
        _manager = _simulationRenderingManager;
        Title = "1. CPU Raytracer";
      }
      else if (e.KeyChar == '2')
      {
        _interactiveSimulationRenderingManager.Load();
        _manager = _interactiveSimulationRenderingManager;
        Title = "2. GPU Raytracer";
      }
      else if (e.KeyChar == '3')
      {
        _transformationRenderingManager.Load();
        _manager = _transformationRenderingManager;
        Title = "3. CPU Rasterizer";
      }
      else if (e.KeyChar == '4')
      {
        _interactiveTransformationRenderingManager.Load();
        _manager = _interactiveTransformationRenderingManager;
        Title = "4. GPU Rasterizer";
      }
      else
      {
        _manager.OnKeyPress(e);
      }
    }

    [STAThread]
    public static void Main()
    {
      RenderingParameters.Instance.Load();
      using (var window = new MainWindow(RenderingParameters.Instance.ImageWidth, RenderingParameters.Instance.ImageHeight))
      {
        window.Run(RenderingParameters.Instance.FrameRate);
      }
    }
  }
}
