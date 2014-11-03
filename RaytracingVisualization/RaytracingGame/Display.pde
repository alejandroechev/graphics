class Display
{
  float displaySize;
  int width;
  color[] pixelValues;
  String name;
  Vector position;
  float pixelHeight = 20;
  
  boolean showable;
  
  boolean clickable = false;
  
  int up = 1;
  
  Display(Vector p, int w, float s, String n, boolean show)
  {
    position = p;
    width = w;
    displaySize = s;
    pixelValues = new color[width];
    for(int i=0; i<width; i++)
      pixelValues[i] = color(255);
    name = n;
    showable = show;
  }
  
  void draw()
  {
    float pixelSize = displaySize/width; 
    for(int i=0; i<width; i++)
    {
      int index = (up == 1) ? width - i - 1 : i;
      stroke(pixelValues[index]);
      fill(pixelValues[index]);
      rect(position.x + pixelSize*i, position.y , pixelSize, pixelHeight);
      stroke(0);
      line(position.x + pixelSize*i, position.y, position.x + pixelSize*i, position.y-5);
    }
    stroke(0);
    fill(0,0,0,0);
    rect(position.x, position.y , displaySize, pixelHeight);
    fill(0);
    text(AppStrings.getString("pixels") + ": "+ width, position.x + pixelSize*(width) + 10, position.y + 10);
    text(name, position.x - 50, position.y + 10);
    
  }
  
  void update()
  {
    pixelValues = new color[width];
    for(int i=0; i<width; i++)
      pixelValues[i] = color(255);
  }
  
  void mouseClicked()
  {
    if(clickable)
    {
      float pixelSize = displaySize/width;
      for(int i=0; i<width; i++)
      {
        int index = (up == 1) ? width - i - 1 : i;
        if(mouseX > position.x + pixelSize*i && mouseX < position.x + pixelSize*(i+1) &&
           mouseY > position.y && mouseY < position.y + pixelHeight)
        {
          if(pixelValues[index] == color(0))
            pixelValues[index] = color(255); 
          else if(pixelValues[index] == color(255))
            pixelValues[index] = color(0);
        }       
      }
    }
  }
  
  boolean hasColor()
  {
    for(int i=0; i<width; i++)
    {
      if(pixelValues[i] != color(255))
        return true;
    }
    return false;
  }
}
