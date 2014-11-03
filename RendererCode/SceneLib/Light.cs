namespace SceneLib
{
  /// <summary>
  /// Represents a point light
  /// </summary>
  public class Light
  {
    public float AtenuationConstant { get; set; }
    public float AtenuationLinear { get; set; }
    public float AtenuationQuadratic { get; set; }
    public Vector Color { get; set; }
    public Vector Position { get; set; }
    public Vector Direction { get; set; }

    public Vector Width { get; set; }
    public Vector Height { get; set; }

    public Light()
    {
      Width = new Vector();
      Height = new Vector();

    }

  }
}