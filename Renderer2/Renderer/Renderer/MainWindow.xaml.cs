using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SceneLib;

namespace Renderer
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : IDisplay
  {
    private BitmapSource _bitmap;
    private PixelFormat _pixelFormat = PixelFormats.Rgb24;
    private int _width, _height, _rawStride;
    private byte[] _pixelData;

    private Scene _scene;
    private readonly List<IRender> _renderers = new List<IRender>();
    private int _currentRendererIndex = 0;

    private readonly Timer _timer = new Timer();
    private DateTime _initialTime;

    private readonly TextureSamplerFactory _textureSamplerFactory = new TextureSamplerFactory();

    public MainWindow()
    {
      InitializeComponent();
      Loaded += OnLoaded;
      KeyDown += OnKeyDown;
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
      const float deltaCamera = 50;
      if (e.Key == Key.R)
        _currentRendererIndex = (_currentRendererIndex + 1) % _renderers.Count;
      if (e.Key == Key.W)
        _scene.Camera.MoveForward(deltaCamera);
      if (e.Key == Key.S)
        _scene.Camera.MoveForward(-deltaCamera);
      if (e.Key == Key.A)
        _scene.Camera.MoveSideways(-deltaCamera);
      if (e.Key == Key.D)
        _scene.Camera.MoveSideways(deltaCamera);
      if (e.Key == Key.P)
        _renderers[_currentRendererIndex].IsParallel = !_renderers[_currentRendererIndex].IsParallel;
      if (e.Key == Key.B)
      {
        foreach (var renderObject in _scene.Objects)
        {
          if (renderObject is IHaveTextureSampler)
            (renderObject as IHaveTextureSampler).TextureSampler = _textureSamplerFactory.CreateBilinearSampler();
        }
      }
      if (e.Key == Key.N)
      {
        foreach (var renderObject in _scene.Objects)
        {
          if (renderObject is IHaveTextureSampler)
            (renderObject as IHaveTextureSampler).TextureSampler = _textureSamplerFactory.CreateNearestNeighbourSampler();
        }
      }
      UpdateRenderer();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      RenderingParameters.Instance.Load();
      var meshLoader = new ObjMeshLoader();
      var renderObjectFactory = new RenderObjectFactory(meshLoader, _textureSamplerFactory);
      _scene = new Scene(RenderingParameters.Instance.ImageWidth, RenderingParameters.Instance.ImageHeight, renderObjectFactory);
      _scene.Load(RenderingParameters.Instance.ScenePath);

      Width = _scene.Width * 1.5;
      Height = _scene.Height * 1.5;
      Display.Width = _scene.Width;
      Display.Height = _scene.Height;

      _width = _scene.Width;
      _height = _scene.Height;
      _rawStride = (_width * _pixelFormat.BitsPerPixel + 7) / 8;
      _pixelData = new byte[_rawStride * _height];

      _renderers.Add(new Raytracer(_scene, this));
      _renderers.Add(new DistributionRaytracer(_scene, this, 256));
      UpdateRenderer();
    }

    private async void UpdateRenderer()
    {
      RendererName.Text = _renderers[_currentRendererIndex].Name;
      _timer.Interval = 1;
      _timer.Elapsed += TimerChanged;
      _timer.Start();
      var taskFactory = new TaskFactory();
      _initialTime = DateTime.Now;
      await taskFactory.StartNew(() => _renderers[_currentRendererIndex].Render());
      _bitmap = BitmapSource.Create(_width, _height,
                96, 96, _pixelFormat, null, _pixelData, _rawStride);
      Display.Source = _bitmap;
      _timer.Stop();
    }

    private void TimerChanged(object sender, ElapsedEventArgs e)
    {
      Dispatcher.Invoke(() => RendererName.Text = _renderers[_currentRendererIndex].Name + ": " + (e.SignalTime - _initialTime).Ticks / 10000000.0f);
    }

    public void UpdateDisplay()
    {
      
    }


    public void SetPixel(int x, int y, float r, float g, float b)
    {
      int xIndex = x * 3;
      int yIndex = (_height - y - 1) * _rawStride;
      _pixelData[xIndex + yIndex] = (byte)(r * 255);
      _pixelData[xIndex + yIndex + 1] = (byte)(g * 255);
      _pixelData[xIndex + yIndex + 2] = (byte)(b * 255);
    }
  }
}
