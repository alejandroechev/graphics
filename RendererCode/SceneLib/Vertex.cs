namespace SceneLib
{
  public class Vertex
  {
    public Vector Position { get; set; }
    public Vector Normal { get; set; }
    public Vector TextureCoordinates { get; set; }
    public Material Material { get; set; }

    public Vector Color { get; set; }
    public Vector PositionHomogeneus { get; set; }
    public Vector WorldPosition { get; set; }
    public Vector ReflectionDirection { get; set; }
    public Vector ShadowPosition { get; set; }

    public Vector SimPosition { get; set; }

    public Vertex()
    {
      TextureCoordinates = new Vector();
    }
  }
}
