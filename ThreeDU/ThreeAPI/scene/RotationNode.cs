using System.Globalization;
using OpenTK;

namespace ThreeAPI.scene
{
  public class RotationNode : TransformNode
  {
    private float _x;
    private float _y;
    private float _z;
    private float _angle;

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

    public float Angle
    {
      get { return _angle; }
      set { _angle = value; }
    }

    public RotationNode()
    {
      
    }

    public RotationNode(float x, float y, float z, float angle)
    {
      Init(x, y, z, angle);
    }
    
    private void Init(float x, float y, float z, float angle)
    {
      _x = x;
      _y = y;
      _z = z;
      _angle = angle;
      _transform = Matrix4.CreateFromAxisAngle(new Vector3(x, y, z), angle);
    }

    public override void Load(IDataNode dataNode)
    {
      float x = float.Parse(dataNode.ReadParameter("x"));
      float y = float.Parse(dataNode.ReadParameter("y"));
      float z = float.Parse(dataNode.ReadParameter("z"));
      float angle = float.Parse(dataNode.ReadParameter("angle")).ToRadians();
      Init(x, y, z, angle);
    }

    public override void Save(IDataNode dataNode)
    {
      dataNode.WriteParameter("x", _x.ToString(CultureInfo.InvariantCulture));
      dataNode.WriteParameter("y", _y.ToString(CultureInfo.InvariantCulture));
      dataNode.WriteParameter("z", _z.ToString(CultureInfo.InvariantCulture));
      dataNode.WriteParameter("angle", _angle.ToString(CultureInfo.InvariantCulture));
    }
  }
}