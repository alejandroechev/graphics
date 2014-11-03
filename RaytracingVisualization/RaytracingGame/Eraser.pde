class Eraser extends Circle
{
  float sizeX;
  float sizeY;
  PImage img;
  boolean isEnabled = false;
  
  Eraser(Vector cent, float r)
  {
    super(color(0), cent, r, true);
    sizeX = 2*r;
    sizeY = 2*r;
    img = loadImage("data//eraser.png");  
  }
  
  void draw()
  {
    if(isEnabled)
    {
      pushMatrix();
      stroke(255);
      fill(255);
      translate(center.x, center.y);
      image(img, - sizeX/2, - sizeY/2, sizeX,sizeY);
      popMatrix();
    }
  }
}
