using System.Collections.Generic;
using OpenTK;

namespace ThreeAPI.scene
{
  public interface IPolygon
  {
    IEnumerable<IVertex> Vertices { get; }
    Vector3 GetFaceNormal();
  }
}