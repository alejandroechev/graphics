class Label implements IControl
{
  String textString;
  Vector position;
  color col;
  
  Label(Vector p, color c, String t)
  {
    position = p;
    col = c;
    textString = t;
  }
  
  void draw()
  {
    fill(col);
    text(textString, position.x, position.y);
  }
  
  void click(float x, float y, int button)
  {
  }
}
