interface IMovable
{
  boolean isMovable();
  void setPosition(Vector p);
  Vector getPosition();
  boolean isInside(Vector p);
  Vector labelPosition();
  String label();
  void setSelected(boolean isSel);
  void flipSelected();
  void setFirstMouse(Vector p);
}
