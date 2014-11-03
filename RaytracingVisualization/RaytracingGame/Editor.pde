class Editor extends RaytracingLevel
{
  Editor()
  {
    super();
  }
  
  void init()
  {
    w = 16;
    n = 50;
    f = 400;
    fov = 45;
  
    Vector t = new Vector(width/2 + 50, height/4);
    Vector pos = new Vector(width/2, height/4); 
    
    cam = new Camera(pos, t, n, f, fov, w, true);
    //camGoal = new Camera(pos, t, n, f, fov, w, true);
    target = cam.target;
    dis = new Display(new Vector(0.1*width, 0.9*height), w, 0.8*width, "Goal", true);
    //disGoal = new Display(new Vector(0.1*width, 0.95*height), cam.rays, 0.8*width, "Goal", true);
    camGUI = new CameraGUI(cam, dis, true, true);   
   
    lights = new ArrayList();    
    
    movableObjects.add(cam);
    movableObjects.add(target);
  }
  
  void save(String fileName)
  {
    String[] list = new String[objects.size() + lights.size() + 1];
    int i = 0;
    for(i=0; i<objects.size(); i++)
      list[i] = ((SceneObject)objects.get(i)).save();
    for(int j=0; j<lights.size(); i++, j++)
      list[i] = ((Light)lights.get(j)).save();
    list[i] = cam.save();
    saveStrings(fileName, list);
  }
  
  void load(String fileName)
  {     
     reset();
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
       else if(splitted[0].equals("camera"))
       {
         cam.load(list[i]);
         target = cam.target;
         dis = new Display(new Vector(0.1*width, 0.9*height), cam.rays, 0.8*width, "Goal", true);
    
       }
       else if(splitted[0].equals("light"))
       {
         Light l = new Light();
         l.load(list[i]);
         lights.add(l);
         if(l.isMovable)
           movableObjects.add(l);
       }
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
    
    if(mouseButton == RIGHT)
    {
      Circle c = new Circle(color(255,0,0), new Vector(mouseX,mouseY), 50, true);
      objects.add(c);
      movableObjects.add(c);
    }
    else if(mouseButton == CENTER)
    {
      Light l = new Light(new Vector(mouseX,mouseY), color(255), true);
      lights.add(l);
      movableObjects.add(l);
    }
  }
  
  void keyPressed()
  {
    if(key == 'w')
    {
      if(selected != null)
      {
        Circle c = (Circle)selected;
        c.radius += 10;
      }
    }
    else if(key == 's')
    {
      if(selected != null)
      {
        Circle c = (Circle)selected;
        if(c.radius > 10)
          c.radius -= 10;
      }
    }
   else if(key == 'r')
    {
      if(selected != null)
      {
        Circle c = (Circle)selected;
        c.diffuse = color(255,0,0);
      }
    }
   else if(key == 'g')
    {
      if(selected != null)
      {
        Circle c = (Circle)selected;
        c.diffuse = color(0,255,0);
      }
    }
   else if(key == 'b')
    {
      if(selected != null)
      {
        Circle c = (Circle)selected;
        c.diffuse = color(0,0,255);
      }
    }
   else if(key == 'v')
    {
      if(selected != null)
      {
        Circle c = (Circle)selected;
        c.diffuse = color(255,0,255);
      }
    }
   else if(key == 'c')
    {
      if(selected != null)
      {
        Circle c = (Circle)selected;
        c.diffuse = color(0,255,255);
      }
    } 
    
    else if(key == 'o')
    {
      save("levelX.txt");
    }
    else if(key == 'p')
    {
      load("levelX.txt");
    }
    else if(key == 'q')
    {
      reset();
    } 
  }
}
