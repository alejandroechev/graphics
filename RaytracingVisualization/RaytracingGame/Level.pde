class Level
{
  Camera cam;
  Camera camGoal;
  Target target;
  Display dis;
  Display disGoal;
  CameraGUI camGUI;
  
  ArrayList movableObjects;
  ArrayList objects;
  IMovable selected;
  IMovable mouseOvered;
  IMovable clicked;
  ArrayList lights;
  
  int w = 10;
  float n = 75;
  float f = 400;
  float fov = 45;
  
  color ambient = color(0);
  color back = color(255);  
  Vector backVec = new Vector(1,1,1);
   
  boolean showRays = false;
  boolean showShadowRays = false;
  boolean showReflectedRays = false;
  boolean showNormals = false;
  boolean showHalfVectors = false;
  boolean showLabels = true;
  boolean showDisplay = false;
  boolean showCameraElements = false;
  //boolean showImagePlane = false;
  
  boolean enableRays = true;
  boolean enableFOV = true;
  boolean cameraMovable = true;
  
  int currentCursor = ARROW;
  
  Level()
  {
    
    selected = null;
    movableObjects = new ArrayList();
    objects = new ArrayList();
    lights = new ArrayList();
  }
  
  void init()
  {
    
  }
  
  void dispose()
  {
  }
  
  void load(String fileName)
  {     
     //init();
     
     String[] list = loadStrings(fileName);
     for(int i=0; i<list.length; i++)
     {
       String[] splitted = list[i].split(":");
       if(splitted[0].equals("circle"))
       {         
         Circle c = new Circle();
         c.load(list[i]);
         objects.add(c);
         if(c.isMovable)
           movableObjects.add(c);
       }
       else if(splitted[0].equals("triangle"))
       {         
         Triangle t = new Triangle();
         t.load(list[i]);
         objects.add(t);
         if(t.isMovable)
         {
           movableObjects.add(t.vertex[0]);
           movableObjects.add(t.vertex[1]);
           movableObjects.add(t.vertex[2]);
         }
       }
       else if(splitted[0].equals("camera"))
       {
         camGoal = new Camera(new Vector(), new Vector(), n, f, fov, w, false);
         camGoal.load(list[i]);
         disGoal = new Display(new Vector(0.1*width, 0.95*height), camGoal.rays, 0.8*width, "Goal", true);
         
       }
       else if(splitted[0].equals("light"))
       {
         Light l = new Light();
         l.load(list[i]);
         lights.add(l);
         if(l.isMovable)
           movableObjects.add(l);
       }
       else if(splitted[0].equals("showRays"))
       {
         showRays = splitted[1].equals("true");
       }
       else if(splitted[0].equals("showShadowRays"))
       {
         showShadowRays = splitted[1].equals("true");
       }
       else if(splitted[0].equals("showReflectedRays"))
       {
         showReflectedRays = splitted[1].equals("true");
       }
       else if(splitted[0].equals("enableFOV"))
       {
         enableFOV =  splitted[1].equals("true");
       }
       else if(splitted[0].equals("enableRays"))
       {
         enableRays =  splitted[1].equals("true");
       }
       else if(splitted[0].equals("showDisplay"))
       {
         showDisplay =  splitted[1].equals("true");
       }
       else if(splitted[0].equals("cameraMovable"))
       {
         cameraMovable =  splitted[1].equals("true");
       }
     }
     
    
      Vector t = new Vector(width/2 + 50, height/4);
      Vector pos = new Vector(width/2, height/4); 
      
      cam = new Camera(pos, t, n, f, fov, w, cameraMovable);
      target = cam.target;
      
      movableObjects.add(cam);
      movableObjects.add(target);
      dis = new Display(new Vector(0.1*width, 0.9*height), cam.rays, 0.8*width, "Display", showDisplay);
      camGUI = new CameraGUI(cam, dis, enableFOV, enableRays);   
   
  }
  
  void reset()
  {
    objects.clear();
    movableObjects.clear();
    lights.clear();
    init();
  }
  
  void update()
  {
    if(selected != null && selected.isMovable())
    {
      Vector mouse = new Vector(mouseX, mouseY);
      selected.setPosition(mouse);  
    }
    if(camGUI != null)
      camGUI.updatePositions(cam.position);
    cam.update();
    
  }
  
  void draw()
  {
    update();  
    background(back);     
    drawScene();
    
  }
  
  void drawScene()
  {
    for(Object o : objects)
    {
      SceneObject so = (SceneObject)o;
      so.draw();
    } 
    for(Object o : lights)
    {
      Light l = (Light)o;
      l.draw();
    } 
    
    cam.draw();
    if(dis != null && dis.showable)
      dis.draw();
    if(disGoal != null && disGoal.showable)
      disGoal.draw();
    if(camGUI != null && showCameraElements)
      camGUI.draw();
      
    if(clicked != null && showLabels)
    {
      stroke(0);
      fill(0);
      Vector pos = clicked.labelPosition();
      text(clicked.label(), pos.x, pos.y);
    }
  }
  
 
  
  void mouseMoved()
  {
     Vector mouse = new Vector(mouseX, mouseY);
     boolean hitObject = false;
     mouseOvered = null;
     for (Object o: movableObjects)
     {
         IMovable sceneObj = (IMovable)o;
         if(sceneObj.isMovable())
        { 
           boolean test = sceneObj.isInside(mouse);
           if(test)
           {
             mouseOvered = sceneObj;
             hitObject = true;
             break;
           }
        }
     }
     
     if(hitObject)
     {
       currentCursor = MOVE;
       cursor(MOVE);
     }
     else
     {
       currentCursor = ARROW;
       cursor(ARROW);
     }
  }
  
  void mouseClicked()
  {    
    ArrayList controls = camGUI.controls;
    for(Object o : controls)
    {
      IControl c = (IControl)o;
      c.click(mouseX, mouseY, mouseButton);
    }
    
    
  }
    
  
  void mousePressed()
  {
     Vector mouse = new Vector(mouseX, mouseY);
     boolean hitObject = false;
     for (Object o: movableObjects)
     {
         IMovable sceneObj = (IMovable)o; 
         boolean test = sceneObj.isInside(mouse);
         if(test)
         {
           selected = sceneObj;
           selected.setFirstMouse(mouse);
           break;
         }         
     }
    
  }
  
  
  void mouseReleased()
  {
    selected = null;
  }
  
  void keyPressed()
  {
  }

  void mouseWheel(int delta) {
   
  }
 
  void controlEvent(ControlEvent ev)
  {
     
  }

}
