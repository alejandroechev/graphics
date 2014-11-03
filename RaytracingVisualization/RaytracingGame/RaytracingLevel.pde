class RaytracingLevel extends Level
{
  ArrayList directRays;
  ArrayList shadowRays;
  ArrayList reflectedRays;
  ArrayList normals;
  ArrayList halfVectors;
 
  int maxRecursion = 3;
  
  float[] dashes = { 8.0f, 4.0f, 8.0f, 4.0f };
  float[] dots = { 1.0f, 4.0f, 1.0f, 4.0f };
  BasicStroke regular;
  BasicStroke dashed;
  BasicStroke dotted;
  
  
  RaytracingLevel()
  {
    super();
    directRays = new ArrayList();
    shadowRays = new ArrayList();
    reflectedRays = new ArrayList();
    normals = new ArrayList();
    halfVectors = new ArrayList();
    
    regular = new BasicStroke(1.0f);
    dashed = new BasicStroke(1.0f, BasicStroke.CAP_ROUND, BasicStroke.JOIN_MITER,
	4.0f, dashes, 0.0f);
    dotted = new BasicStroke(1.0f, BasicStroke.CAP_ROUND, BasicStroke.JOIN_MITER,
	4.0f, dots, 0.0f);

  }
  
  void load(String fileName)
  {
    super.load(fileName);
     if(disGoal != null)
      {
        camGoal.update();
        for(int i=0; i<camGoal.rays; i++)
        {
          color c = calculatePixel(camGoal, i, false);
          disGoal.pixelValues[i] = c;
        }
      }
  }
  
  void update()
  {
    super.update();
    
    directRays.clear();
    shadowRays.clear();
    reflectedRays.clear();
    normals.clear();
    halfVectors.clear();
    for(int i=0; i<cam.rays; i++)
    {
      color c = calculatePixel(cam, i, true);
      if(dis!= null)
         dis.pixelValues[i] = c;
    }
  }
  
  void draw()
  {
    update();
    background(back);
    if(showRays)
    {
      for(Object o : directRays)
      {
        Ray r = (Ray)o;
        r.draw();
      }
    }
    if(showShadowRays)
    {
      Graphics2D g2 = ((PGraphicsJava2D) g).g2;
      g2.setStroke(dotted);
    
      for(Object o : shadowRays)
      {
        Ray r = (Ray)o;
        r.draw();
      }
      g2.setStroke(regular);
    
    }
    if(showReflectedRays)
    {
      Graphics2D g2 = ((PGraphicsJava2D) g).g2;
      g2.setStroke(dashed); 
      
      for(Object o : reflectedRays)
      {
        Ray r = (Ray)o;
        r.draw();
      }
      g2.setStroke(regular);
    }
    if(showNormals)
    {
      for(Object o : normals)
      {
        Ray r = (Ray)o;
        r.draw();
      }
    }
    
    if(showHalfVectors)
    {
      for(Object o : halfVectors)
      {
        Ray r = (Ray)o;
        r.draw();
      }
    }
    
    super.drawScene();
    
  }
  
  color calculatePixel(Camera cam, int pixelIndex, boolean storeRays)
  {
     float u = cam.l + (cam.r - cam.l) * (pixelIndex + 0.5f) / cam.rays;
     Vector pixelCoords = new Vector(u, -cam.nearClip);
     
     Vector pixelWorldCoords = Vector.Add(cam.position, Vector.Add(Vector.Multiply(pixelCoords.x, cam.u), Vector.Multiply(pixelCoords.y, cam.w)));
     Vector dir = Vector.Normalize3(Vector.Minus(pixelWorldCoords, cam.position));
     Ray r = new Ray(cam.position, dir); // eye rays
     if(storeRays)
       directRays.add(r);
      
     Vector colVec = getColor(r, storeRays, 0);
     color c = color(int(255*colVec.x),int(255*colVec.y),int(255*colVec.z));
     return c;
  }
  
  Vector getColor(Ray r, boolean storeRays, int recursion)
  {
    if(recursion > 0)
    {
      //println(r.direction.ToString2D());
      reflectedRays.add(r);
    }
    intersectAll(r);
    if(r.firstObj != null)// && r.firstT > cam.nearClip && r.firstT < cam.farClip)
    {
      Vector intersection = Vector.Add(r.position, Vector.Multiply(r.firstT, r.direction));
      Vector n = r.firstObj.getNormal(intersection);
      Ray normalRay = new Ray(intersection, n);
      normalRay.firstT = 10;
      normalRay.col = color(50);
      normals.add(normalRay);
      Vector shading = new Vector(red(ambient)/255.0, green(ambient)/255.0,blue(ambient)/255.0) ; //Ambient shading
      Vector base = new Vector(red(r.firstObj.diffuse)/255.0, green(r.firstObj.diffuse)/255.0,blue(r.firstObj.diffuse)/255.0) ;
      Vector specBase = new Vector(red(r.firstObj.specular)/255.0, green(r.firstObj.specular)/255.0,blue(r.firstObj.specular)/255.0);
      float epsilon = 0.01f;
      
      for(Object o : lights)
      {
        Light lightObj = (Light)o;
        Vector lightColor = new Vector(red(lightObj.col)/255.0, green(lightObj.col)/255.0,blue(lightObj.col)/255.0) ;
        Vector v = Vector.Multiply(-1, r.direction);
        Vector l = Vector.Normalize3(Vector.Minus(lightObj.position,intersection));
        
        //Half vector for Phong shading
        Vector h = Vector.Normalize3(Vector.Add(v,l));
        Ray halfRay = new Ray(intersection, h);
        halfRay.firstT = 20;
        halfRay.col = color(110);
        halfVectors.add(halfRay);
        
        //Shadow rays
        Ray shadow = new Ray(Vector.Add(intersection , Vector.Multiply(epsilon,l)), l);
        intersectAll(shadow);  // shadow rays
        Ray shadowDraw = new Ray(Vector.Add(intersection , Vector.Multiply(epsilon,l)), l);
        //shadowDraw.col = color(10);
        shadowDraw.firstT = shadow.firstT;
        
        float light_dist = Vector.Minus(lightObj.position, intersection).Magnitude3();
        if (shadow.firstObj == null || shadow.firstT >= light_dist) // if we are not in shadow
        {
          float distanceSquared = light_dist * light_dist;
          float attenuationFactor = 1/(lightObj.atenuationConstant + lightObj.atenuationLinear * light_dist + lightObj.atenuationQuadratic * distanceSquared); 
          Vector attenuationVector = Vector.Multiply(attenuationFactor, lightColor);
          
          //Diffuse
          float normalAngleFactor = Math.max(0, Vector.Dot3(n, l));
          float diffuseFactor = normalAngleFactor;// / (lightObj.atenuationConstant + lightObj.atenuationQuadratic * distanceSquared);   
          Vector diffuse = Vector.Multiply(diffuseFactor, base);
          diffuse = Vector.MultiplyV(attenuationVector, diffuse);
          shading = Vector.Add(shading, diffuse);
          
          //Specular
          float viewAngleFactor = (float)Math.pow(Math.max(0, Vector.Dot3(n, h)),r.firstObj.shininess);
          float specularFactor = viewAngleFactor;// / (lightObj.atenuationConstant + lightObj.atenuationQuadratic * distanceSquared);   
          Vector specular = Vector.Multiply(specularFactor, specBase);
          specular = Vector.MultiplyV(attenuationVector, specular);
          shading = Vector.Add(shading, specular);           
          
          shadowDraw.firstT = light_dist;
        }
        if(storeRays)
          shadowRays.add(shadowDraw);
      } 
      if(r.firstObj.reflectiveness > 0 && recursion < maxRecursion)
      {
        Vector ref = (Vector.Minus(r.direction, Vector.Multiply(Vector.Dot3(r.direction, n) * 2, n)).Normalize3());
        Ray reflected = new Ray(Vector.Add(intersection, Vector.Multiply(epsilon, ref)), ref);
        
        shading = Vector.Add(shading, Vector.Multiply(r.firstObj.reflectiveness, getColor(reflected, true, recursion+1)));
        println(ref.ToString2D());
      }
      /*
      if(r.firstObj.refractionIndex > 0 && recursion < maxRecursion)
      {
        float nT = r.firstObj.refractionIndex;
        float root = (1 - (1 - pow(Vector.Dot3(r.direction, n),2)/(nT*nT)));
        if(root >= 0)
        {
            float sqrt = sqrt(root);
            Vector t = Vector.Minus(Vector.Multiply(1 / nT,(Vector.Minus(r.direction, Vector.Multiply(Vector.Dot3(r.direction, n) , n)))), Vector.Multiply(sqrt,n)); 
            float factor = pow(-1, recursion);
            t = Vector.Multiply(factor, t.Normalize3());
            Ray refracted = new Ray(Vector.Add(intersection, Vector.Multiply(epsilon, t)), t);
            shading = Vector.Add(shading, Vector.Multiply(r.firstObj.refractionIndex, getColor(refracted, true, recursion+1)));
            
        }
      }*/
      
      return shading;
    }
    return backVec;
  }
  
  void intersectAll(Ray ray) //  intersection with all objects
  {
      float e = 0.001f;    
      ray.firstT = 100000;
      ray.firstObj = null;
  
      for (Object o: objects)
      {
          SceneObject sceneObj = (SceneObject)o;                    
          float t = ray.intersect(sceneObj);
          if (t > e && t < ray.firstT)
          {
              ray.firstT = t;
              ray.firstObj = sceneObj;
          }
      }
  }
}
