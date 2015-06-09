using System;
using System.Collections.Generic;

namespace SceneLib
{
  /// <summary>
  /// Corresponds to a sphere geometrical object
  /// </summary>
  public abstract class SphereBase : RenderObject, IHavePolygons
  {
    public Vector Center { get; set; }
    public float Radius { get; set; }
    public Material Material { get; set; }
    public List<PolygonBase> Polygons { get { return _sphereMesh.Polygons; } }
    private readonly MeshBase _sphereMesh;

    protected SphereBase(MeshBase sphereMesh)
    {
      if (sphereMesh == null) throw new ArgumentNullException("sphereMesh");
      _sphereMesh = sphereMesh;
    }

    public void Load()
    {
      _sphereMesh.Material = Material;
      _sphereMesh.Position = Center;
      _sphereMesh.Scale = new Vector(Radius, Radius, Radius);
      _sphereMesh.Rotation = Rotation;
      Position = Center;
      Scale = new Vector(Radius, Radius, Radius);
      Rotation = Rotation;
      _sphereMesh.Load();
      
    }
   
  }
}