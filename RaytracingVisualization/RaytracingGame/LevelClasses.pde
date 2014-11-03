class Level1 extends Level
{
  Level1()
  {
    super();
  }
  
  void init()
  {
    w = 10;
    n = 50;
    f = 400;
    fov = 45;
  
    Vector t = new Vector(width/2 + 50, height/4);
    Vector pos = new Vector(width/2, height/4); 
    
    cam = new Camera(pos, t, n, f, fov, w, true);
    camGoal = new Camera(new Vector(random(0, width), random(0, 0.85*height)), new Vector(random(0, width), random(0, 0.85*height)), n, f, random(30,90), w, false);
    target = cam.target;
    dis = new Display(new Vector(0.1*width, 0.9*height), w, 0.8*width, "Display", true);
    disGoal = new Display(new Vector(0.1*width, 0.95*height), camGoal.rays, 0.8*width, "Goal", true);
    camGUI = new CameraGUI(cam, dis, false, false);    
    
    Circle c = new Circle(color(255,0,0), new Vector(100,100), 50, false);
    objects.add(c);
    movableObjects.add(c);
    c = new Circle(color(0,255,0), new Vector(width - 100,100), 30, false);
    objects.add(c);
    movableObjects.add(c);
    
    lights = new ArrayList();
    Light l = new Light(new Vector(width/2, height/2), color(255), false);
    lights.add(l);
    
    movableObjects.add(cam);
    movableObjects.add(target);
    movableObjects.add(l);
  }
}

class Level2 extends Level
{
  Level2()
  {
    super();
  }
  
  void init()
  {
    w = 10;
    n = 50;
    f = 400;
    fov = 45;
  
    Vector t = new Vector(width/2 + 50, height/4);
    Vector pos = new Vector(width/2, height/4); 
    
    cam = new Camera(pos, t, n, f, fov, w, true);
    camGoal = new Camera(new Vector(random(0, width), random(0, 0.85*height)), new Vector(random(0, width), random(0, 0.85*height)), n, f, random(30,90), w, false);
    target = cam.target;
    dis = new Display(new Vector(0.1*width, 0.9*height), w, 0.8*width, "Display", true);
    disGoal = new Display(new Vector(0.1*width, 0.95*height), camGoal.rays, 0.8*width, "Goal", true);
    camGUI = new CameraGUI(cam, dis, false, true);    
    
    Circle c = new Circle(color(255,0,0), new Vector(100,100), 50, false);
    objects.add(c);
    movableObjects.add(c);
    c = new Circle(color(0,255,0), new Vector(width - 100,100), 30, false);
    objects.add(c);
    movableObjects.add(c);
    
    lights = new ArrayList();
    Light l = new Light(new Vector(width/2, height/2), color(255), false);
    lights.add(l);
    
    movableObjects.add(cam);
    movableObjects.add(target);
    movableObjects.add(l);
    
    
  }
}

class Level3 extends Level
{
  Level3()
  {
    super();
  }
  
  void init()
  {
    w = 10;
    n = 50;
    f = 400;
    fov = 45;
  
    Vector t = new Vector(width/2 + 50, height/4);
    Vector pos = new Vector(width/2, height/4); 
    
    cam = new Camera(pos, t, n, f, fov, w, true);
    camGoal = new Camera(new Vector(random(0, width), random(0, 0.85*height)), new Vector(random(0, width), random(0, 0.85*height)), n, f, random(30,90), w, false);
    target = cam.target;
    dis = new Display(new Vector(0.1*width, 0.9*height), w, 0.8*width, "Display", true);
    disGoal = new Display(new Vector(0.1*width, 0.95*height), camGoal.rays, 0.8*width, "Goal", true);
    camGUI = new CameraGUI(cam, dis, true, true);    
    
    Circle c = new Circle(color(255,0,0), new Vector(100,100), 50, false);
    objects.add(c);
    movableObjects.add(c);
    c = new Circle(color(0,255,0), new Vector(width - 100,100), 30, false);
    objects.add(c);
    movableObjects.add(c);
    
    lights = new ArrayList();
    Light l = new Light(new Vector(width/2, height/2), color(255), false);
    lights.add(l);
    
    movableObjects.add(cam);
    movableObjects.add(target);
    movableObjects.add(l);
    
  }
}

class Level4 extends Level
{
  Level4()
  {
    super();
  }
  
  void init()
  {
    w = 10;
    n = 50;
    f = 400;
    fov = 45;
  
    Vector t = new Vector(width/2 + 50, height/4);
    Vector pos = new Vector(width/2, height/4); 
    
    cam = new Camera(pos, t, n, f, fov, w, true);
    camGoal = new Camera(new Vector(random(0, width), random(0, 0.85*height)), new Vector(random(0, width), random(0, 0.85*height)), n, f, random(30,90), w, false);
    target = cam.target;
    dis = new Display(new Vector(0.1*width, 0.9*height), w, 0.8*width, "Display", false);
    disGoal = new Display(new Vector(0.1*width, 0.95*height), camGoal.rays, 0.8*width, "Goal", true);
    camGUI = new CameraGUI(cam, dis, true, true);    
    
    Circle c = new Circle(color(255,0,0), new Vector(100,100), 50, false);
    objects.add(c);
    movableObjects.add(c);
    c = new Circle(color(0,255,0), new Vector(width - 100,100), 30, false);
    objects.add(c);
    movableObjects.add(c);
    
    lights = new ArrayList();
    Light l = new Light(new Vector(width/2, height/2), color(255), false);
    lights.add(l);
    
    movableObjects.add(cam);
    movableObjects.add(target);
    movableObjects.add(l);
    
  }
}

class Level5 extends Level
{
  Level5()
  {
    super();
  }
  
  void init()
  {
    w = 10;
    n = 50;
    f = 400;
    fov = 45;
  
    Vector t = new Vector(width/2 + 50, height/4);
    Vector pos = new Vector(width/2, height/4); 
    
    cam = new Camera(pos, t, n, f, fov, w, false);
    camGoal = new Camera(new Vector(random(0, width), random(0, 0.85*height)), new Vector(random(0, width), random(0, 0.85*height)), n, f, random(30,90), w, false);
    target = cam.target;
    dis = new Display(new Vector(0.1*width, 0.9*height), w, 0.8*width, "Display", false);
    disGoal = new Display(new Vector(0.1*width, 0.95*height), camGoal.rays, 0.8*width, "Goal", true);
    camGUI = new CameraGUI(cam, dis, false, false);    
    
    Circle c = new Circle(color(255,0,0), new Vector(100,100), 50, true);
    objects.add(c);
    movableObjects.add(c);
    c = new Circle(color(0,255,0), new Vector(width - 100,100), 30, true);
    objects.add(c);
    movableObjects.add(c);
    
    lights = new ArrayList();
    Light l = new Light(new Vector(width/2, height/2), color(255), true);
    lights.add(l);
    
    movableObjects.add(cam);
    movableObjects.add(target);
    movableObjects.add(l);
    
  }
}

class Level6 extends Level
{
  Level6()
  {
    super();
  }
  
  void init()
  {
    showRays = false;
    showShadowRays = false;
    showReflectedRays = false;
  
    w = 10;
    n = 50;
    f = 400;
    fov = 45;
  
    Vector t = new Vector(width/2 + 50, height/4);
    Vector pos = new Vector(width/2, height/4); 
    
    cam = new Camera(pos, t, n, f, fov, w, true);
    camGoal = new Camera(new Vector(random(0, width), random(0, 0.85*height)), new Vector(random(0, width), random(0, 0.85*height)), n, f, random(30,90), w, false);
    target = cam.target;
    dis = new Display(new Vector(0.1*width, 0.9*height), w, 0.8*width, "Display", false);
    disGoal = new Display(new Vector(0.1*width, 0.95*height), camGoal.rays, 0.8*width, "Goal", true);
    camGUI = new CameraGUI(cam, dis, true, true);    
    
    Circle c = new Circle(color(255,0,0), new Vector(100,100), 50, true);
    objects.add(c);
    movableObjects.add(c);
    c = new Circle(color(0,255,0), new Vector(width - 100,100), 30, true);
    objects.add(c);
    movableObjects.add(c);
    
    lights = new ArrayList();
    Light l = new Light(new Vector(width/2, height/2), color(255), true);
    lights.add(l);
    
    movableObjects.add(cam);
    movableObjects.add(target);
    movableObjects.add(l);
    
  }
}
