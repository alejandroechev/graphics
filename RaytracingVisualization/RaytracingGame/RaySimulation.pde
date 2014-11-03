class RaySimulation extends RaytracingLevel
{
  float deltaSize = 5;
  float attenuationFactor = 2;
  
  ControlP5 controlP5;
  ColorPicker cp, cpSpec, cpAmbient, cpLight, lineColor;
  controlP5.Button eyeRaysButton, shadowRaysButton, reflectedRaysButton, normalsButton, halfsButton, labelsButton, 
                   protractorButton, linesButton, eraseButton, rulerButton, eraseAllButton, displayButton, cameraElementsButton, imagePlaneButton;
  Slider shininessSlider, reflectivenessSlider, attenLinearSlider, attenQuadSlider, radiusSlider;
  
  PApplet app;
  
  //Tools
  boolean showProtractor = false, showRuler = false, drawLines = false, eraseLine = false;
  Protractor protractor;
  ArrayList lines;
  Line currentLine;
  Eraser eraser;
  Ruler ruler;
  
  PFont font;
  
  //Parameters
  boolean shadingAvailable = true, circleAvailable = true, optionsAvailable = true, toolsAvailable = true;
  
  RaySimulation(PApplet _app)
  {
    super();
    app = _app;
    lights = new ArrayList();   
    
  }
  
  void init()
  {
    movableObjects.clear();
    objects.clear();
    lights.clear();
    if(controlP5 != null)
      controlP5.dispose();
    
    font = loadFont("Calibri-48.vlw"); 
    textFont(font, 11); 
    
    setOptions();    
    setParameters();
    
    initDisplay();
    initCamera();
    initGUI();    
    initObjects();    
    initTools();
    
  }
  
  void setOptions()
  {
    showRays = true;   
    showCameraElements = true;
  }
  
  void setParameters()
  {
    w = 16;
    n = 75;
    f = 400;
    fov = 45;
  }
    
  void initCamera()
  {
    Vector t = new Vector(width/2, 3*height/4 - 100);
    Vector pos = new Vector(width/2, 3*height/4); 
    
    cam = new Camera(pos, t, n, f, fov, w, true);
    cam.showNearClip = true;
    target = cam.target;
    camGUI = new CameraGUI(cam, dis, true, true);   
    
    movableObjects.add(cam);
    movableObjects.add(target);
  }
  
  void initDisplay()
  {
    dis = new Display(new Vector(0.1*width, 0.95*height), w, 0.8*width, AppStrings.getString("display"), true);
  }
  
  void initObjects()
  {
    addCircle();
    addLight();
  }
  
  void initTools()
  {
    addProtractor();
    addRuler();
    lines = new ArrayList();
    eraser = new Eraser(new Vector(-100,-100), 12);
  }
  
  void initGUI()
  {
    controlP5 = new ControlP5(app);
    /*PFont font;
    font = loadFont("Calibri-48.vlw");
    controlP5.setControlFont(font,11);*/
    
   
    Textlabel myTextlabelA;
    
    int circleY = 10;//instructionsY + 25;
    
      
    if(circleAvailable)
    {
      myTextlabelA = controlP5.addTextlabel("label83",AppStrings.getString("circle") + ":",10,circleY);
      myTextlabelA.setColorValue(color(0));
      
      myTextlabelA = controlP5.addTextlabel("label100",AppStrings.getString("radius") + ":",10,circleY+=15);
      myTextlabelA.setColorValue(color(100));
      
      radiusSlider = controlP5.addSlider("rs",5,100,40,10,circleY += 10,100,10);
      radiusSlider.setColorBackground(color(150));
      radiusSlider.setColorForeground(color(0));
    }
    
    if(shadingAvailable)
    {
      myTextlabelA = controlP5.addTextlabel("label10",AppStrings.getString("shininess") + ":",10,circleY += 15);
      myTextlabelA.setColorValue(color(100));
      
      shininessSlider = controlP5.addSlider("ss",1,1000,1000,10,circleY += 10,100,10);
      shininessSlider.setColorBackground(color(150));
      shininessSlider.setColorForeground(color(0));
      
      myTextlabelA = controlP5.addTextlabel("label1023",AppStrings.getString("reflectiveness") + ":",10,circleY += 15);
      myTextlabelA.setColorValue(color(100));
      
      reflectivenessSlider = controlP5.addSlider("refs",0,1,0,10,circleY += 10,100,10);
      reflectivenessSlider.setColorBackground(color(150));
      reflectivenessSlider.setColorForeground(color(0));
      
      myTextlabelA = controlP5.addTextlabel("label9",AppStrings.getString("diffuse") + ":",10,circleY += 15);
      myTextlabelA.setColorValue(color(100));
      
      cp = controlP5.addColorPicker("picker",10,circleY += 10,100,10);
      cp.setColorValue(color(255,0,0));
      
      myTextlabelA = controlP5.addTextlabel("label91",AppStrings.getString("specular") + ":",10,circleY += 65);
      myTextlabelA.setColorValue(color(100));
      
      cpSpec = controlP5.addColorPicker("pickerSpec",10,circleY += 10,100,10);
      cpSpec.setColorValue(color(0,0,0));
      
      
    }
    
    if(circleAvailable)
    {
      controlP5.Button circleButton = controlP5.addButton(AppStrings.getString("addCircle"),128,10,circleY += 75,100,20);
      circleButton.setColorForeground(color(0));
      circleButton.setColorBackground(color(150));
    }
    
    int lightY = circleY + 35;
    
    if(shadingAvailable)
    {
      myTextlabelA = controlP5.addTextlabel("label112",AppStrings.getString("light") + ":",10,lightY);
      myTextlabelA.setColorValue(color(0));
      
      myTextlabelA = controlP5.addTextlabel("label113",AppStrings.getString("lightColor") + ":",10,lightY += 15);
      myTextlabelA.setColorValue(color(100));
      
      cpLight = controlP5.addColorPicker("pickerLightColor",10,lightY += 10,100,10);
      cpLight.setColorValue(color(255,255,255));
      
      myTextlabelA = controlP5.addTextlabel("label113",AppStrings.getString("attenuationLinear") + ":",10,lightY += 65);
      myTextlabelA.setColorValue(color(100));
      attenLinearSlider = controlP5.addSlider("als",0,0.01,0,10,lightY += 10,100,10);
      attenLinearSlider.setColorBackground(color(150));
      attenLinearSlider.setColorForeground(color(0));
      
      myTextlabelA = controlP5.addTextlabel("label114",AppStrings.getString("attenuationQuad") + ":",10,lightY += 15);
      myTextlabelA.setColorValue(color(100));
      attenQuadSlider = controlP5.addSlider("aqs",0,0.00001,0,10,lightY += 10,100,10);
      attenQuadSlider.setColorBackground(color(150));
      attenQuadSlider.setColorForeground(color(0));
      
      controlP5.Button lightButton = controlP5.addButton(AppStrings.getString("addLight"),128,10,lightY +=20,100,20);
      lightButton.setColorForeground(color(0));
      lightButton.setColorBackground(color(150));
    }
    
    if(circleAvailable)
    {
      controlP5.Button delButton = controlP5.addButton(AppStrings.getString("delete"),128,10,lightY += 40,100,20);
      delButton.setColorForeground(color(0));
      delButton.setColorBackground(color(150));
    }
    
    int sceneY = 10;//instructionsY + 25;
    int toggleSectionX = width - 110;
    
    
    myTextlabelA = controlP5.addTextlabel("label83",AppStrings.getString("scene") + ":",toggleSectionX,sceneY);
    myTextlabelA.setColorValue(color(0));
      
    myTextlabelA = controlP5.addTextlabel("label92",AppStrings.getString("ambient") + ":",toggleSectionX,sceneY += 15);
    myTextlabelA.setColorValue(color(100));
      
    cpAmbient = controlP5.addColorPicker("pickerAmbient",toggleSectionX,sceneY += 10,100,10);
    cpAmbient.setColorValue(ambient);
    
    //Options
    int toggleSectionY = sceneY + 65;
    
    if(optionsAvailable)
    {
      myTextlabelA = controlP5.addTextlabel("label8",AppStrings.getString("options") + ":",toggleSectionX,toggleSectionY);
      myTextlabelA.setColorValue(color(0));
      
      eyeRaysButton = controlP5.addButton(AppStrings.getString("showEyeRays"),128,toggleSectionX,toggleSectionY += 15,100,20);
      eyeRaysButton.setColorForeground(color(0));
      eyeRaysButton.setColorBackground(color(150));
      eyeRaysButton.setColorActive(color(0,128,0));
      eyeRaysButton.setSwitch(true);
      if(showRays)
        eyeRaysButton.setOn();
      
      shadowRaysButton = controlP5.addButton(AppStrings.getString("showShadowRays"),128,toggleSectionX,toggleSectionY += 25,100,20);
      shadowRaysButton.setColorForeground(color(0));
      shadowRaysButton.setColorBackground(color(150));
      shadowRaysButton.setColorActive(color(0,128,0));
      shadowRaysButton.setSwitch(true);
      if(showShadowRays)
        shadowRaysButton.setOn();
        
      reflectedRaysButton = controlP5.addButton(AppStrings.getString("showReflectedRays"),128,toggleSectionX,toggleSectionY += 25,100,20);
      reflectedRaysButton.setColorForeground(color(0));
      reflectedRaysButton.setColorBackground(color(150));
      reflectedRaysButton.setColorActive(color(0,128,0));
      reflectedRaysButton.setSwitch(true);
      if(showReflectedRays)
        reflectedRaysButton.setOn();
      
      normalsButton = controlP5.addButton(AppStrings.getString("showNormals"),128,toggleSectionX,toggleSectionY += 25,100,20);
      normalsButton.setColorForeground(color(0));
      normalsButton.setColorBackground(color(150));
      normalsButton.setColorActive(color(0,128,0));
      normalsButton.setSwitch(true);
      if(showNormals)
        normalsButton.setOn();
      
      halfsButton = controlP5.addButton(AppStrings.getString("showHalfVectors"),128,toggleSectionX,toggleSectionY += 25,100,20);
      halfsButton.setColorForeground(color(0));
      halfsButton.setColorBackground(color(150));
      halfsButton.setColorActive(color(0,128,0));
      halfsButton.setSwitch(true);
      if(showHalfVectors)
        halfsButton.setOn();
        
      displayButton = controlP5.addButton(AppStrings.getString("showDisplay"),128,toggleSectionX,toggleSectionY += 25,100,20);
      displayButton.setColorForeground(color(0));
      displayButton.setColorBackground(color(150));
      displayButton.setColorActive(color(0,128,0));
      displayButton.setSwitch(true);
      if(dis.showable)
        displayButton.setOn();
      
      imagePlaneButton = controlP5.addButton(AppStrings.getString("showImagePlane"),128,toggleSectionX,toggleSectionY += 25,100,20);
      imagePlaneButton.setColorForeground(color(0));
      imagePlaneButton.setColorBackground(color(150));
      imagePlaneButton.setColorActive(color(0,128,0));
      imagePlaneButton.setSwitch(true);
      if(cam.showNearClip)
        imagePlaneButton.setOn();
        
      cameraElementsButton = controlP5.addButton(AppStrings.getString("showCameraGUI"),128,toggleSectionX,toggleSectionY += 25,100,20);
      cameraElementsButton.setColorForeground(color(0));
      cameraElementsButton.setColorBackground(color(150));
      cameraElementsButton.setColorActive(color(0,128,0));
      cameraElementsButton.setSwitch(true);
      if(showCameraElements)
        cameraElementsButton.setOn();
    } 
    //Tools
    int toolsSectionX = width - 110;
    int toolsSectionY = toggleSectionY + 30;
    
    if(toolsAvailable)
    {
      myTextlabelA = controlP5.addTextlabel("label8w",AppStrings.getString("tools") + ":",toolsSectionX,toolsSectionY);
      myTextlabelA.setColorValue(color(0));
      
      protractorButton = controlP5.addButton(AppStrings.getString("protractor"),128,toolsSectionX,toolsSectionY += 15,100,20);
      protractorButton.setColorForeground(color(0));
      protractorButton.setColorBackground(color(150));
      protractorButton.setColorActive(color(0,128,0));
      protractorButton.setSwitch(true);
      if(showProtractor)
        protractorButton.setOn();
        
      rulerButton = controlP5.addButton(AppStrings.getString("ruler"),128,toolsSectionX,toolsSectionY += 25,100,20);
      rulerButton.setColorForeground(color(0));
      rulerButton.setColorBackground(color(150));
      rulerButton.setColorActive(color(0,128,0));
      rulerButton.setSwitch(true);
      if(showRuler)
        rulerButton.setOn();  
        
      myTextlabelA = controlP5.addTextlabel("labelDD",AppStrings.getString("lineColor") + ":",toolsSectionX,toolsSectionY += 25);
      myTextlabelA.setColorValue(color(100));
      
      lineColor = controlP5.addColorPicker("pickerL",toolsSectionX,toolsSectionY += 10,100,10);
      lineColor.setColorValue(color(100));
      
      linesButton = controlP5.addButton(AppStrings.getString("lines"),128,toolsSectionX,toolsSectionY += 65,100,20);
      linesButton.setColorForeground(color(0));
      linesButton.setColorBackground(color(150));
      linesButton.setColorActive(color(0,128,0));
      linesButton.setSwitch(true);
      if(drawLines)
        linesButton.setOn();
        
      eraseButton = controlP5.addButton(AppStrings.getString("eraser"),128,toolsSectionX,toolsSectionY += 25,100,20);
      eraseButton.setColorForeground(color(0));
      eraseButton.setColorBackground(color(150));
      eraseButton.setColorActive(color(0,128,0));
      eraseButton.setSwitch(true);
      if(eraseLine)
        eraseButton.setOn();  
        
      eraseAllButton = controlP5.addButton(AppStrings.getString("eraseAll"),128,toolsSectionX,toolsSectionY += 25,100,20);
      eraseAllButton.setColorForeground(color(0));
      eraseAllButton.setColorBackground(color(150));
      eraseAllButton.setColorActive(color(128,0,0));
    }
  }
  
  void update()
  {
    super.update();
    
    protractor.update();
    ruler.update();
    
    ambient = cpAmbient.getColorValue();
    
    if(optionsAvailable)
    {
      showRays = eyeRaysButton.booleanValue();
      showShadowRays = shadowRaysButton.booleanValue();
      showNormals = normalsButton.booleanValue();
      showHalfVectors = halfsButton.booleanValue();
      showReflectedRays = reflectedRaysButton.booleanValue();
      dis.showable = displayButton.booleanValue();
      showCameraElements = cameraElementsButton.booleanValue();
      cam.showNearClip = imagePlaneButton.booleanValue();
      //showImagePlane = imagePlaneButton.booleanValue();
      //cam.showNearClipPosition = cameraElementsButton.booleanValue();
    }
    
    if(toolsAvailable)
    {
      showProtractor = protractorButton.booleanValue();
      protractor.isEnabled = showProtractor;
      showRuler = rulerButton.booleanValue();
      ruler.isEnabled = showRuler;
      drawLines = linesButton.booleanValue();
      eraseLine = eraseButton.booleanValue();
      eraser.isEnabled = eraseLine;
      
      if(eraseLine)
      {
        linesButton.setOff();
        currentLine = null;
         noCursor();
      }
      else
         cursor(currentCursor);
      //showLabels = labelsButton.booleanValue();
    }
    
    
    if(clicked != null)
    {
      if(clicked instanceof Circle)
      {
        Circle c = (Circle)clicked;
        if(circleAvailable)
          c.radius = radiusSlider.getValue();
        if(shadingAvailable)
        {
          c.diffuse = cp.getColorValue();
          c.specular = cpSpec.getColorValue();
          //c.ambient = cpAmbient.getColorValue();
          c.shininess = shininessSlider.getValue();
          c.reflectiveness = reflectivenessSlider.getValue();
        }
      }
      else if(clicked instanceof Light)
      {
        if(shadingAvailable)
        {
          Light l = (Light)clicked;
          l.col = cpLight.getColorValue();
          l.atenuationLinear = attenLinearSlider.getValue();
          l.atenuationQuadratic = attenQuadSlider.getValue();
        }
      }
    }
    
    if(currentLine != null && !eraseLine)
    {
      currentLine.v2 = new Vector(mouseX, mouseY); 
    }
  } 
  
  void draw()
  {
    super.draw();
    protractor.draw();
    textFont(font, 7);
    ruler.draw();
    textFont(font, 11);
    if(currentLine != null)
      currentLine.draw();
    for(Object o : lines)
    {
      Line l = (Line)o;
      l.draw(); 
    }
    eraser.draw();
  }

  void mousePressed()
  {
    if(drawLines && !eraseLine)
    {
      Line l = new Line(new Vector(mouseX, mouseY), new Vector(mouseX, mouseY), lineColor.getColorValue());
      currentLine = l; 
    }
    else if(!eraseLine)
    {
      super.mousePressed();
      if(mouseX > 110 && mouseX < width-100)
      {
         Vector mouse = new Vector(mouseX, mouseY);
         boolean hitObject = false;
         for (Object o: movableObjects)
         {
           IMovable sceneObj = (IMovable)o; 
           boolean test = sceneObj.isInside(mouse);
           if(test)
           {
             if(clicked != null)
               clicked.flipSelected();
             clicked = sceneObj;
             clicked.flipSelected(); 
             hitObject = true;      
             
             if(clicked instanceof Circle)
             {
               Circle c = (Circle)clicked;
               if(circleAvailable)
                 radiusSlider.setValue(c.radius);
               if(shadingAvailable)
               {
                 //cpAmbient.setColorValue(c.ambient);
                 cp.setColorValue(c.diffuse);
                 cpSpec.setColorValue(c.specular);
                 shininessSlider.setValue(c.shininess);
                 reflectivenessSlider.setValue(c.reflectiveness);
               }
             }   
             else if(clicked instanceof Light)
             {
               if(shadingAvailable)
               {
                 Light l = (Light)clicked;
                 cpLight.setColorValue(l.col);
                 attenLinearSlider.setValue(l.atenuationLinear);
                 attenQuadSlider.setValue(l.atenuationQuadratic);
               }
             } 
             break;
           }         
         }
         if(!hitObject && clicked != null)
       {      
         clicked.flipSelected();
         clicked = null;
       } 
      }
     
    }
  }
  
  void mouseMoved()
  {
    if(!drawLines && !eraseLine)
    {      
      super.mouseMoved();
    }
    else if(eraseLine)
    {
      eraser.center = new Vector(mouseX, mouseY);
    }
  }
  
  void mouseReleased()
  {
    if(drawLines && !eraseLine)
    {
      if(currentLine != null)
      {
        currentLine.v2 = new Vector(mouseX, mouseY);
        lines.add(currentLine);        
        currentLine = null;
      } 
    }
    else if(!eraseLine)
    {
      super.mouseReleased();
    }
  }
  
  
  void mouseClicked()
  {
    if(!drawLines && !eraseLine)
    {
      if(camGUI != null)
      {
        ArrayList controls = camGUI.controls;
      
        for(Object o : controls)
        {
          IControl c = (IControl)o;
          c.click(mouseX, mouseY, mouseButton);
        }
      }      
    }
    else if(eraseLine)
    {
      Line lineToRemove =  null;
      for(Object o : lines)
      {
        Line l = (Line)o;
        Ray r = l.getRay();
        float intercept = eraser.intersect(l.getRay());
        if(intercept > 0 && intercept < 1){
          lineToRemove = l;
          break;
        }
      }
      if(lineToRemove != null)
        lines.remove(lineToRemove);
    }
    
  }
  
  
  void keyPressed()
  {    
    if(key == DELETE)
    {
      delete();  
    }
    else if(keyCode == UP)
    {
      move(0,-1);
    }
    else if(keyCode == DOWN)
    {
      move(0,1);
    }
    else if(keyCode == LEFT)
    {
      move(-1,0);
    }
    else if(keyCode == RIGHT)
    {
      move(1,0);
    }
  }
  
  void move(float x, float y)
  {
    if(clicked != null)
    {
      Vector p = clicked.getPosition();
      clicked.setFirstMouse(p);
      p = Vector.Add(p, new Vector(x,y));
      clicked.setPosition(p);
    }
  }
  
  void delete()
  {
    if(clicked != null && clicked instanceof Circle)
      {
        objects.remove(clicked);
        movableObjects.remove(clicked);
      }
      
      if(clicked != null && clicked instanceof Light)
      {
        lights.remove(clicked);
        movableObjects.remove(clicked);
      }
    
  }
  
  void addCircle()
  {
      Circle c = new Circle(cp.getColorValue(), new Vector(width/2,height/4), radiusSlider.getValue(), true);
      c.specular = cpSpec.getColorValue();
      c.ambient = cpAmbient.getColorValue();
      c.shininess = shininessSlider.getValue();
      c.reflectiveness = reflectivenessSlider.getValue();
      objects.add(c);
      movableObjects.add(c);
  }
  
   void addLight()
  {
    Light l = new Light(new Vector(width/4,height/2), cpLight.getColorValue(), true);
      l.atenuationLinear = attenLinearSlider.getValue();
      l.atenuationQuadratic = attenQuadSlider.getValue();
      lights.add(l);
      movableObjects.add(l);
  }
  
  void addProtractor()
  {
    protractor = new Protractor(new Vector(3*width/4,3*height/4), new Vector(3*width/4,3*height/4 - 200));
    movableObjects.add(protractor);
    movableObjects.add(protractor.target);
  }
  
  void addRuler()
  {
    ruler = new Ruler(new Vector(3*width/4,height/4), new Vector(3*width/4,height/4 - 120));
    movableObjects.add(ruler);
    movableObjects.add(ruler.target);
  }
  
  
  void controlEvent(ControlEvent ev)
  {
    if(ev.controller().name().equals(AppStrings.getString("addCircle")))
    {
      addCircle();
    } 
   else if(ev.controller().name().equals(AppStrings.getString("addLight")))
    {
      addLight();
    }
    else if(ev.controller().name().equals(AppStrings.getString("delete")))
    {
      delete();
    }
    else if(ev.controller().name().equals(AppStrings.getString("eraseAll")))
    {
      lines.clear();
    }
    
  }


}
