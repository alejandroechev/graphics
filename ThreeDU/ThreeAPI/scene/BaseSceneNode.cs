using System.Collections.Generic;

namespace ThreeAPI.scene
{
  public class BaseSceneNode : ISceneNode
  {
    protected List<ISceneNode> _children = new List<ISceneNode>(); 
    
    public IEnumerable<ISceneNode> Children
    {
      get { return _children; }
    }

    public void AddChild(ISceneNode child)
    {
      _children.Add(child);
    }

    public void RemoveChild(ISceneNode child)
    {
      _children.Remove(child);
    }

    public virtual void Load(IDataNode dataNode)
    {

    }

    public virtual void Save(IDataNode dataNode)
    {
      
    }
  }
}