namespace SceneLib
{
    public class MathUtils
    {
        public static float[] GetVector3Array(Vector v)
        {
            var array = new float[3];
            array[0] = v.X;
            array[1] = v.Y;
            array[2] = v.Z;
            return array;
        }

        public static float[] GetVector4Array(Vector v)
        {
            var array = new float[4];
            array[0] = v.X;
            array[1] = v.Y;
            array[2] = v.Z;
            array[3] = 1;
            return array;
        }


    }
}
