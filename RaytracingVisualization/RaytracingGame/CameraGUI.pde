class CameraGUI
{
  Vector position;
  float fov;
  int rays;
  
  Button minusFov;
  Button plusFov;
  Button minusRay;
  Button plusRay;
  Button up;
  
  ArrayList controls;
  
  CameraGUI(Camera cam, Display dis, boolean enableFOV, boolean enableRays)
  {
    position = cam.position;
    minusFov = new MinusFOVButton(new Vector(position.x - 30, position.y + 30), 12,12, "data//minus.png", "data//minusB.png", cam, enableFOV);
    plusFov = new PlusFOVButton(new Vector(position.x + 30, position.y + 30), 12,12, "data//plus.png", "data//plusB.png", cam, enableFOV);
    minusRay = new MinusRaysButton(new Vector(position.x - 30, position.y - 30), 12,12, "data//minus.png", "data//minusB.png", cam, dis, enableRays);
    plusRay = new PlusRaysButton(new Vector(position.x + 30, position.y - 30), 12,12, "data//plus.png", "data//plusB.png", cam, dis, enableRays);
    up = new UpButton(new Vector(position.x - 30, position.y + 50), 12,12, "data//up.png", "data//upB.png", cam, dis, true);
    
    controls = new ArrayList(); 
    controls.add(minusFov);
    controls.add(plusFov);
    controls.add(minusRay);
    controls.add(plusRay);
    controls.add(up);
  }
  
  void updatePositions(Vector p)
  {
    position = p;
    minusFov.position = new Vector(position.x - 30, position.y + 30);
    plusFov.position = new Vector(position.x + 30, position.y + 30);
    minusRay.position = new Vector(position.x - 30, position.y - 30);
    plusRay.position = new Vector(position.x + 30, position.y - 30);
    up.position = new Vector(position.x - 30, position.y + 50); 
  }
  
  void draw()
  {
    
      for(Object o : controls)
      {
        Button b = (Button)o;
        if(b.isEnabled)
          b.draw();
      }
    
  }
  
  
}
