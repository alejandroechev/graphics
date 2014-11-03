class Vertex implements IMovable
{
  Vector v;
  Vector altV;
  Vector n;
  float vertexSize = 10;
  color diffuse;
  color specular;
  color ambient;
  color shading;
  float shininess;
  float reflectiveness;
  
  boolean movable;
  boolean isSelected;
  boolean shade;
  boolean correctPerspective;
  
  Vector previousMouse;
  
  Vertex()
  {
    this(new Vector(), false, color(0));
  }
  
  Vertex(Vector _v, boolean _mov, color _c)
  {
    this(_v, _mov, _c, 10);
  }
  
  Vertex(Vector _v, boolean _mov, color _c, float size)
  {
    v = _v;
    altV = v;
    movable = _mov;
    diffuse = _c;
    vertexSize = size;
    shading = diffuse;
  }
  
  Vector getV()
  {
    return correctPerspective ? altV : v;
  }
  
  void draw()
  {
    Vector current = correctPerspective ? altV : v;
    if(isSelected)
     {
       stroke(0,0,0,0);
       fill(0,0,0,25);
       ellipse(current.x, current.y, 2.5*vertexSize, 2.5*vertexSize);
     }
     
    stroke(shade ? shading : diffuse);
    fill(shade ? shading : diffuse);
    ellipse(current.x, current.y, vertexSize,vertexSize);
  }
  
  void setMaterial(color diff, color spec, color amb, float shin, float ref)
  {
    diffuse = diff;
    specular = spec;
    ambient = amb;
    shininess = shin;
    reflectiveness = ref;
  }
  
  boolean isMovable()
  {
    return movable;
  }
  
   void setPosition(Vector p)
    {
      Vector delta = Vector.Minus(p, previousMouse);
      v = Vector.Add(v, delta);
      previousMouse = p;
    }
    Vector getPosition()
    {
      return v;
    }
    void setFirstMouse(Vector p)
    {
      previousMouse = p;
    }
  
  boolean isInside(Vector p)
  {
    float d = v.Distance3(p);    
    return d < vertexSize;
  }
  
  String label()
  {
    return "";//v.ToString2D();
  }
  
  Vector labelPosition()
  {
    return Vector.Add(v, new Vector(1.2*vertexSize, 0));
  }
  
  void setSelected(boolean isSel)
    {
      isSelected = isSel;
    }
    
    void flipSelected()
    {
      isSelected = !isSelected;
    }
    
    
  
}
