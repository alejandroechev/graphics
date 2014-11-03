/* Camera element of a scene
 
 Stores and draws the relevant information of the camera and its coordinate system

*/
class Camera implements IMovable
{  
  
  //Camera coordinate parameteres
  Vector position;  
  Target target;
  int up = 1;
 
  //Camera coordinates
  Vector u;
  Vector w;
   
  //Frustrum parameters
  float nearClip;
  float farClip;
  float fov;
  float r,l;
  
  //Number of rays/pixels
  int rays;
  
  //Resources
  PImage img;
  PImage imgDown;  
  PFont font;
  
  //Sprite size
  float sizeX = 48;
  float sizeY = 48;
  
  //Coordinate systems sprite
  CoordinateDisplay cameraCoords;
  CoordinateDisplay frustrumCoords;
  CoordinateDisplay viewportCoords;
  
  //For mouse movement
  boolean isMovable;
  boolean isSelected;
  Vector previousMouse;
   
  //Configuration parameteres
  boolean showTarget = true;
  boolean showNearClip = false;
  boolean showNearClipPosition = false;
  boolean showFarClip = false;
  boolean showFrustrum = false;
  boolean showFrustrumModPerspect = false;
  boolean drawPixels = true;
  boolean showCameraCoords = false;
  boolean showFrustrumCoords = false;
  boolean showViewportCoords = false;
    
  Camera(Vector pos, Vector targ, float n, float f, float _fov, int _rays, boolean movable)
  {
    target = new Target(targ, movable, "data//target.png");
    position = pos;
    nearClip = n;
    farClip = f;
    fov = _fov;
    rays = _rays;
    isMovable = movable;
    img = loadImage("data//cameraUp.png");  
    imgDown = loadImage("data//cameraDown.png");    
    cameraCoords = new CoordinateDisplay();
    frustrumCoords = new CoordinateDisplay();
    viewportCoords = new CoordinateDisplay();
    viewportCoords.showMinusAxis = false;
    update();
  }
  
  String save()
  {
     String movableS = isMovable ? "true" : "false";
     return "camera:"+position.ToString()+";"+target.position.ToString()+";"+fov+";"+rays+";"+nearClip+";"+farClip+";"+movableS;
  }
  
  void load(String s)
  {
    s = s.trim();
    String info = s.split(":")[1];
    String[] vals = info.split(";");
    position.FromString(vals[0]);
    Vector targ = new Vector();
    targ.FromString(vals[1]);
    fov = Float.parseFloat(vals[2]);
    rays = Integer.parseInt(vals[3]);
    nearClip = Float.parseFloat(vals[4]);
    farClip = Float.parseFloat(vals[5]);
    isMovable = vals[6].equals("true");
    target = new Target(targ, isMovable, "data//target.png");
    update();
    
  }
  
  boolean isMovable(){ return isMovable;}
  
   void setPosition(Vector p)
    {
      Vector delta = Vector.Minus(p, previousMouse);
      position = Vector.Add(position, delta);
      previousMouse = p;
    }
    
    Vector getPosition()
    {
      return position;
    }
    
    void setFirstMouse(Vector p)
    {
      previousMouse = p;
    }
  
  boolean isInside(Vector p)
  {
    Vector center = new Vector(position.x, position.y);
    float d = center.Distance3(p);    
    return d < sizeX/2;
  }
  
  String label()
  {
    return AppStrings.getString("position") + ": " + new Vector(position.x, height - position.y).ToString2D();
  }
  
  Vector labelPosition()
  {
    return Vector.Add(position, new Vector(1.2*sizeX, 0));
  }
  
  void setSelected(boolean isSel)
    {
      isSelected = isSel;
    }
    
    void flipSelected()
    {
      isSelected = !isSelected;
    }
    
 
  
  
  void update()
  {
    r = (float)(Math.abs(nearClip) * Math.tan(((fov / 2) / 180.0) * PI));
    l = -r;
    
    w = Vector.Minus(target.position, position);
    w = Vector.Normalize3(w);
    w = Vector.Multiply(-1, w);
    
    u = new Vector(-w.y, w.x);
    
    cameraCoords.init(position, u, w, 50, 50);
    
    float midFrustrum = (nearClip + farClip)/2.0;
    Vector frustrumCenter = Vector.Add(position, Vector.Multiply(midFrustrum, Vector.Multiply(-1, w))); 
    
    float centerFrustrum = (farClip - nearClip)/2.0;
    frustrumCoords.init(frustrumCenter, u, w, r, centerFrustrum);
    
    float pixelSize = r / rays;
    Vector viewportOrigin = Vector.Add(position, Vector.Multiply(nearClip, Vector.Multiply(-1, w)));
    viewportOrigin = Vector.Add(viewportOrigin, Vector.Multiply(r-pixelSize, u));
    viewportCoords.init(viewportOrigin, Vector.Multiply(-1, u), w, 2*r, 20);
    
  }
  
  void draw()
  {
    if(showTarget)
      target.draw();
    
    if(showCameraCoords)
      cameraCoords.draw(); 
   
    if(showFrustrumCoords)
      frustrumCoords.draw();
     
    if(showViewportCoords)
      viewportCoords.draw(); 
   
    if(isSelected)
     {
       stroke(0,0,0,0);
       fill(0,0,0,25);
       ellipse(position.x, position.y, 1.5*sizeX, 1.5*sizeY);
     }
    
    Vector delta = Vector.Minus(target.position, position);
    delta = Vector.Normalize3(delta);
    
    if(showNearClipPosition)
      drawNearClipPosition(delta);
    if(showNearClip)
      drawClipPlane(delta, nearClip, false);
    if(showFarClip)
      drawClipPlane(delta, farClip, false);
    if(showFrustrum)
      drawFrustrum(delta, nearClip, farClip, false);
    if(showFrustrumModPerspect)
      drawFrustrum(delta, nearClip, farClip, true);  
    
    pushMatrix();
    
    float angle = atan2(delta.y, delta.x);
    translate(position.x, position.y);
    rotate(angle);
    stroke(255);
    fill(255);
    image(up == 1 ? img : imgDown, - sizeX/2, - sizeY/2, sizeX,sizeY);
    ellipse(0,0,3,3);
    
    popMatrix();
    
    
  }
  
  Vector[] drawNearClipPosition(Vector dir)
  {
    Vector viewDist = Vector.Multiply(nearClip, dir);
    Vector viewCenter = Vector.Add(position, viewDist);
    Vector perpDir = new Vector(-dir.y, dir.x);
    
    float x = 10;
    Vector view1 = Vector.Add(viewCenter, Vector.Multiply(x, perpDir));
    Vector view2 = Vector.Minus(viewCenter, Vector.Multiply(x, perpDir));
    
    stroke(50);
    line(view1.x, view1.y, view2.x, view2.y);
    
    return new Vector[]{view1, view2};
    
  }
  
  Vector[] drawClipPlane(Vector dir, float clip, boolean modPerspective)
  {
    Vector viewDist = Vector.Multiply(clip, dir);
    Vector viewCenter = Vector.Add(position, viewDist);
    Vector perpDir = new Vector(-dir.y, dir.x);
    
    if(modPerspective)
      clip = nearClip;
      
    float x = clip * tan((fov/2)*PI/180.0f);
    Vector view1 = Vector.Add(viewCenter, Vector.Multiply(x, perpDir));
    Vector view2 = Vector.Minus(viewCenter, Vector.Multiply(x, perpDir));
    
    stroke(50);
    line(view1.x, view1.y, view2.x, view2.y);
    
    float d = view1.Distance3(view2);
    float pixelSize = d / rays;
    Vector delta = Vector.Minus(view2, view1).Normalize3();
    Vector perpDelta = new Vector(-delta.y, delta.x);
    Vector current = view1;
    Vector nextPerp = Vector.Add(current, Vector.Multiply(3, perpDelta));
    if(drawPixels)
    {
      for(int i=0; i<rays+1; i++)
      {      
        line(current.x, current.y, nextPerp.x, nextPerp.y);
        current = Vector.Add(current, Vector.Multiply(pixelSize, delta));
        nextPerp = Vector.Add(current, Vector.Multiply(3, perpDelta));
      }
    }
    
    return new Vector[]{view1, view2};
    
  }
  
  void drawFrustrum(Vector dir, float nearClip, float farClip, boolean modPerspective)
  {
    Vector[] nearVectors = drawClipPlane(dir, nearClip, modPerspective);
    Vector[] farVectors = drawClipPlane(dir, farClip, modPerspective);
    
    stroke(50);
    line(nearVectors[0].x, nearVectors[0].y,farVectors[0].x,farVectors[0].y); 
    line(nearVectors[1].x, nearVectors[1].y,farVectors[1].x,farVectors[1].y); 
    
  }
  
}
