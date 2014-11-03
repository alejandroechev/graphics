using SceneLib;

namespace Renderer.Base
{
  public class DependencyInjectorContainer
  {
    private static readonly DependencyInjectorContainer instance = new DependencyInjectorContainer();
    static DependencyInjectorContainer() {} 
   
    public static DependencyInjectorContainer Instance
    {
      get { return instance; }
    }

    private readonly IRenderObjectFactory _renderObjectFactory;
    private readonly ITextureSamplerFactory _textureSamplerFactory;
    private readonly IPixelSamplerFactory _pixelSamplerFactory;
    private readonly IMeshLoader _meshLoader;

    public IRenderObjectFactory RenderObjectFactory
    {
      get { return _renderObjectFactory; }
    }

    public ITextureSamplerFactory TextureSamplerFactory
    {
      get { return _textureSamplerFactory; }
    }

    public IPixelSamplerFactory PixelSamplerFactory
    {
      get { return _pixelSamplerFactory; }
    }

    private DependencyInjectorContainer()
    {
      _meshLoader = new ObjMeshLoader();
      _textureSamplerFactory = new TextureSamplerFactory();
      _renderObjectFactory = new RenderObjectFactory(_meshLoader);
      _pixelSamplerFactory = new PixelSamplerFactory();
    }
  }
}
