using System.Collections.Generic;

namespace SceneLib
{
  public interface IHaveTriangles
  {
    List<TriangleBase> Triangles { get; }
    Vector Scale { get; set; }
    Vector Position { get; set; }
    Vector Rotation { get; set; }
  }
}