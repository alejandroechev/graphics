/*
Triangle primitive of a scene

*/
class Triangle extends SceneObject
{

  //Triangle specific data: vertex and normals
  Vertex[] vertex;
  Vector[] normals;
  
  //Visual representation of normals
  Ray[] normalRays;
  
  //Object coordinates
  CoordinateDisplay coords;
  
  //Configuration parameteres
  boolean drawNormals = true;
  boolean drawCoords = true;
  
  Triangle()
  {
    super(color(0), true);
    vertex = new Vertex[3];
    normals = new Vector[3];
    normalRays = new Ray[3];
    coords = new CoordinateDisplay();
  }
  
  Triangle(color c, Vector v1, Vector v2, Vector v3, boolean movable)
  {
    super(c, movable);
    vertex = new Vertex[3];
    vertex[0] = new Vertex(v1, movable,c);
    vertex[1] = new Vertex(v2, movable,c);
    vertex[2] = new Vertex(v3, movable,c);
    coords = new CoordinateDisplay();
    
    setNormals();
  }
  
  void setMaterial(color diff, color spec, color amb, float shin, float ref)
  {
    diffuse = diff;
    specular = spec;
    ambient = amb;
    shininess = shin;
    reflectiveness = ref;
    for(Vertex vx : vertex)
    {
      vx.setMaterial(diffuse,specular,amb,shin,ref);
    }
  }
  
  
  void setNormals()
  {
    Vector c = centroid();
    
    normals = new Vector[3];
    for(int i=0; i<normals.length; i++)
    {
      normals[i] = Vector.Normalize3(Vector.Minus(vertex[i].v, c));
      vertex[i].n = normals[i];
    }
    
    normalRays = new Ray[normals.length];
    for(int i=0; i<normals.length; i++)
    {
      Ray normalRay = new Ray(vertex[i].v, normals[i]);
      normalRay.firstT = 10;
      normalRay.col = color(50);
      normalRays[i] = (normalRay);
    }
  }
  
  boolean isInside(Vector p)
  {
    Vector bar = Barycentric(p.x, p.y, vertex[0].v.x, vertex[1].v.x, vertex[2].v.x, vertex[0].v.y, vertex[1].v.y, vertex[2].v.y);                    
    return (bar.x >= 0 && bar.y >= 0 && bar.z >= 0);
                    
  }
  
  void setPosition(Vector p)
    {
      Vector delta = Vector.Minus(p, previousMouse);
      for(Vertex vx : vertex)
        vx.v = Vector.Add(vx.v, delta);
      previousMouse = p;
    }
  
  float intersect(Ray r)
  {    
    return -1;
  }
  
  Vector Barycentric(float x, float y, float x0, float x1, float x2, float y0, float y1, float y2)
  {
      float alpha = ((y1 - y2) * x + (x2 - x1) * y + x1 * y2 - x2 * y1) / ((y1 - y2) * x0 + (x2 - x1) * y0 + x1 * y2 - x2 * y1);
      float beta = ((y2 - y0) * x + (x0 - x2) * y + x2 * y0 - x0 * y2) / ((y2 - y0) * x1 + (x0 - x2) * y1 + x2 * y0 - x0 * y2);
      float gamma = ((y0 - y1) * x + (x1 - x0) * y + x0 * y1 - x1 * y0) / ((y0 - y1) * x2 + (x1 - x0) * y2 + x0 * y1 - x1 * y0);

      return new Vector(alpha, beta, gamma);
  }
  
  Vector centroid()
  {
    Vector c = Vector.Multiply(1.0/3.0, Vector.Add(vertex[0].v, Vector.Add(vertex[1].v, vertex[2].v)));
    return c; 
  }
  
  void draw()
  {
     Vector c = centroid();
     coords.vertexes.clear();
     for(Vertex v : vertex)
      coords.vertexes.add(v);
       
     if(isSelected)
     {
       stroke(0,0,0,0);
       fill(0,0,0,25);
       float factor =1.5;
       triangle((vertex[0].getV().x-c.x)*factor+c.x, (vertex[0].getV().y-c.y)*factor+c.y, (vertex[1].getV().x-c.x)*factor+c.x, (vertex[1].getV().y-c.y)*factor+c.y, 
                (vertex[2].getV().x-c.x)*factor+c.x, (vertex[2].getV().y-c.y)*factor+c.y);
     }
     
     if(drawCoords)
     {
       coords.init(c, new Vector(1,0), new Vector(0,1), 25,25);
       coords.draw();
     }
     
     stroke(100);
     fill(255,0);
     triangle(vertex[0].getV().x, vertex[0].getV().y, vertex[1].getV().x, vertex[1].getV().y, vertex[2].getV().x, vertex[2].getV().y);
     for(Vertex vert : vertex) 
       vert.draw();
     
     if(drawNormals)
     {
        setNormals();
        for(Object o : normalRays)
        {
          Ray r = (Ray)o;
          r.draw();
        }
     }
  }
  
  Vector getNormal(Vector p)
  {
    //Vector n = Vector.Normalize3(Vector.Minus(p, center));
    return null;
  }
  
  String save()
  {
    String movableS = isMovable ? "true" : "false";
    return "triangle:"+vertex[0].v.ToString()+";"+vertex[1].v.ToString()+";"+vertex[2].v.ToString()+";"+diffuse+";"+movableS;
  }
  
  void load(String s)
  {
    s = s.trim();
    String info = s.split(":")[1];
    String[] vals = info.split(";");
    isMovable = vals[4].equals("true");
    diffuse = Integer.parseInt(vals[3]);
    for(int i=0; i<3; i++)
    {
      vertex[i] = new Vertex();
      vertex[i].movable = isMovable;
      vertex[i].diffuse = diffuse;
      vertex[i].v.FromString(vals[i]);
    }
    setNormals();
  }
  
}
