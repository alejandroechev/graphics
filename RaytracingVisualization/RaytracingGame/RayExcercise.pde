class RayExcercise extends RaySimulation
{
  int numObjects = 12;
  Display clickableDisplay;
  Display objectiveDisplay;
  Camera cameraObjective;
  
  int raysObjective;
  int fovObjective;
  int upObjective;
  Vector targetObjective;
  Vector positionObjective;
  
  Vector t, pos;
  
  int scaffold;
  int iterations = 0;
  
  ArrayList raysRanges;
  
  Textlabel feedbackLabelCorrect, feedbackLabelIncorrect;
  
  RayExcercise(PApplet _app, int _scaffold)
  {
    super(_app);
    scaffold = _scaffold;
    
    shadingAvailable = false;
    circleAvailable = false;
    optionsAvailable = false;
    toolsAvailable = scaffold != 0;
    
    raysRanges = new ArrayList();
    raysRanges.add(new int[]{10,20});
    raysRanges.add(new int[]{6, 12});
    raysRanges.add(new int[]{4, 8});
  }
  
  void setOptions()
  {
    showRays = scaffold == 0;
    showCameraElements = true;
  }
  
  void setParameters()
  {
    w = 16;
    n = 75;
    f = 10000;
    fov = 60;
    t = new Vector(width/2, height/2 - 100);
    pos = new Vector(width/2, height/2); 
    
    int[] raysRange = (int[])raysRanges.get(scaffold);
    raysObjective = int(random(raysRange[0],raysRange[1]));
    fovObjective = int(random(40,80));
    fovObjective = fovObjective % 2 == 0 ? fovObjective : fovObjective + 1; 
    float r = random(0.05*width, 0.05*width);
    float theta = random(0, 2*PI);
    float x = r*cos(theta) + 0.5*width;
    float y = r*sin(theta) + 0.4*height;
    positionObjective = new Vector(int(x), int(y)); 
    r = random(0.15*width, 0.15*width);
    theta = random(0, 2*PI);
    x = r*cos(theta) + 0.5*width;
    y = r*sin(theta) + 0.4*height;
    targetObjective = new Vector(int(x), int(y));
    upObjective = random(0,1) < 0.5 ? 1 : -1;
  }
    
  void initCamera()
  {
    
    cam = new Camera(pos, t, n, f, fov, w, true);
    cam.showNearClip = scaffold != 2;
    target = cam.target;
    camGUI = new CameraGUI(cam, clickableDisplay, scaffold != 2, true);   
    
    cameraObjective = new Camera(positionObjective, targetObjective, n, f, fovObjective, raysObjective, false); 
    cameraObjective.up = upObjective;
    
    movableObjects.add(cam);
    movableObjects.add(target);   
  }
  
  void initDisplay()
  {
    objectiveDisplay = new Display(new Vector(0.1*width, 0.85*height), raysObjective, 0.8*width, "Goal", true);
    clickableDisplay = new Display(new Vector(0.1*width, 0.9*height), w, 0.8*width, "Display", true);
    clickableDisplay.clickable = true;
  }

  
  void initObjects()
  {
      for(int i=0; i<numObjects; i++)
      {
        Circle c = new Circle(color(0), new Vector(0, 0), 1, false);
        objects.add(c);
        movableObjects.add(c);
      }
  }
  
  void randomizeObjects()
  {
    float angle = 0;
    float slice = 2*PI/objects.size();
    for(Object o : objects)
    {
      Circle c = (Circle)o;
      float a = random(0.3*width, 0.3*width);
      float b = random(0.3*height, 0.3*height);
      float theta = angle + random(0, slice);
      float x = a*cos(theta) + 0.5*width;
      float y = b*sin(theta) + 0.4*height;
      float radius = random(5, 25);
      c.center = new Vector(x,y);
      c.radius = radius;
      angle += slice;
    }
  }
  
  void initGUI()
  {
    println("0");
    super.initGUI();
    
    int instructionsY = 10;
    Textlabel label = controlP5.addTextlabel("labelI-1",AppStrings.getString("activity") + ":" + (scaffold+1),10,instructionsY += 15);
    label.setColorValue(color(0));
    
    label = controlP5.addTextlabel("labelI0",AppStrings.getString("iterations") +":" + (iterations),10,instructionsY += 15);
    label.setColorValue(color(0));
    
    label = controlP5.addTextlabel("labelI1",AppStrings.getString("instructions")+":",10,instructionsY += 25);
    label.setColorValue(color(0));
    
    label = controlP5.addTextlabel("labelI2",AppStrings.getString("rays") +": " + raysObjective,10,instructionsY += 15);
    label.setColorValue(color(100));
    
    label = controlP5.addTextlabel("labelI3",AppStrings.getString("fov") + ": "+ fovObjective,10,instructionsY += 15);
    label.setColorValue(color(100));
    
    label = controlP5.addTextlabel("labelI4",AppStrings.getString("up") + ": " + upObjective,10,instructionsY += 15);
    label.setColorValue(color(100));
    
    label = controlP5.addTextlabel("labelI5",AppStrings.getString("position") + ": "  + new Vector(positionObjective.x, height - positionObjective.y).ToString2D(),10,instructionsY += 15);
    label.setColorValue(color(100));
    
    label = controlP5.addTextlabel("labelI6",AppStrings.getString("target") + ": " + new Vector(targetObjective.x, height - targetObjective.y).ToString2D(),10,instructionsY += 15);
    label.setColorValue(color(100));
    
    controlP5.Button checkButton = controlP5.addButton(AppStrings.getString("submit"),128,int(0.1*width),int(0.95*height),80,20);
    checkButton.setColorForeground(color(0));
    checkButton.setColorBackground(color(0,150,0));
    
    controlP5.Button nextButton = controlP5.addButton(AppStrings.getString("next"),128,int(0.8*width),int(0.95*height),80,20);
    nextButton.setColorForeground(color(0));
    nextButton.setColorBackground(color(150));
    nextButton.hide();
    
    feedbackLabelCorrect = controlP5.addTextlabel("feedbackLabelCorrect",AppStrings.getString("correct"),int(0.1*width) + 100,int(0.95*height) + 5);
    feedbackLabelCorrect.setColorValue(color(0,120,0));
    feedbackLabelCorrect.setVisible(false);
    
    feedbackLabelIncorrect = controlP5.addTextlabel("feedbackLabelIncorrect",AppStrings.getString("incorrect"),int(0.1*width) + 100,int(0.95*height) + 5);
    feedbackLabelIncorrect.setColorValue(color(120,0,0));
    feedbackLabelIncorrect.setVisible(false);
  
  }
  
  void init()
  {
    super.init();
    int t = 0;
    while(!objectiveDisplay.hasColor() && t < 10)
    { 
      randomizeObjects();  
      for(int i=0; i<cameraObjective.rays; i++)
      {
        color c = calculatePixel(cameraObjective, i, true);
        objectiveDisplay.pixelValues[i] = c;
      }
      t++;
    }
  }
  
  void dispose()
  {
    controlP5.isApplet = false;
    controlP5.dispose();
  }
  
  void draw()
  {
    super.draw();
    clickableDisplay.draw();
  }
  
  void update()
  {
    /*if(selected != null && selected.isMovable())
    {
      Vector mouse = new Vector(mouseX, mouseY);
      selected.setPosition(mouse);  
    }
    if(camGUI != null)
      camGUI.updatePositions(cam.position);
    cam.update();
    
    directRays.clear();
    shadowRays.clear();
    reflectedRays.clear();
    normals.clear();
    halfVectors.clear();
    
    for(int i=0; i<cam.rays; i++)
    {
      color c = calculatePixel(cam, i, true);
    }*/
    
    super.update();
    
  }
  
  void mouseClicked()
  {
    super.mouseClicked();
    clickableDisplay.mouseClicked();
  }
  
  boolean check()
  {
    if(cam.rays != cameraObjective.rays)
      return false;
      
    for(int i=0; i<cam.rays; i++)
    {
      if(objectiveDisplay.pixelValues[i] != clickableDisplay.pixelValues[i])
        return false;
    }
    return true;
  }
  
  void controlEvent(ControlEvent ev)
  {
    super.controlEvent(ev);
    if(ev.controller().name().equals(AppStrings.getString("submit")))
    {
      if(check())
      {
        feedbackLabelIncorrect.hide();
        feedbackLabelCorrect.show();
        controlP5.controller(AppStrings.getString("next")).show();
      }
      else
      {
        feedbackLabelCorrect.hide();
        feedbackLabelIncorrect.show();
      }
    }
    else if (ev.controller().name().equals(AppStrings.getString("next")))
    {
      iterations++;
      dispose();
      init();
    }
  }
}
