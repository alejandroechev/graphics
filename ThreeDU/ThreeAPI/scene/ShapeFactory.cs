using System;

namespace ThreeAPI.scene
{
  public class ShapeFactory : IShapeFactory
  {
    private readonly IMeshLoader _meshLoader;

    public ShapeFactory(IMeshLoader meshLoader)
    {
      _meshLoader = meshLoader;
    }

    public IShape CreateShape(ShapeType type)
    {
      switch (type)
      {
        case ShapeType.Mesh:
          return new Mesh(_meshLoader);
        case ShapeType.PointCloud:
          throw new NotImplementedException();
        case ShapeType.Curve:
          throw new NotImplementedException();
        default:
          throw new ArgumentOutOfRangeException("type");
      }
    }
  }
}