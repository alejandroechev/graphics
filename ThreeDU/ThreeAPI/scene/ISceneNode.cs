using System.Collections.Generic;

namespace ThreeAPI.scene
{
  public enum SceneNodeType
  {
    Scene = 0,
    Translate,
    Rotate,
    Scale,
    Shape
  }

  public interface ISceneNode
  {
    IEnumerable<ISceneNode> Children { get; }
    void AddChild(ISceneNode child);
    void RemoveChild(ISceneNode child);
    void Load(IDataNode dataNode);
    void Save(IDataNode dataNode);
  }
}