/* Displays a coordinate system in a specific origin and with two basis
*/
class CoordinateDisplay
{
  //Coordinate system info
  Vector origin, dirX, dirY;
  float xSize, ySize;
  
  //Arrows to draw coordinate basis
  Arrow x;
  Arrow y;
  Arrow xMinus;
  Arrow yMinus;
  
  //Stores the vertexes associated to this coordinate system
  ArrayList vertexes;
  
  //For drawing dotted lines
  BasicStroke dotted;
  BasicStroke regular;
  float[] dots = { 1.0f, 4.0f, 1.0f, 4.0f };
  
  //Configurable parameteres
  boolean showMinusAxis = true;
  
  CoordinateDisplay(Vector origin, Vector dirX, Vector dirY, float xSize, float ySize)
  {   
    vertexes = new ArrayList();
    dotted = new BasicStroke(1.0f, BasicStroke.CAP_ROUND, BasicStroke.JOIN_MITER,
	4.0f, dots, 0.0f);
    regular = new BasicStroke(1.0f);
    
    init(origin, dirX, dirY, xSize, ySize);
  }
  
  CoordinateDisplay()
  {
    this(new Vector(), new Vector(), new Vector(), 45,45);
  }
  
  void init(Vector _origin, Vector _dirX, Vector _dirY, float _xSize, float _ySize)
  {
    origin = _origin;
    dirX = _dirX;
    dirY = _dirY;
    xSize = _xSize;
    ySize = _ySize;
    
    x = new Arrow(origin, dirX, xSize);
    y = new Arrow(origin, dirY, ySize);
    xMinus = new Arrow(origin, Vector.Multiply(-1,dirX), xSize);
    yMinus = new Arrow(origin, Vector.Multiply(-1,dirY), ySize);
    
  }
  
  void draw()
  { 
    x.draw();
    y.draw();
    if(showMinusAxis)
    {
      xMinus.draw();
      yMinus.draw();
    }
    
    Graphics2D g2 = ((PGraphicsJava2D) g).g2;
    g2.setStroke(dotted);
    for(Object o : vertexes)
    {
      Vertex v = (Vertex)o;
      stroke(200);
      line(x.pos.x, x.pos.y, v.getV().x, v.getV().y);
    }
    g2.setStroke(regular);
  }
}
