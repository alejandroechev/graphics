using System.Xml.Linq;

namespace ThreeAPI.scene
{
  public class DataNodeFactory : IDataNodeFactory
  {
    private readonly ISceneNodeFactory _sceneNodeFactory;

    public DataNodeFactory(ISceneNodeFactory sceneNodeFactory)
    {
      _sceneNodeFactory = sceneNodeFactory;
    }

    public IDataNode CreateXmlDataNode(XElement element)
    {
      return new XMLDataNode(element, _sceneNodeFactory, this);
    }
  }
}