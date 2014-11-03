class Light implements IMovable
{
        float atenuationConstant = 1;
        float atenuationLinear = 0;
        float atenuationQuadratic = 0.00001f;
        color col;
        Vector position;
        
        PImage img;
        float sizeX = 25;
        float sizeY = 25;
        
        boolean isMovable;
        boolean isSelected;
        
        Vector previousMouse;
        
        Light()
        {
          position = new Vector();
          col = color(0);
          img = loadImage("data//sun.png");
          isMovable = true;
        }
        
        Light(Vector p, color c, boolean movable)
        {
          position = p;
          col = c;
          img = loadImage("data//sun.png");
          isMovable = movable;
        }
        
        boolean isMovable(){ return isMovable;}
        
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
        
        boolean isInside(Vector p)
        {
          Vector center = new Vector(position.x, position.y);
          float d = center.Distance3(p);    
          return d < sizeX/2;
        }
        
        String label()
        {
          return AppStrings.getString("light") + ": " + new Vector(position.x, height - position.y).ToString2D();// +
                //" \n Atten.Linear " + atenuationLinear + "\n Atten.Quadratic " + atenuationQuadratic;
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
        
       
        
        String save()
        {
          String movableS = isMovable ? "true" : "false";
          return "light:"+position.ToString()+";"+col+";"+movableS;
        }
        
        void load(String s)
        {
          s = s.trim();
          String info = s.split(":")[1];
          String[] vals = info.split(";");
          position = new Vector();
          position.FromString(vals[0]);
          col = Integer.parseInt(vals[1]);
          isMovable = vals[2].equals("true");
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
          tint(col);
          image(img, position.x - sizeX/2, position.y - sizeY/2, sizeX,sizeY);
          tint(255);
        }
        
}
