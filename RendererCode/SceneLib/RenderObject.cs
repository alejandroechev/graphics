namespace SceneLib
{
  /// <summary>
  /// Represent an abstract object in the scene
  /// </summary>
  public abstract class RenderObject
  {
    public string Name { get; set; }
    public Vector Scale { get; set; }
    public Vector Position { get; set; }
    public Vector Rotation { get; set; }
    public Vector Velocity { get; set; }

    protected RenderObject()
    {
      Scale = new Vector(1, 1, 1);
      Position = new Vector(0, 0, 0);
      Rotation = new Vector(0, 0, 0);
      Velocity= new Vector(0,0,0);
    }

    public abstract bool Intersect(Ray ray);
    public abstract Vector GetNormal(Vector point, float time);
    public abstract Material GetMaterial(Vector point);

    public virtual void Update(float deltaTime)
    {
      
    }

  }
}