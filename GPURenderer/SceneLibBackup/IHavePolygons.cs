using System.Collections.Generic;

namespace SceneLib
{
  public interface IHavePolygons
  {
    List<PolygonBase> Polygons { get; }
    Vector Scale { get; set; }
    Vector Position { get; set; }
    Vector Rotation { get; set; }
  }
}