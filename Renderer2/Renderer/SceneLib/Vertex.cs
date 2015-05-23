namespace SceneLib
{
  public class Vertex
  {
    public Vector Position { get; set; }
    public Vector Normal { get; set; }
    public Vector TextureCoordinates { get; set; }
    public Material Material { get; set; }

    public TriangleBase ParentTriangle { get; set; }

    public Vertex()
    {
      TextureCoordinates = new Vector();
    }
  }
}
