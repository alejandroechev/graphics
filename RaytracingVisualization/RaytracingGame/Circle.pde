class Circle extends SceneObject
{
  Vector center;
  float radius; 
  
  
  Circle()
  {
    super(color(0), true);
    radius = 0;
    center = new Vector();
  }
  
  Circle(color c, Vector cent, float r, boolean movable)
  {
    super(c, movable);
    radius = r;
    center = cent;
  }
  
   void setPosition(Vector p)
    {
      Vector delta = Vector.Minus(p, previousMouse);
      center = Vector.Add(center, delta);
      previousMouse = p;
    }
    
    Vector getPosition()
    {
      return center;
    }
  
  
  float intersect(Ray r)
  {
    float A = Vector.Dot3(r.direction, r.direction);
    float B = 2 * Vector.Dot3(Vector.Minus(r.position, center), r.direction);
    float C = Vector.Dot3(Vector.Minus(r.position, center), Vector.Minus(r.position, center)) - (radius*radius);

    float discr = B*B - 4*A*C;
    if (discr < 0)
      return -1;
    else
    {
      discr = (float)Math.sqrt(discr);
      float t0 = (-B - discr) / (2 * A);
      if (t0 > 0)
        return t0;
      else
      {
        float t1 = (-B + discr) / (2 * A);
        if (t1 > 0)
         return t1;
      }
    }
    return -1;
  }
  
  void draw()
  {
     if(isSelected)
     {
       stroke(0,0,0,0);
       fill(0,0,0,25);
       ellipse(center.x, center.y, 2.5*radius, 2.5*radius);
     }
     strokeWeight(2);
     stroke(diffuse);
     fill(0,0,0,0);
     ellipse(center.x, center.y, 2*radius, 2*radius);
     strokeWeight(1);
  }
  
  Vector getNormal(Vector p)
  {
    Vector n = Vector.Normalize3(Vector.Minus(p, center));
    return n;
  }
  
  boolean isInside(Vector p)
  {
    float d = center.Distance3(p);    
    return d < radius;
  }
  
  String label()
  {
    return AppStrings.getString("circle") + ": " + new Vector(center.x, height - center.y).ToString2D();// + "\nRadius " + radius +  
    //"\nDiffuse (" + red(col) +", "+green(col) +", "+blue(col) +", "+alpha(col) +")" + "\nShininess " + shininess + "\nReflectivness " + reflectiveness;
  }
  
  Vector labelPosition()
  {
    return Vector.Add(center, new Vector(1.2*radius, 0));
  }
  
  
  
  String save()
  {
    String movableS = isMovable ? "true" : "false";
    return "circle:"+radius+";"+center.ToString()+";"+diffuse+";"+movableS;
  }
  
  void load(String s)
  {
    s = s.trim();
    String info = s.split(":")[1];
    String[] vals = info.split(";");
    radius = Float.parseFloat(vals[0]);
    center = new Vector();
    center.FromString(vals[1]);
    diffuse = Integer.parseInt(vals[2]);
    isMovable = vals[3].equals("true");
  }
}
