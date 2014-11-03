import controlP5.*;
import java.awt.BasicStroke;
import java.awt.Graphics2D;


LevelManager levelManager;
NextButton next;

int gameMode = 1;


void setup()
{
  smooth();
  size(800,600);
  //size(int(0.8*screen.width), int(0.8*screen.height));
  /*addMouseWheelListener(new java.awt.event.MouseWheelListener() { 
    public void mouseWheelMoved(java.awt.event.MouseWheelEvent evt) { 
      mouseWheel(evt.getWheelRotation());
  }});*/ 
  
  
  PFont font;
  font = loadFont("Calibri-48.vlw"); 
  textFont(font, 11); 
  
  levelManager = new LevelManager(gameMode, this);
  if(gameMode == 2)
    next = new NextButton(new Vector(width - 10, 10), 24,24, "data//arrow.png", "data//arrowR.png", true, levelManager, this);
}



void draw()
{
  levelManager.draw();
  if(gameMode == 2)
    next.draw();
}

void mouseWheel(int delta) {
  //println(delta);
  levelManager.mouseWheel(delta);
}

void mouseMoved()
{
  levelManager.mouseMoved();
}

void mouseClicked()
{
  levelManager.mouseClicked();
  if(gameMode == 2)
    next.click(mouseX, mouseY, mouseButton);
}
  

void mousePressed()
{
   levelManager.mousePressed();
}


void mouseReleased()
{
  levelManager.mouseReleased();
}

void keyPressed()
{
  levelManager.keyPressed();
}

public void controlEvent(ControlEvent theEvent) {
  if(levelManager != null)
    levelManager.controlEvent(theEvent);
}


