using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using SceneLib;

namespace Renderer
{
  public class Rasterizer : AbstractRenderer
  {
    public Rasterizer(Scene scene, IDisplay display)
      : base(scene, display)
    {
    }

    public override void Render()
    {
      var meshes = _scene.Objects.Where(o => o is IHaveTriangles).Cast<IHaveTriangles>().ToList();
      var triangles = new List<TriangleBase>();
      meshes.ForEach(m => m.Triangles.ForEach(t => triangles.Add(t)));
      var vertices = new List<Vertex>();
      triangles.ForEach(t => t.Vertices.ForEach(v => vertices.Add(ProcessVertex(v))));



      //var vertices = triangles.Select(t => t.)
/*
vertices' = vertices.AsParallel.Process
triangles = vertices.Map
triangles' = triangles.ClippAndCull
pixels = triangles.AsParallel.Rasterize
pixels' = pixels.AsParallel.Process
pixels'' = pixels'.Map
pixels''' = pizels''.Blend
pixels'''.Paint*/
    }

    private Vertex ProcessVertex(Vertex vertex)
    {
      return null;
    }
  }
}