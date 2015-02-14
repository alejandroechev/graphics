using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Renderer
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow
  {
    private BitmapSource _bitmap;
    private PixelFormat _pixelFormat = PixelFormats.Rgb24;
    private int _width, _height, _rawStride;
    private byte[] _pixelData;

    public MainWindow()
    {
      InitializeComponent();
      Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      _width = (int)Display.Width;
      _height = (int)Display.Height;
      _rawStride = (_width * _pixelFormat.BitsPerPixel + 7) / 8;
      _pixelData = new byte[_rawStride * _height];

      UpdateDisplay();
    }

    private void UpdateDisplay()
    {
      Render();
      _bitmap = BitmapSource.Create(_width, _height,
                96, 96, _pixelFormat, null, _pixelData, _rawStride);
      Display.Source = _bitmap;
    }

    private void Render()
    {
      for (int y = 0; y < _height; y++)
        for (int x = 0; x < _width; x++)
          SetPixel(x, y, Colors.Blue);
    }

    private void SetPixel(int x, int y, Color c)
    {
      int xIndex = x * 3;
      int yIndex = y * _rawStride;
      _pixelData[xIndex + yIndex] = c.R;
      _pixelData[xIndex + yIndex + 1] = c.G;
      _pixelData[xIndex + yIndex + 2] = c.B;
    }
  }
}
