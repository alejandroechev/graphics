using System.Globalization;
using OpenTK;

namespace ThreeAPI.scene
{
  public class TranslationNode : TransformNode
  {
    private float _x;
    private float _y;
    private float _z;

    public float X
    {
      get { return _x; }
      set { _x = value; }
    }

    public float Y
    {
      get { return _y; }
      set { _y = value; }
    }

    public float Z
    {
      get { return _z; }
      set { _z = value; }
    }

    public TranslationNode(float x, float y, float z)
    {
      Init(x, y, z);
    }

    private void Init(float x, float y, float z)
    {
      _x = x;
      _y = y;
      _z = z;
      _transform = Matrix4.CreateTranslation(x, y, z);
    }

    public TranslationNode()
    {
      
    }

    public override void Load(IDataNode dataNode)
    {
      float x = float.Parse(dataNode.ReadParameter("x"));
      float y = float.Parse(dataNode.ReadParameter("y"));
      float z = float.Parse(dataNode.ReadParameter("z"));
      Init(x,y,z);
    }

    public override void Save(IDataNode dataNode)
    {
      dataNode.WriteParameter("x", _x.ToString(CultureInfo.InvariantCulture));
      dataNode.WriteParameter("y", _y.ToString(CultureInfo.InvariantCulture));
      dataNode.WriteParameter("z", _z.ToString(CultureInfo.InvariantCulture));
    }
  }
}