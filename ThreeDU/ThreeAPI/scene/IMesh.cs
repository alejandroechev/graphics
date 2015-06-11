using System.Collections.Generic;

namespace ThreeAPI.scene
{
  public interface IMesh : IShape
  {
    IEnumerable<IVertex> Vertices { get; }
    IEnumerable<IPolygon> Polygons { get; }
    IPolygon AddPolygon(List<IVertex> vertices);
  }
}