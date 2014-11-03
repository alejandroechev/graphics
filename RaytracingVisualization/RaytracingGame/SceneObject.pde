class SceneObject implements IMovable
{
    color diffuse;
    color specular;
    color ambient;
    float shininess;
    float reflectiveness;
    float refractionIndex;
    boolean isSelected;
    boolean isMovable;
    
    Vector previousMouse;
        
    SceneObject(color c, boolean movable)
    {
        diffuse = c;
        specular = color(0);
        ambient = color(0);
        isSelected = false;
        isMovable = movable;
        shininess = 1;
        reflectiveness = 0;
        refractionIndex = 0.7;
    }
    
    boolean isMovable(){ return isMovable;}
    
     void setPosition(Vector p)
    {
      
    }
    Vector getPosition()
    {
      return null;
    }
    void setFirstMouse(Vector p)
    {
      previousMouse = p;
    }
    
    float intersect(Ray r)
    {
      return 0;
    }
    
    Vector labelPosition()
    {
      return new Vector();
    }
    
    void draw()
    {
    }
    
    Vector getNormal(Vector p)
    {
      return null;
    }
    
    boolean isInside(Vector p)
    {
      return false;
    }
    
    String label()
    {
      return "Object";
    }
    
    void setSelected(boolean isSel)
    {
      isSelected = isSel;
    }
    
    void flipSelected()
    {
      isSelected = !isSelected;
    }
    
    
    
    
    String save()
    {
      return "";
    }
    
    void load(String s)
    {
    }
    
}
