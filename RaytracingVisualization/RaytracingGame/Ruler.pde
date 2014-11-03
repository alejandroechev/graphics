class Ruler implements IMovable
{
  Vector position;
  
  float sizeX = 320;
  float sizeY = 80;
  
  Target target;
  
  boolean isMovable;
  boolean isSelected;
  
  boolean isEnabled = false;
  
  Vector previousMouse;
    
  Ruler(Vector pos, Vector targ)
  {
    target = new Target(targ, true, "data//rotate.png");
    target.showLabel = false;
    position = pos;
    isMovable = true;
  
  }
    
   boolean isMovable(){ return isMovable;}
  
   void setPosition(Vector p)
    {
      Vector delta = Vector.Minus(p, previousMouse);
      position = Vector.Add(position, delta);
      target.position = Vector.Add(target.position, delta);
      previousMouse = p;
    }
    
    Vector getPosition()
    {
      return position;
    }
    
    void setFirstMouse(Vector p)
    {
      previousMouse = p;
    }
  
  boolean isInside(Vector p)
  {
   if(!isEnabled)
      return false;
    
    Vector delta = Vector.Minus(target.position, position);
      delta = Vector.Normalize3(delta);      
          
    float angle = atan2(delta.y, delta.x)+ PI/2;
    Matrix m = new Matrix();
    m.M11 = cos(-angle);
    m.M12 = -sin(-angle);
    m.M21 = sin(-angle);
    m.M22 = cos(-angle);
    
    Vector pTrans = new Vector(p.x - position.x, p.y - position.y);
    pTrans = Matrix.MultiplyVector(m, pTrans);
        
    if(pTrans.x > sizeX/2 || pTrans.x < -sizeX/2 
       || pTrans.y > 0  || pTrans.y < -sizeY)
      return false;
    return true;
      
  }
  
  String label()
  {
    return "";
  }
  
  Vector labelPosition()
  {
    return Vector.Add(position, new Vector(1.2*sizeX, 0));
  }
  
  void setSelected(boolean isSel)
  {
    isSelected = isSel;
  }
  
  void flipSelected()
  {
    isSelected = !isSelected;
  }
    
  
  void draw()
  {
    if(isEnabled)
    {
      stroke(150);
      line(target.position.x, target.position.y, position.x, position.y);      
   
      target.draw();
      
      Vector delta = Vector.Minus(target.position, position);
      delta = Vector.Normalize3(delta);      
          
      pushMatrix();
      
      float angle = atan2(delta.y, delta.x);
      translate(position.x, position.y);
      rotate(angle + PI/2);
      if(isSelected)
      {
         stroke(0,0,0,0);
         fill(0,0,0,25);
         rect(-sizeX/2, -sizeY, sizeX, sizeY);
      }
      stroke(0);
      fill(0,0,0,0);
      
      rect(-sizeX/2, -sizeY, sizeX,sizeY);
      fill(0);
      for(int i=0; i<sizeX; i+=5)
      {
        if(i % 2 == 0)
        {  
          line(-sizeX/2 + i ,-sizeY, -sizeX/2 + i ,-sizeY + 10);
          text(""+((i)/10), -sizeX/2 + i, -sizeY + 15);
        }
        else
        {
          line(-sizeX/2 + i ,-sizeY, -sizeX/2 + i ,-sizeY + 5);
        }  
      }
      
      popMatrix();
    }
    
  }
  
  void update()
  {
    Vector delta = Vector.Minus(target.position, position);
    delta = Vector.Normalize3(delta);   
    target.position = Vector.Add(position, Vector.MultiplyS(delta, 100));
  }
 
  
}
