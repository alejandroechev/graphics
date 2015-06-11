namespace ThreeAPI.scene
{
  public enum ShapeType
  {
    Mesh = 0,
    PointCloud,
    Curve
  }

  public interface IShapeFactory
  {
    IShape CreateShape(ShapeType type);
  }
}