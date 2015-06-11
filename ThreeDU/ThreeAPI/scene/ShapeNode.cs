using System;

namespace ThreeAPI.scene
{
  public class ShapeNode : BaseSceneNode
  {
    private readonly IShape _shape;
    private readonly IShapeFactory _shapeFactory;

    public IShape Shape
    {
      get { return _shape; }
    }

    public ShapeNode(IShape shape, IShapeFactory shapeFactory)
      : this(shapeFactory)
    {
      _shape = shape;
    }

    public ShapeNode(IShapeFactory shapeFactory)
    {
      _shapeFactory = shapeFactory;
    }

    public override void Load(IDataNode dataNode)
    {
      var type = dataNode.ReadParameter("type");
      var filePath = dataNode.ReadParameter("filePath");
      
      var shapeType = (ShapeType)Enum.Parse(typeof(ShapeType), type);
      var shape = _shapeFactory.CreateShape(shapeType);
      shape.Load(filePath);
    }
  }
}