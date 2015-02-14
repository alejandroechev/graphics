namespace SceneLib
{
  public class Matrix
  {
    private float[,] matrixData = new float[4, 4];

    public float[,] MatrixData
    {
      get { return matrixData; }
      set { matrixData = value; }
    }

    public float M11 { get { return matrixData[0, 0]; } set { matrixData[0, 0] = value; } }
    public float M12 { get { return matrixData[0, 1]; } set { matrixData[0, 1] = value; } }
    public float M13 { get { return matrixData[0, 2]; } set { matrixData[0, 2] = value; } }
    public float M14 { get { return matrixData[0, 3]; } set { matrixData[0, 3] = value; } }
    public float M21 { get { return matrixData[1, 0]; } set { matrixData[1, 0] = value; } }
    public float M22 { get { return matrixData[1, 1]; } set { matrixData[1, 1] = value; } }
    public float M23 { get { return matrixData[1, 2]; } set { matrixData[1, 2] = value; } }
    public float M24 { get { return matrixData[1, 3]; } set { matrixData[1, 3] = value; } }
    public float M31 { get { return matrixData[2, 0]; } set { matrixData[2, 0] = value; } }
    public float M32 { get { return matrixData[2, 1]; } set { matrixData[2, 1] = value; } }
    public float M33 { get { return matrixData[2, 2]; } set { matrixData[2, 2] = value; } }
    public float M34 { get { return matrixData[2, 3]; } set { matrixData[2, 3] = value; } }
    public float M41 { get { return matrixData[3, 0]; } set { matrixData[3, 0] = value; } }
    public float M42 { get { return matrixData[3, 1]; } set { matrixData[3, 1] = value; } }
    public float M43 { get { return matrixData[3, 2]; } set { matrixData[3, 2] = value; } }
    public float M44 { get { return matrixData[3, 3]; } set { matrixData[3, 3] = value; } }

    public static Vector operator *(Matrix M, Vector v)
    {
      var x = M.M11 * v.X + M.M12 * v.Y + M.M13 * v.Z + M.M14 * v.W;
      var y = M.M21 * v.X + M.M22 * v.Y + M.M23 * v.Z + M.M24 * v.W;
      var z = M.M31 * v.X + M.M32 * v.Y + M.M33 * v.Z + M.M34 * v.W;
      var w = M.M41 * v.X + M.M42 * v.Y + M.M43 * v.Z + M.M44 * v.W;

      var transformedVector = new Vector(x, y, z, w);
      return transformedVector;
    }

    public static Matrix operator *(float scalar, Matrix M)
    {
      var result = new Matrix();
      result.M11 = M.M11 * scalar;
      result.M12 = M.M12 * scalar;
      result.M13 = M.M13 * scalar;
      result.M14 = M.M14 * scalar;
      result.M21 = M.M21 * scalar;
      result.M22 = M.M22 * scalar;
      result.M23 = M.M23 * scalar;
      result.M24 = M.M24 * scalar;
      result.M31 = M.M31 * scalar;
      result.M32 = M.M32 * scalar;
      result.M33 = M.M33 * scalar;
      result.M34 = M.M34 * scalar;
      result.M41 = M.M41 * scalar;
      result.M42 = M.M42 * scalar;
      result.M43 = M.M43 * scalar;
      result.M44 = M.M44 * scalar;
      return result;
    }

    public static Matrix operator *(Matrix M1, Matrix M2)
    {
      var v1 = new Vector(M2.M11, M2.M21, M2.M31, M2.M41);
      var v2 = new Vector(M2.M12, M2.M22, M2.M32, M2.M42);
      var v3 = new Vector(M2.M13, M2.M23, M2.M33, M2.M43);
      var v4 = new Vector(M2.M14, M2.M24, M2.M34, M2.M44);

      var v1Result = M1 * v1;
      var v2Result = M1 * v2;
      var v3Result = M1 * v3;
      var v4Result = M1 * v4;

      var result = new Matrix();
      result.M11 = v1Result.X;
      result.M12 = v2Result.X;
      result.M13 = v3Result.X;
      result.M14 = v4Result.X;
      result.M21 = v1Result.Y;
      result.M22 = v2Result.Y;
      result.M23 = v3Result.Y;
      result.M24 = v4Result.Y;
      result.M31 = v1Result.Z;
      result.M32 = v2Result.Z;
      result.M33 = v3Result.Z;
      result.M34 = v4Result.Z;
      result.M41 = v1Result.W;
      result.M42 = v2Result.W;
      result.M43 = v3Result.W;
      result.M44 = v4Result.W;
      return result;
    }

    public static Matrix Identity()
    {
      var result = new Matrix();
      result.M11 = 1;
      result.M12 = 0;
      result.M13 = 0;
      result.M14 = 0;
      result.M21 = 0;
      result.M22 = 1;
      result.M23 = 0;
      result.M24 = 0;
      result.M31 = 0;
      result.M32 = 0;
      result.M33 = 1;
      result.M34 = 0;
      result.M41 = 0;
      result.M42 = 0;
      result.M43 = 0;
      result.M44 = 1;
      return result;
    }

    public float Determinant()
    {
      return M11 * M22 * M33 * M44 +
             M11 * M23 * M34 * M42 +
             M11 * M24 * M32 * M43 +
             M12 * M21 * M34 * M43 +
             M12 * M23 * M31 * M44 +
             M12 * M24 * M33 * M41 +
             M13 * M21 * M32 * M44 +
             M13 * M22 * M34 * M41 +
             M13 * M24 * M31 * M42 +
             M14 * M21 * M33 * M42 +
             M14 * M22 * M31 * M43 +
             M14 * M23 * M32 * M41 -
             M11 * M22 * M34 * M43 -
             M11 * M23 * M32 * M44 -
             M11 * M24 * M33 * M42 -
             M12 * M21 * M33 * M44 -
             M12 * M23 * M34 * M41 -
             M12 * M24 * M31 * M43 -
             M13 * M21 * M34 * M42 -
             M13 * M22 * M31 * M44 -
             M13 * M24 * M32 * M41 -
             M14 * M21 * M32 * M43 -
             M14 * M22 * M33 * M41 -
             M14 * M23 * M31 * M42;
    }

    public Matrix Inverse()
    {
      var inverse = new Matrix();
      inverse.M11 = M22 * M33 * M44 +
                    M23 * M34 * M42 +
                    M24 * M32 * M43 -
                    M22 * M34 * M43 -
                    M23 * M32 * M44 -
                    M24 * M33 * M42;
      inverse.M12 = M12 * M34 * M43 +
                    M13 * M32 * M44 +
                    M14 * M33 * M42 -
                    M12 * M33 * M44 -
                    M13 * M34 * M42 -
                    M14 * M32 * M43;
      inverse.M13 = M12 * M23 * M44 +
                    M13 * M24 * M42 +
                    M14 * M22 * M43 -
                    M12 * M24 * M43 -
                    M13 * M22 * M44 -
                    M14 * M23 * M42;
      inverse.M14 = M12 * M24 * M33 +
                    M13 * M22 * M34 +
                    M14 * M23 * M32 -
                    M12 * M23 * M34 -
                    M13 * M24 * M32 -
                    M14 * M22 * M33;
      inverse.M21 = M21 * M34 * M43 +
                    M23 * M31 * M44 +
                    M24 * M33 * M41 -
                    M21 * M33 * M44 -
                    M23 * M34 * M41 -
                    M24 * M31 * M43;
      inverse.M22 = M11 * M33 * M44 +
                      M13 * M34 * M41 +
                      M14 * M31 * M43 -
                      M11 * M34 * M43 -
                      M13 * M31 * M44 -
                      M14 * M33 * M41;
      inverse.M23 = M11 * M24 * M43 +
                      M13 * M21 * M44 +
                      M14 * M23 * M41 -
                      M11 * M23 * M44 -
                      M13 * M24 * M41 -
                      M14 * M21 * M43;
      inverse.M24 = M11 * M23 * M34 +
                      M13 * M24 * M31 +
                      M14 * M21 * M33 -
                      M11 * M24 * M33 -
                      M13 * M21 * M34 -
                      M14 * M23 * M31;
      inverse.M31 = M21 * M32 * M44 +
                    M22 * M34 * M41 +
                    M24 * M31 * M42 -
                    M21 * M34 * M42 -
                    M22 * M31 * M44 -
                    M24 * M32 * M41;
      inverse.M32 = M11 * M34 * M42 +
                      M12 * M31 * M44 +
                      M14 * M32 * M41 -
                      M11 * M32 * M44 -
                      M12 * M34 * M41 -
                      M14 * M31 * M42;
      inverse.M33 = M11 * M22 * M44 +
                      M12 * M24 * M41 +
                      M14 * M21 * M42 -
                      M11 * M24 * M42 -
                      M12 * M21 * M44 -
                      M14 * M22 * M41;
      inverse.M34 = M11 * M24 * M32 +
                      M12 * M21 * M34 +
                      M14 * M22 * M31 -
                      M11 * M22 * M34 -
                      M12 * M24 * M31 -
                      M14 * M21 * M32;
      inverse.M41 = M21 * M33 * M42 +
                    M22 * M31 * M43 +
                    M23 * M32 * M41 -
                    M21 * M32 * M43 -
                    M22 * M33 * M41 -
                    M23 * M31 * M42;
      inverse.M42 = M11 * M32 * M43 +
                      M12 * M33 * M41 +
                      M13 * M31 * M42 -
                      M11 * M33 * M42 -
                      M12 * M31 * M43 -
                      M13 * M32 * M41;
      inverse.M43 = M11 * M23 * M42 +
                      M12 * M21 * M43 +
                      M13 * M22 * M41 -
                      M11 * M22 * M43 -
                      M12 * M23 * M41 -
                      M13 * M21 * M42;
      inverse.M44 = M11 * M22 * M33 +
                      M12 * M23 * M31 +
                      M13 * M21 * M32 -
                      M11 * M23 * M32-
                      M12 * M21 * M33 -
                      M13 * M22 * M31;


      float det = Determinant();

      return (1.0f/det)*inverse;

    }

    public Matrix Transpose()
    {
      var transpose = new Matrix();
      for (int i = 0; i < 4; i++)
      {
        for (int j = 0; j < 4; j++)
        {
          transpose.MatrixData[i, j] = this.MatrixData[j, i];
        }
      }
      return transpose;
    }


  }
}
