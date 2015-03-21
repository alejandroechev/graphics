using System.Windows;
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
    private Raytracer _raytracer;

    public MainWindow()
    {
      InitializeComponent();
      Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      RenderingParameters.Instance.Load();
      var meshLoader = new ObjMeshLoader();
      var renderObjectFactory = new RenderObjectFactory(meshLoader);
      _scene = new Scene(RenderingParameters.Instance.ImageWidth, RenderingParameters.Instance.ImageHeight, renderObjectFactory);
      _scene.Load(RenderingParameters.Instance.ScenePath);

      Width = _scene.Width * 1.2;
      Height = _scene.Height * 1.2;
      Display.Width = _scene.Width;
      Display.Height = _scene.Height;

      _width = _scene.Width;
      _height = _scene.Height;
      _rawStride = (_width * _pixelFormat.BitsPerPixel + 7) / 8;
      _pixelData = new byte[_rawStride * _height];

      _raytracer = new Raytracer(_scene, this);
      _raytracer.Render();
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
