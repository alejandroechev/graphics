class Ray
{
  Vector position;
  Vector direction;
  float firstT;
  SceneObject firstObj;
  float maxSize = 100;
  float arrowSize = 8;
  
  color col;
  
  Ray(Vector p, Vector d)
  {
    position = p;
    direction = d;
    firstT = 100000;
    col = color(180);
  }
  
  float intersect(SceneObject o)
  {
    return o.intersect(this);
  }
  
  void draw()
  {
    stroke(col);
    fill(col);
    Vector end = Vector.Add(position, Vector.Multiply(firstT, direction));
    line(position.x, position.y, end.x, end.y);
    
    float ponder = min(firstT, maxSize);
    
    Vector middle = Vector.Add(position, Vector.Multiply(ponder, direction));
    Vector dirPerp1 = new Vector(-direction.y, direction.x);
    Vector dirPerp2 = new Vector(direction.y, -direction.x);
    Vector arrowPoint1 = Vector.Add(middle, Vector.Multiply(arrowSize, direction));
    Vector arrowPoint2 = Vector.Add(middle, Vector.Multiply(arrowSize/2, dirPerp1));
    Vector arrowPoint3 = Vector.Add(middle, Vector.Multiply(arrowSize/2, dirPerp2));
    
    triangle(arrowPoint1.x, arrowPoint1.y, arrowPoint2.x, arrowPoint2.y, arrowPoint3.x, arrowPoint3.y);  
  }
  
}
