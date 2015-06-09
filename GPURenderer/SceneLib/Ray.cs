namespace SceneLib
{
  public class Ray
  {
    public Vector Position { get; set; }
    public Vector Direction { get; set; }
    public float IntersectionDistance { get; set; }
    public RenderObject IntersectedObject { get; set; }

    public float Time { get; set; }

    public Ray(Vector pos, Vector dir)
    {
      Position = pos;
      Direction = dir.Normalize3();
      IntersectionDistance = float.MaxValue;
    }

    public Vector GetIntersectionPoint()
    {
      return Position + IntersectionDistance*Direction;
    }
  }
}