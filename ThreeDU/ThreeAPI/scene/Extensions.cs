using System;

namespace ThreeAPI.scene
{
  public static class Extensions
  {
    public static float ToRadians(this float angle)
    {
      return (float)(Math.PI * (angle / 180.0f));
    }
  }
}
