class Line
{
  Vector v1, v2;
  color col;
  
  Line(Vector _v1, Vector _v2, color _col)
  {
    v1 = _v1;
    v2 = _v2;
    col = _col;
  }
  
  void draw()
  {
    fill(col);
    stroke(col);
    line(v1.x, v1.y, v2.x, v2.y);
  }
  
  Ray getRay()
  {
    Ray r = new Ray(v1, Vector.Minus(v2,v1));
    //r.firstT = v1.Distance3(v2);
    return r;
  }
  
  
  
}
