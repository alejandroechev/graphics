//Copyright Alejandro Echeverr�a 2011
static class Vector
{
        float x;
        float y;
        float z;
        float w;

        public Vector(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
        
        public Vector(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = 1;
        }
        
        public Vector(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0;
            this.w = 1;
        }
        
         public Vector()
        {
            this.x = 0;
            this.y = 0;
            this.z = 0;
            this.w = 1;
        }
        

        /// <summary>
        /// Returns the magnitude of a 3-dimensional vector
        /// </summary>
        /// <returns></returns>
        public float Magnitude3()
        {
            return (float)Math.sqrt(x * x + y * y + z * z);
        }

        /// <summary>
        /// Normalizes a 3-dimensional vector
        /// </summary>
        public Vector Normalize3()
        {
            float currentMagnitude = Magnitude3();
            if (currentMagnitude == 0)
                return new Vector();
            x /= currentMagnitude;
            y /= currentMagnitude;
            z /= currentMagnitude;
            return this;
        }

        /// <summary>
        /// Limit a 3-dimensional vector
        /// </summary>
        public Vector Limit3(float limit)
        {
            x = Math.min(x, limit);
            y = Math.min(y, limit);
            z = Math.min(z, limit);
            return this;
        }

        public float Distance3(Vector original)
        {
            return Minus(this, original).Magnitude3();
        }


        public boolean Equals(Vector obj)
        {
            Vector other = obj;
            return this.x == other.x && this.y == other.y && this.z == other.z;
        }        

        public static Vector Add(Vector v1, Vector v2)
        {
            return new Vector(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z, v1.w + v2.w);
        }

        public static Vector Minus(Vector v1, Vector v2)
        {
            return new Vector(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z, v1.w - v2.w);
        }

        public static Vector Multiply(float scalar, Vector v)
        {
            return new Vector(scalar * v.x, scalar * v.y, scalar * v.z, scalar*v.w);
        }

        public static Vector MultiplyS(Vector v, float scalar)
        {
            return new Vector(scalar * v.x, scalar * v.y, scalar * v.z, scalar * v.w);
        }

        public static Vector MultiplyV(Vector v1, Vector v2)
        {
            return new Vector(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z, v1.w * v2.w);
        }

        public static Vector Division(Vector v, float scalar)
        {
            return new Vector(v.x / scalar, v.y / scalar, v.z / scalar, v.w / scalar);
        }

        public static Vector Cross3(Vector v1, Vector v2)
        {
            Vector crossVec = new Vector();
            crossVec.x = v1.y * v2.z - v2.y * v1.z;
            crossVec.y = v2.x * v1.z - v1.x * v2.z;
            crossVec.z = v1.x * v2.y - v2.x * v1.y;
            crossVec.w = 0.0f;
            return crossVec;
        }
        

        public static Vector Normalize3(Vector original)
        {
            Vector v = new Vector();
            float currentMagnitude = original.Magnitude3();
            if (currentMagnitude == 0)
                return v;
            v.x = original.x / currentMagnitude;
            v.y = original.y / currentMagnitude;
            v.z = original.z / currentMagnitude;
            return v;
        }

        
        /// <summary>
        /// Dot product of a 3-dimensional vector
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static float Dot3(Vector v1, Vector v2)
        {
            return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
        }


        public String ToString()
        {
            return "(" +x +"," +y +"," +z + ")";
        }
        
        public String ToString2D()
        {
            return "(" +x +"," +y +")";
        }
        
        void FromString(String s)
        {
          s = s.replace('(', ' ');
          s = s.replace(')', ' ');
          s = s.trim();
          String[] vals = s.split(",");
          x = Float.parseFloat(vals[0]);
          y = Float.parseFloat(vals[1]);
          z = Float.parseFloat(vals[2]);
        }

    }
