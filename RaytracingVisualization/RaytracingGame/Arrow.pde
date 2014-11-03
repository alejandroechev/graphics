class Arrow
{
   Vector pos, dir;
   color col;
   float arrowSize = 45;
   float pointSize = 5;
   
   Arrow(Vector _pos, Vector _dir, float _arrowSize)
   {     
     pos = _pos;
     dir = _dir;
     arrowSize = _arrowSize;
     col = color(128);
   }
   
   Arrow()
   {
     this(new Vector(), new Vector(), 45);     
   }
   
   void draw()
   {
      stroke(col);
      fill(col);
      Vector end = Vector.Add(pos, Vector.Multiply(arrowSize, dir));
      line(pos.x, pos.y, end.x, end.y);
      
      
      Vector dirPerp1 = new Vector(-dir.y, dir.x);
      Vector dirPerp2 = new Vector(dir.y, -dir.x);
      Vector arrowPoint1 = Vector.Add(end, Vector.Multiply(pointSize, dir));
      Vector arrowPoint2 = Vector.Add(end, Vector.Multiply(pointSize/2, dirPerp1));
      Vector arrowPoint3 = Vector.Add(end, Vector.Multiply(pointSize/2, dirPerp2));
      
      triangle(arrowPoint1.x, arrowPoint1.y, arrowPoint2.x, arrowPoint2.y, arrowPoint3.x, arrowPoint3.y); 
   }
}
