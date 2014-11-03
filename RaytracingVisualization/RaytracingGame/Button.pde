class Button implements IControl
{
  //IMouseListener mouseListener;
  
  Vector position;
  float sizeX;
  float sizeY;
  
  PImage baseImage;
  PImage hoverImage;
  
  boolean isEnabled;
  
  Button(Vector p, float sX, float sY, String baseFile, String hoverFile, boolean enabled)
  {
    position = p;
    sizeX = sX;
    sizeY = sY;
    baseImage = loadImage(baseFile);
    hoverImage = loadImage(hoverFile);
    isEnabled = enabled;
  }
  
  void draw()
  {
    Vector mouse = new Vector(mouseX, mouseY);
    if(isInside(mouse))
      image(hoverImage, position.x - sizeX/2, position.y - sizeY/2, sizeX,sizeY);
    else
      image(baseImage, position.x - sizeX/2, position.y - sizeY/2, sizeX,sizeY);
  }
  
  boolean isInside(Vector p)
  {
    Vector center = new Vector(position.x, position.y);
    float d = center.Distance3(p);    
    return d < sizeX/2;
  }
  
  void click(float x, float y, int button)
  {
  }
  
}

class MinusFOVButton extends Button
{
  
  Camera camera;
  
  MinusFOVButton(Vector p, float sX, float sY, String baseFile, String hoverFile, Camera cam, boolean enabled)
  {
    super(p,sX,sY,baseFile, hoverFile, enabled);
    camera = cam;
  }
  
  void draw()
  {
    super.draw();
    stroke(0);
    fill(0);
    text(AppStrings.getString("fov") +  ": "+ int(camera.fov) + "˚", position.x + 2*sizeX/3, position.y + sizeY/3);
  }
  
  void click(float x, float y, int button)
  {
    if(isInside(new Vector(x,y)) && isEnabled)
    {
      if(camera.fov > 0)
        camera.fov--;
      camera.update();
    }   
  }
}

class PlusFOVButton extends Button
{
  
  Camera camera;
  
  PlusFOVButton(Vector p, float sX, float sY, String baseFile, String hoverFile, Camera cam, boolean enabled)
  {
    super(p,sX,sY,baseFile, hoverFile, enabled);
    camera = cam;
  }
  
  void click(float x, float y, int button)
  {
    if(isInside(new Vector(x,y)) && isEnabled)
    {
      if(camera.fov < 180)
        camera.fov++;
      camera.update();
    }   
  }
}

class MinusRaysButton extends Button
{
  
  Camera camera;
  Display display;
  
  MinusRaysButton(Vector p, float sX, float sY, String baseFile, String hoverFile, Camera cam, Display dis, boolean enabled)
  {
    super(p,sX,sY,baseFile, hoverFile, enabled);
    camera = cam;
    display = dis;
  }
  
  void draw()
  {
    super.draw();
    stroke(0);
    fill(0);
    text(AppStrings.getString("rays") + ":"+ + camera.rays, position.x + 2*sizeX/3-2, position.y + sizeY/3);
  }
  
  void click(float x, float y, int button)
  {
    if(isInside(new Vector(x,y)) && isEnabled)
    {
      if(camera.rays > 0)
        camera.rays--;
      display.width = camera.rays;
      display.update();
    }   
  }
}

class PlusRaysButton extends Button
{  
  Camera camera;
  Display display;
  
  PlusRaysButton(Vector p, float sX, float sY, String baseFile, String hoverFile, Camera cam, Display dis, boolean enabled)
  {
    super(p,sX,sY,baseFile, hoverFile, enabled);
    camera = cam;
    display = dis;
  }
  
  void click(float x, float y, int button)
  {
    if(isInside(new Vector(x,y)) && isEnabled)
    {
      camera.rays++;
      display.width = camera.rays;
      display.update();
    }   
  }
}

class UpButton extends Button
{  
  Camera camera;
  Display display;
  
  UpButton(Vector p, float sX, float sY, String baseFile, String hoverFile, Camera cam, Display dis, boolean enabled)
  {
    super(p,sX,sY,baseFile, hoverFile, enabled);
    camera = cam;
    display = dis;
  }
  
  void draw()
  {
    super.draw();
    stroke(0);
    fill(0);
    text(AppStrings.getString("up") + ": "+ camera.up, position.x + 2*sizeX/3, position.y + sizeY/3);
  }
  
  void click(float x, float y, int button)
  {
    if(isInside(new Vector(x,y)) && isEnabled)
    {
      camera.up *= -1;
      display.up *= -1;
      display.update();
    }   
  }
}

class NextButton extends Button
{  
  LevelManager man;
  ControlP5 controlP5;
  Textfield myTextfield;
  
  NextButton(Vector p, float sX, float sY, String baseFile, String hoverFile, boolean enabled, LevelManager m, PApplet _app)
  {
    super(p,sX,sY,baseFile, hoverFile, enabled);
    man = m;
    controlP5 = new ControlP5(_app);
    controlP5.setColorValue(color(255));
    myTextfield = controlP5.addTextfield("texting",int(p.x) - 100, 0, 90, int(sY));
 
  }
  
  void click(float x, float y, int button)
  {
    if(isInside(new Vector(x,y)) && isEnabled)
    {
      man.next(myTextfield.getText());
    }   
  }
}
