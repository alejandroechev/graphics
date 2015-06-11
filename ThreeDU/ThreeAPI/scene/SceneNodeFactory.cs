using System;

namespace ThreeAPI.scene
{
  public class SceneNodeFactory : ISceneNodeFactory
  {
    private IShapeFactory _shapeFactory;

    public SceneNodeFactory(IShapeFactory shapeFactory)
    {
      _shapeFactory = shapeFactory;
    }

    public ISceneNode CreateSceneNode(SceneNodeType type)
    {
      switch (type)
      {
        case SceneNodeType.Scene:
          return new BaseSceneNode();
        case SceneNodeType.Translate:
          return new TranslationNode();
        case SceneNodeType.Rotate:
          return new RotationNode();
        case SceneNodeType.Scale:
          return new ScaleNode();
        case SceneNodeType.Shape:
          return new ShapeNode(_shapeFactory);
        default:
          throw new ArgumentOutOfRangeException("type");
      }
    }
  }
}