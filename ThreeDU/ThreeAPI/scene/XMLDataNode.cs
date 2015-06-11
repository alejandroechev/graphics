using System;
using System.Xml.Linq;

namespace ThreeAPI.scene
{
  public class XMLDataNode : IDataNode
  {
    private readonly XElement _element;
    private readonly ISceneNodeFactory _sceneNodeFactory;
    private readonly IDataNodeFactory _dataNodeFactory;

    public XMLDataNode(XElement element, ISceneNodeFactory sceneNodeFactory, IDataNodeFactory dataNodeFactory)
    {
      _element = element;
      _sceneNodeFactory = sceneNodeFactory;
      _dataNodeFactory = dataNodeFactory;
    }

    public ISceneNode Load()
    {
      var type = (SceneNodeType)Enum.Parse(typeof(SceneNodeType), _element.Name.ToString());
      var sceneNode = _sceneNodeFactory.CreateSceneNode(type);
      sceneNode.Load(this);
      foreach (var childElement in _element.Elements())
      {
        var childDataNode = _dataNodeFactory.CreateXmlDataNode(childElement);
        var childSceneNode = childDataNode.Load();
        sceneNode.AddChild(childSceneNode);
      }
      return sceneNode;
    }

    public void WriteParameter(string parameterName, string parameterValue)
    {
      throw new System.NotImplementedException();
    }

    public string ReadParameter(string parameterName)
    {
      return _element.Attribute(parameterName).Value;
    }
  }
}