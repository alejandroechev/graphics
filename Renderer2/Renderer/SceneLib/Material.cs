using System.Drawing;

namespace SceneLib
{
  /// <summary>
  /// Represents a material
  /// </summary>
  public class Material
  {
    
    public string Name { get; set; }
    public Bitmap DiffuseTexture { get; set; }
    public Bitmap SpecularTexture { get; set; }
    public Bitmap NormalTexture { get; set; }
    public Bitmap DisplacementMap { get; set; }
    public Bitmap EnvironmentMap { get; set; }
    public bool HasDiffuseTexture { get { return DiffuseTexture != null; }}
    public bool HasSpecularTexture { get { return SpecularTexture != null; } }
    public bool HasNormalTexture { get { return NormalTexture != null; } }
    public bool HasDisplacementMap { get { return DisplacementMap != null; } }
    public bool HasEnvironmentMap { get { return EnvironmentMap != null; } }
    public Vector Diffuse { get; set; }
    public Vector Specular { get; set; }
    public float ReflectivityAttenuation { get; set; }
    public float RefractiveIndex { get; set; }
    public float RefractiveAttenuation { get; set; }
    public float Shininess { get; set; }

    public Material()
    {
      Diffuse = new Vector();
      Specular = new Vector();
    }

    public Material Clone()
    {
      var material = new Material();
      material.Diffuse = Diffuse.Clone();
      material.Specular = Specular.Clone();
      material.ReflectivityAttenuation = ReflectivityAttenuation;
      material.RefractiveIndex = RefractiveIndex;
      material.RefractiveAttenuation = RefractiveAttenuation;
      material.Shininess = Shininess;
      return material;
    }

    
    
  }
}