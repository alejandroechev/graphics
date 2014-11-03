class Target implements IMovable
{
        Vector position;
        
        PImage img;
        float sizeX = 16;
        float sizeY = 16;
        
        boolean isMovable;
        boolean isSelected;
        
        boolean showLabel = true;
        
        Vector previousMouse;
        
        Target(Vector p, boolean movable, String imagePath)
        {
          position = p;
          isMovable = movable;
          img = loadImage(imagePath);
        }
        
         void setPosition(Vector p)
        {
          Vector delta = Vector.Minus(p, previousMouse);
          position = Vector.Add(position, delta);
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
        
        boolean isMovable(){ return isMovable;}
        
        boolean isInside(Vector p)
        {
          Vector center = new Vector(position.x, position.y);
          float d = center.Distance3(p);    
          return d < sizeX/2;
        }
        
        String label()
        {
          if(!showLabel)
            return "";
          return AppStrings.getString("target")  + ": " + new Vector(position.x, height - position.y).ToString2D();
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
      if(isSelected)
       {
         stroke(0,0,0,0);
         fill(0,0,0,25);
         ellipse(position.x, position.y, 1.5*sizeX, 1.5*sizeY);
       }
      stroke(255);
      fill(255);
      image(img, position.x - sizeX/2, position.y - sizeY/2, sizeX,sizeY);
    }
        
   
}
