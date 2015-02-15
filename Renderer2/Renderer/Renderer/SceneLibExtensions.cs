using System;
using System.Collections.Generic;
using SceneLib;

namespace Renderer
{
  public static class SceneLibExtensions
  {

    public static float ToRadians(this float angle)
    {
      return (float) (Math.PI*(angle/180.0f));
    }
  }
}
