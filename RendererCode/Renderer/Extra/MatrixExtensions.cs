using System;
using SceneLib;

namespace Renderer.Base
{
  public static class MatrixExtensions
  {
    public static Matrix Translation(Vector translation)
    {
      var transMat = new Matrix();

      transMat.M11 = 1;
      transMat.M12 = 0;
      transMat.M13 = 0;
      transMat.M14 = translation.X;
      transMat.M21 = 0;
      transMat.M22 = 1;
      transMat.M23 = 0;
      transMat.M24 = translation.Y;
      transMat.M31 = 0;
      transMat.M32 = 0;
      transMat.M33 = 1;
      transMat.M34 = translation.Z;
      transMat.M41 = 0;
      transMat.M42 = 0;
      transMat.M43 = 0;
      transMat.M44 = 1;

      return transMat;
    }

    public static Matrix Scaling(Vector scaling)
    {

      var scalingMat = new Matrix();
      scalingMat.M11 = scaling.X;
      scalingMat.M12 = 0;
      scalingMat.M13 = 0;
      scalingMat.M14 = 0;
      scalingMat.M21 = 0;
      scalingMat.M22 = scaling.Y;
      scalingMat.M23 = 0;
      scalingMat.M24 = 0;
      scalingMat.M31 = 0;
      scalingMat.M32 = 0;
      scalingMat.M33 = scaling.Z;
      scalingMat.M34 = 0;
      scalingMat.M41 = 0;
      scalingMat.M42 = 0;
      scalingMat.M43 = 0;
      scalingMat.M44 = 1;
      return scalingMat;
    }

    public static Matrix Rotation(Vector rotation)
    {
      var xRadAngle = (float)(rotation.X * Math.PI / 180.0f);
      var yRadAngle = (float)(rotation.Y * Math.PI / 180.0f);
      var zRadAngle = (float)(rotation.Z * Math.PI / 180.0f);

      var rotMatX = new Matrix();
      rotMatX.M11 = 1;
      rotMatX.M12 = 0;
      rotMatX.M13 = 0;
      rotMatX.M14 = 0;
      rotMatX.M21 = 0;
      rotMatX.M22 = (float)Math.Cos(xRadAngle);
      rotMatX.M23 = -(float)Math.Sin(xRadAngle);
      rotMatX.M24 = 0;
      rotMatX.M31 = 0;
      rotMatX.M32 = (float)Math.Sin(xRadAngle);
      rotMatX.M33 = (float)Math.Cos(xRadAngle);
      rotMatX.M34 = 0;
      rotMatX.M41 = 0;
      rotMatX.M42 = 0;
      rotMatX.M43 = 0;
      rotMatX.M44 = 1;

      var rotMatY = new Matrix();
      rotMatY.M11 = (float)Math.Cos(yRadAngle);
      rotMatY.M12 = 0;
      rotMatY.M13 = (float)Math.Sin(yRadAngle);
      rotMatY.M14 = 0;
      rotMatY.M21 = 0;
      rotMatY.M22 = 1;
      rotMatY.M23 = 0;
      rotMatY.M24 = 0;
      rotMatY.M31 = -(float)Math.Sin(yRadAngle);
      rotMatY.M32 = 0;
      rotMatY.M33 = (float)Math.Cos(yRadAngle);
      rotMatY.M34 = 0;
      rotMatY.M41 = 0;
      rotMatY.M42 = 0;
      rotMatY.M43 = 0;
      rotMatY.M44 = 1;

      var rotMatZ = new Matrix();
      rotMatZ.M11 = (float)Math.Cos(zRadAngle);
      rotMatZ.M12 = -(float)Math.Sin(zRadAngle);
      rotMatZ.M13 = 0;
      rotMatZ.M14 = 0;
      rotMatZ.M21 = (float)Math.Sin(zRadAngle);
      rotMatZ.M22 = (float)Math.Cos(zRadAngle);
      rotMatZ.M23 = 0;
      rotMatZ.M24 = 0;
      rotMatZ.M31 = 0;
      rotMatZ.M32 = 0;
      rotMatZ.M33 = 1;
      rotMatZ.M34 = 0;
      rotMatZ.M41 = 0;
      rotMatZ.M42 = 0;
      rotMatZ.M43 = 0;
      rotMatZ.M44 = 1;

      return rotMatX * rotMatY * rotMatZ;
    }

    public static Matrix BasisChangeToCanonicalFrame(Vector u, Vector v, Vector w, Vector e)
    {
      var matrix = new Matrix();
      matrix.M11 = u.X;
      matrix.M21 = u.Y;
      matrix.M31 = u.Z;
      matrix.M12 = v.X;
      matrix.M22 = v.Y;
      matrix.M32 = v.Z;
      matrix.M13 = w.X;
      matrix.M23 = w.Y;
      matrix.M33 = w.Z;
      matrix.M14 = e.X;
      matrix.M24 = e.Y;
      matrix.M34 = e.Z;
      matrix.M44 = 1;
      return matrix;

    }

    public static Matrix BasisChangeFromCanonicalFrame(Vector u, Vector v, Vector w, Vector e)
    {
      var toCanonical = BasisChangeToCanonicalFrame(u, v, w, e);
      return toCanonical.Inverse();
    }
  }
}
