using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    private bool _isAsync = true;

    public MainWindow()
    {
      InitializeComponent();
      Loaded += OnLoaded;
      KeyDown += OnKeyDown;
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
      const float deltaCamera = 2f;
      if (e.Key == Key.R)
      {
        _currentRendererIndex = (_currentRendererIndex + 1) % _renderers.Count;
        RendererName.Text = _renderers[_currentRendererIndex].Name;
        UpdateRendererAsync();
      }
      if (e.Key == Key.W)
      {
        _scene.Camera.MoveForward(deltaCamera);
        UpdateRendererAsync();
      }
      if (e.Key == Key.S)
      {
        _scene.Camera.MoveForward(-deltaCamera);
        UpdateRendererAsync();
      }
      if (e.Key == Key.A)
      {
        _scene.Camera.MoveSideways(-deltaCamera);
        UpdateRendererAsync();
      }

      if (e.Key == Key.D)
      {
        _scene.Camera.MoveSideways(deltaCamera);
        UpdateRendererAsync();
      }
      if (e.Key == Key.P)
      {
        _renderers[_currentRendererIndex].IsParallel = !_renderers[_currentRendererIndex].IsParallel;
        UpdateRendererAsync();
      }
      if (e.Key == Key.B)
      {
        foreach (var renderObject in _scene.Objects)
        {
          if (renderObject is IHaveTextureSampler)
            (renderObject as IHaveTextureSampler).TextureSampler = _textureSamplerFactory.CreateBilinearSampler();
        }
        UpdateRendererAsync();
      }
      if (e.Key == Key.N)
      {
        foreach (var renderObject in _scene.Objects)
        {
          if (renderObject is IHaveTextureSampler)
            (renderObject as IHaveTextureSampler).TextureSampler = _textureSamplerFactory.CreateNearestNeighbourSampler();
        }
        UpdateRendererAsync();
      }
      if (e.Key == Key.G)
      {
        using (var fileStream = new FileStream("image.png", FileMode.Create))
        {
          var encoder = new PngBitmapEncoder();
          encoder.Frames.Add(BitmapFrame.Create(_bitmap));
          encoder.Save(fileStream);
        }
      }
      if (e.Key == Key.I)
      {
        _scene.Lights.First().Position.Z -= 10f;
        UpdateRendererAsync();
      }
      if (e.Key == Key.K)
      {
        _scene.Lights.First().Position.Z += 10f;
        UpdateRendererAsync();
      }
      if (e.Key == Key.J)
      {
        _scene.Lights.First().Position.X -= 10f;
        UpdateRendererAsync();
      }
      if (e.Key == Key.L)
      {
        _scene.Lights.First().Position.X += 10f;
        UpdateRendererAsync();
      }
      if (e.Key == Key.O)
      {
        RenderingParameters.Instance.UsePerspectiveProjection = !RenderingParameters.Instance.UsePerspectiveProjection;
        UpdateRendererAsync();
      }
      if (e.Key == Key.Z)
      {
        RenderingParameters.Instance.EnableDepthBuffer = !RenderingParameters.Instance.EnableDepthBuffer;
        UpdateRendererAsync();
      }

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

      _renderers.Add(new Rasterizer(_scene, this));
      _renderers.Add(new WireFrameRasterizer(_scene, this));
      _renderers.Add(new PointRasterizer(_scene, this));
      _renderers.Add(new Raytracer(_scene, this));
      _renderers.Add(new DistributionRaytracer(_scene, this, RenderingParameters.Instance.NumberOfSamplesPerPixel));
      RendererName.Text = _renderers[_currentRendererIndex].Name;
      UpdateRendererAsync();
    }

    private async void UpdateRendererAsync()
    {
      if (!_isAsync)
      {
        _renderers[_currentRendererIndex].Render();
        UpdateDisplay();
      }
      else
      {
        _timer.Interval = 1;
        _timer.Elapsed += TimerChanged;
        _timer.Start();
        var taskFactory = new TaskFactory();
        _initialTime = DateTime.Now;
        await taskFactory.StartNew(() => _renderers[_currentRendererIndex].Render());
        UpdateDisplay();
        _timer.Stop(); 
      }
    }

    private void TimerChanged(object sender, ElapsedEventArgs e)
    {
      Dispatcher.Invoke(() => RendererName.Text = _renderers[_currentRendererIndex].Name + ": " + (e.SignalTime - _initialTime).Ticks / 10000000.0f);
    }

    public void UpdateDisplay()
    {
      _bitmap = BitmapSource.Create(_width, _height,
                96, 96, _pixelFormat, null, _pixelData, _rawStride);
      Display.Source = _bitmap;
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
