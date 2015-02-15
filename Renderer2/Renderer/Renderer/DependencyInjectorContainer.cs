using SceneLib;

namespace Renderer
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

    public IRenderObjectFactory RenderObjectFactory
    {
      get { return _renderObjectFactory; }
    }

    public ITextureSamplerFactory TextureSamplerFactory
    {
      get { return _textureSamplerFactory; }
    }

    private DependencyInjectorContainer()
    {
      var meshLoader = new ObjMeshLoader();
      _textureSamplerFactory = new TextureSamplerFactory();
      _renderObjectFactory = new RenderObjectFactory(meshLoader);
    }
  }
}
