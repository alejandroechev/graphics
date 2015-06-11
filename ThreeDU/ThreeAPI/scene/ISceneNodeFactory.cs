namespace ThreeAPI.scene
{
  public interface ISceneNodeFactory
  {
    ISceneNode CreateSceneNode(SceneNodeType type);
  }
}