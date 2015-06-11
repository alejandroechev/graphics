namespace ThreeAPI.scene
{
  public interface IDataNode
  {
    ISceneNode Load();
    void WriteParameter(string parameterName, string parameterValue);
    string ReadParameter(string parameterName);
  }
}