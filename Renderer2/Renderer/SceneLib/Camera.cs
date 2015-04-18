namespace SceneLib
{
  /// <summary>
  /// Represents a camera
  /// </summary>
  public class Camera
  {
    public Vector Position { get; set; }
    public Vector Target { get; set; }
    public Vector Up { get; set; }
    public float FieldOfView { get; set; }
    public float NearClip { get; set; }
    public float FarClip { get; set; }
    public float LensSize { get; set; }
    public float ExposureTime { get; set; }

  public void MoveForward(float delta)
    {
      Vector direction = Target - Position;
      direction = direction.Normalize3();
      Position = Position + delta*direction;
      Target = Target + delta*direction;
  }

    public void MoveSideways(float delta)
    {
      Vector direction = Vector.Cross3(Target - Position, Up);
      direction = direction.Normalize3();
      Position = Position + delta * direction;
      Target = Target + delta * direction;
    }

   
  }
}