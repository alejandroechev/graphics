/* Interavtive visualization of the rendering process of a basic Graphics Pipeline
*/
class PipelineLevel extends Level
{  
  //GUI Elements
  ControlP5 controlP5;
  ColorPicker cp, cpSpec, cpAmbient;
  controlP5.Button eyeRaysButton, shadowRaysButton, reflectedRaysButton, normalsButton, halfsButton, labelsButton;
  Slider shininessSlider, reflectivenessSlider, attenLinearSlider, attenQuadSlider, radiusSlider;
  controlP5.Button[] stagesButtonArray;
  
  //Parent control
  PApplet app;
  
  //World coordinate system
  CoordinateDisplay worldCoords;
  boolean showWorldCoords = false;
  
  //Current pipeline state
  int currentStage = PipelineStages.None;
  CoordinateDisplay currentCoords = null;
  
  //Object list for perspective correction
  ArrayList secondaryObjects;
  
  PipelineLevel(PApplet _app)
  {
    super();
    app = _app;
    worldCoords = new CoordinateDisplay(new Vector(150, 10), new Vector(1, 0), new Vector(0, 1), 100,100);
    worldCoords.showMinusAxis = false;
    secondaryObjects = new ArrayList();
  }
  
  void init()
  {
    w = 16;
    n = 80;
    f = 350;
    fov = 45;
  
    Vector t = new Vector(width/2, 3*height/4 - 100);
    Vector pos = new Vector(width/2, 3*height/4); 
    
    cam = new Camera(pos, t, n, f, fov, w, true);
    cam.showFrustrum = true;
    cam.showFrustrumModPerspect = true;
    cam.drawPixels = false;
    target = cam.target;
    
    dis = new Display(new Vector(0.1*width, 0.9*height), w, 0.8*width, "Display", true);
    camGUI = new CameraGUI(cam, dis, true, false);   
    showCameraElements = true;
   
    lights = new ArrayList();    
    
    movableObjects.add(cam);
    movableObjects.add(target);
    
    initGUI();    
    
    addTriangle();
  }
  
  void initGUI()
  {
    controlP5 = new ControlP5(app);
    PFont font;
    font = loadFont("Calibri-48.vlw");
    controlP5.setControlFont(font,11);
    
    int circleY = 10;
    
    Textlabel myTextlabelA = controlP5.addTextlabel("label83","Triangle:",10,circleY);
    myTextlabelA.setColorValue(color(0));
   
    myTextlabelA = controlP5.addTextlabel("label10","Shininess:",10,circleY += 15);
    myTextlabelA.setColorValue(color(100));
    
    shininessSlider = controlP5.addSlider("",1,1000,1000,10,circleY += 10,100,10);
    shininessSlider.setColorBackground(color(150));
    shininessSlider.setColorForeground(color(0));
    
    myTextlabelA = controlP5.addTextlabel("label1023","Reflectiveness:",10,circleY += 15);
    myTextlabelA.setColorValue(color(100));
    
    reflectivenessSlider = controlP5.addSlider("",0,1,0,10,circleY += 10,100,10);
    reflectivenessSlider.setColorBackground(color(150));
    reflectivenessSlider.setColorForeground(color(0));
    
    myTextlabelA = controlP5.addTextlabel("label9","Diffuse color:",10,circleY += 15);
    myTextlabelA.setColorValue(color(100));
    
    cp = controlP5.addColorPicker("picker",10,circleY += 10,100,10);
    cp.setColorValue(color(255,0,0));
    
    myTextlabelA = controlP5.addTextlabel("label91","Specular color:",10,circleY += 65);
    myTextlabelA.setColorValue(color(100));
    
    cpSpec = controlP5.addColorPicker("pickerSpec",10,circleY += 10,100,10);
    cpSpec.setColorValue(color(0,0,0));
    
    myTextlabelA = controlP5.addTextlabel("label92","Ambient color:",10,circleY += 65);
    myTextlabelA.setColorValue(color(100));
    
    cpAmbient = controlP5.addColorPicker("pickerAmbient",10,circleY += 10,100,10);
    cpAmbient.setColorValue(color(0,0,0));
    
    controlP5.Button circleButton = controlP5.addButton("Add Triangle",128,10,circleY += 75,100,20);
    circleButton.setColorForeground(color(0));
    circleButton.setColorBackground(color(150));
    
    int lightY = circleY + 35;
    
    myTextlabelA = controlP5.addTextlabel("label112","Light:",10,lightY);
    myTextlabelA.setColorValue(color(0));
    
    myTextlabelA = controlP5.addTextlabel("label113","Attenuation.Linear:",10,lightY += 15);
    myTextlabelA.setColorValue(color(100));
    attenLinearSlider = controlP5.addSlider("",0,0.01,0,10,lightY += 10,100,10);
    attenLinearSlider.setColorBackground(color(150));
    attenLinearSlider.setColorForeground(color(0));
    
    myTextlabelA = controlP5.addTextlabel("label114","Attenuation.Quad:",10,lightY += 15);
    myTextlabelA.setColorValue(color(100));
    attenQuadSlider = controlP5.addSlider("",0,0.00001,0,10,lightY += 10,100,10);
    attenQuadSlider.setColorBackground(color(150));
    attenQuadSlider.setColorForeground(color(0));
    
    controlP5.Button lightButton = controlP5.addButton("Add Light",128,10,lightY +=20,100,20);
    lightButton.setColorForeground(color(0));
    lightButton.setColorBackground(color(150));
    
    //Pipeline Stages
    int stagesSectionX = width - 130;
    int stagesSectionY = 10;
        
    myTextlabelA = controlP5.addTextlabel("label8",AppStrings.getString("pipelineStages") + ":",stagesSectionX,stagesSectionY);
    myTextlabelA.setColorValue(color(0));
    
    stagesButtonArray = new controlP5.Button[PipelineStages.StagesNames.length];
    for(int i=0; i< PipelineStages.StagesNames.length; i++)
    {
      stagesButtonArray[i] = controlP5.addButton(PipelineStages.StagesNames[i],128,stagesSectionX,stagesSectionY += 35,110,20);
      stagesButtonArray[i].setColorForeground(color(0));
      stagesButtonArray[i].setColorBackground(color(150));
      stagesButtonArray[i].setColorActive(color(0,128,0));
      stagesButtonArray[i].setSwitch(true);
    }
    
  }
      
  void update()
  {
    super.update();
    shading();
    updateStage();
    if(clicked != null)
    {
      if (clicked instanceof Vertex)
      {
        Vertex t = (Vertex)clicked;
        t.setMaterial(cp.getColorValue(),cpSpec.getColorValue(),cpAmbient.getColorValue(),shininessSlider.getValue(),reflectivenessSlider.getValue());
      }
      else if(clicked instanceof Light)
      {
        Light l = (Light)clicked;
        l.atenuationLinear = attenLinearSlider.getValue();
        l.atenuationQuadratic = attenQuadSlider.getValue();
      }
    }
    
  }
  
  void updateStage()
  {  
    Matrix Cam = CameraMatrix();
    Matrix ICam = InvCameraMatrix();
    Matrix P = PerspetiveMatrix();
    Matrix M = Matrix.MultiplyMatrix(P, Cam);
    for(Object o : objects)
    {
      if(o instanceof Triangle)
      {
        Triangle so = (Triangle)o;
        so.drawCoords = currentStage == PipelineStages.Init;
        so.drawNormals = currentStage <= PipelineStages.Shading;
        for(Vertex v : so.vertex)
        {
          v.shade = currentStage >= PipelineStages.Shading;
          println("0)" + v.v.ToString2D());
          v.altV = Matrix.MultiplyVector(M, v.v);
          println("1)" + v.altV.ToString2D());
          v.altV = Matrix.MultiplyVector(ICam, v.altV);
          println("2)" +v.altV.ToString2D());
          v.altV = Vector.MultiplyS(v.altV, 1.0/v.altV.w);
          println("3)" +v.altV.ToString2D());
          v.correctPerspective = currentStage >= PipelineStages.Perspective;
        }
      }
    } 
    showWorldCoords = currentStage == PipelineStages.WoorldCoords;
    cam.showCameraCoords = currentStage == PipelineStages.CameraCoords || 
                           currentStage == PipelineStages.Shading || 
                           currentStage == PipelineStages.Perspective;
    cam.showFrustrum = currentStage < PipelineStages.Perspective;                        
    cam.showFrustrumModPerspect = currentStage >= PipelineStages.Perspective;
    cam.showFrustrumCoords = currentStage >= PipelineStages.ProjectionCoords && currentStage < PipelineStages.ViewportCoords;
    cam.showViewportCoords = currentStage >= PipelineStages.ViewportCoords;
    
  }
  
  void shading()
  {
    for(Object o : objects)
    {
      if(o instanceof Triangle)
      {
        Triangle so = (Triangle)o;
        for(Vertex vertex : so.vertex)
        {
          Vector shading = new Vector(red(vertex.ambient)/255.0, green(vertex.ambient)/255.0,blue(vertex.ambient)/255.0) ; //Ambient shading
          Vector base = new Vector(red(vertex.diffuse)/255.0, green(vertex.diffuse)/255.0,blue(vertex.diffuse)/255.0) ;
          Vector specBase = new Vector(red(vertex.specular)/255.0, green(vertex.specular)/255.0,blue(vertex.specular)/255.0);
          for(Object ol : lights)
          {
            Light lightObj = (Light)ol;
            Vector lightColor = new Vector(red(lightObj.col)/255.0, green(lightObj.col)/255.0,blue(lightObj.col)/255.0) ;
            Vector v = Vector.Normalize3(Vector.Minus(cam.position,vertex.v));
            Vector l = Vector.Normalize3(Vector.Minus(lightObj.position,vertex.v));
            
            //Half vector for Phong shading
            Vector h = Vector.Normalize3(Vector.Add(v,l));
            
            float light_dist = Vector.Minus(lightObj.position, vertex.v).Magnitude3();
            
            float distanceSquared = light_dist * light_dist;
            float attenuationFactor = 1/(lightObj.atenuationConstant + lightObj.atenuationLinear * light_dist + lightObj.atenuationQuadratic * distanceSquared); 
            Vector attenuationVector = Vector.Multiply(attenuationFactor, lightColor);
            
            //Diffuse
            float normalAngleFactor = Math.max(0, Vector.Dot3(vertex.n, l));
            float diffuseFactor = normalAngleFactor;   
            Vector diffuse = Vector.Multiply(diffuseFactor, base);
            diffuse = Vector.MultiplyV(attenuationVector, diffuse);
            shading = Vector.Add(shading, diffuse);
            
            //Specular
            float viewAngleFactor = (float)Math.pow(Math.max(0, Vector.Dot3(vertex.n, h)),vertex.shininess);
            float specularFactor = viewAngleFactor; 
            Vector specular = Vector.Multiply(specularFactor, specBase);
            specular = Vector.MultiplyV(attenuationVector, specular);
            shading = Vector.Add(shading, specular);           
            
          }
          vertex.shading = color(int(255*shading.x),int(255*shading.y),int(255*shading.z));
        }        
      } 
    }
  }
  
  void draw()
  {
    super.draw();
    if(showWorldCoords)
      worldCoords.draw();
  }
  
  void mousePressed()
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
           
           if(clicked instanceof Triangle)
           {
             Triangle c = (Triangle)clicked;
             cpAmbient.setColorValue(c.ambient);
             cp.setColorValue(c.diffuse);
             cpSpec.setColorValue(c.specular);
             shininessSlider.setValue(c.shininess);
             reflectivenessSlider.setValue(c.reflectiveness);
           }   
           else if(clicked instanceof Vertex)
           {
             Vertex c = (Vertex)clicked;
             cpAmbient.setColorValue(c.ambient);
             cp.setColorValue(c.diffuse);
             cpSpec.setColorValue(c.specular);
             shininessSlider.setValue(c.shininess);
             reflectivenessSlider.setValue(c.reflectiveness);
           }
           else if(clicked instanceof Light)
           {
             Light l = (Light)clicked;
             attenLinearSlider.setValue(l.atenuationLinear);
             attenQuadSlider.setValue(l.atenuationQuadratic);
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
  

  
  void controlEvent(ControlEvent ev)
  {
    if(ev.controller().name().equals("Add Triangle"))
    {
      addTriangle();
    } 
    else if(ev.controller().name().equals("Add Light"))
    {
      addLight();
    }
    int newStage = -1;
    boolean flag = false;
    for(int i=0; i< PipelineStages.StagesNames.length; i++)
    {
      if(ev.controller().name().equals(PipelineStages.StagesNames[i]))
      {
        flag = true;
        if(!stagesButtonArray[i].booleanValue())
        {
          newStage = i;
        }
        break;
      }
    }
    if(newStage != -1)
    {
      for(int i=0; i< PipelineStages.StagesNames.length; i++)
      {
        if(i != newStage)
          stagesButtonArray[i].setOff();
      }
    }
    if(flag)
      currentStage = newStage;
   
  }
  
  void addTriangle()
  {
      Triangle t = new Triangle(cp.getColorValue(), new Vector(width/2,height/4+50), new Vector(width/2 - 25,height/4), new Vector(width/2 + 25,height/4),  true);
      t.setMaterial(cp.getColorValue(),cpSpec.getColorValue(),cpAmbient.getColorValue(),shininessSlider.getValue(),reflectivenessSlider.getValue());
      objects.add(t);
      movableObjects.add(t);
      movableObjects.add(t.vertex[0]);
      movableObjects.add(t.vertex[1]);
      movableObjects.add(t.vertex[2]);
      
      for(Vertex v : t.vertex)
      {
        worldCoords.vertexes.add(v);
        cam.cameraCoords.vertexes.add(v);
        cam.frustrumCoords.vertexes.add(v);
        cam.viewportCoords.vertexes.add(v);
      }
  }
  
   void addLight()
  {
    Light l = new Light(new Vector(3*width/4,height/2), color(255), true);
      l.atenuationLinear = attenLinearSlider.getValue();
      l.atenuationQuadratic = attenQuadSlider.getValue();
      lights.add(l);
      movableObjects.add(l);
  }
  
Matrix ViewportMatrix()
{
    Matrix Mvp = new Matrix();

      Mvp.M11 = cam.rays / 2.0f;
      Mvp.M12 = 0;
      Mvp.M13 = 0;
      Mvp.M14 = (cam.rays - 1) / 2.0f;
      Mvp.M21 = 0;
      Mvp.M22 = cam.rays / 2.0f;
      Mvp.M23 = 0;
      Mvp.M24 = (cam.rays - 1) / 2.0f;
      Mvp.M31 = 0;
      Mvp.M32 = 0;
      Mvp.M33 = 1;
      Mvp.M34 = 0;
      Mvp.M41 = 0;
      Mvp.M42 = 0;
      Mvp.M43 = 0;
      Mvp.M44 = 1;

      return Mvp;

  }

  Matrix OrthographicProjectionMatrix()
  {
      float r = cam.r;
      float l = cam.l;
      float t = r;
      float b = l;
      float n = -cam.nearClip;
      float f = -cam.farClip;
      Matrix Mo = new Matrix();

      Mo.M11 = 2.0f / (r - l);
      Mo.M12 = 0;
      Mo.M13 = 0;
      Mo.M14 = -(r + l) / (r - l);
      Mo.M21 = 0;
      Mo.M22 = 2.0f / (t - b);
      Mo.M23 = 0;
      Mo.M24 = -(t + b) / (t - b);
      Mo.M31 = 0;
      Mo.M32 = 0;
      Mo.M33 = 2.0f / (n - f);
      Mo.M34 = -(n + f) / (n - f);
      Mo.M41 = 0;
      Mo.M42 = 0;
      Mo.M43 = 0;
      Mo.M44 = 1;


      return Mo;
  }

  Matrix PerspetiveMatrix()
  {
      Matrix P = new Matrix();
      float n = -cam.nearClip;
      float f = -cam.farClip;

      P.M11 = n;      P.M12 = 0;       P.M13 = 0;       P.M14 = 0;
      P.M21 = 0;      P.M22 = n + f;   P.M23 = 0;       P.M24 = -f * n;
      P.M31 = 0;      P.M32 = 0;       P.M33 = 0;       P.M34 = 0;
      P.M41 = 0;      P.M42 = 1;       P.M43 = 0;       P.M44 = 0;

      return P;
  }

   Matrix PerspectiveProjectionMatrix()
  {
      Matrix Mper = new Matrix();

      Matrix P = PerspetiveMatrix();
      Matrix Mo = OrthographicProjectionMatrix();

      Mper = Matrix.MultiplyMatrix(Mo, P);

      return Mper;
  }

  Matrix CameraMatrix()
  {
      Matrix a, b;
      a = new Matrix();
      b = new Matrix();
      
      
      a.M11 = cam.u.x;
      a.M12 = cam.u.y;
      a.M13 = 0;
      a.M21 = cam.w.x;
      a.M22 = cam.w.y;
      a.M23 = 0;
      a.M31 = 0;
      a.M32 = 0;
      a.M33 = 1;
      a.M44 = 1;

      b.M11 = 1;
      b.M12 = 0;
      b.M13 = -cam.position.x;
      b.M21 = 0;
      b.M22 = 1;
      b.M23 = -cam.position.y;
      b.M31 = 0;
      b.M32 = 0;
      b.M33 = 1;
      b.M44 = 1;
      
      return Matrix.MultiplyMatrix(a,b);
  }
  
  Matrix InvCameraMatrix()
  {
      Matrix a, b;
      a = new Matrix();
      b = new Matrix();
      
      
      a.M11 = cam.u.x;
      a.M12 = cam.w.x;
      a.M13 = 0;
      a.M21 = cam.u.y;
      a.M22 = cam.w.y;
      a.M23 = 0;
      a.M31 = 0;
      a.M32 = 0;
      a.M33 = 1;
      a.M44 = 1;
      
      b.M11 = 1;
      b.M12 = 0;
      b.M13 = cam.position.x;
      b.M21 = 0;
      b.M22 = 1;
      b.M23 = cam.position.y;
      b.M31 = 0;
      b.M32 = 0;
      b.M33 = 1;
      b.M44 = 1;
      
      return Matrix.MultiplyMatrix(b,a);
  }
}
