class LevelManager
{
  ArrayList levels;
  int currentLevel;
  int numLevels = 4;
  String[] codes;
  
  LevelManager(int mode, PApplet app)
  {
    levels = new ArrayList();  
    if(mode == 0) //Editor
    {
      Level lev = new Editor();
      levels.add(lev);
      lev.init();
    }
    else if(mode == 1) //Sim
    {
      Level lev = new RaySimulation(app);
      levels.add(lev);
      lev.init();
    }
    else if(mode == 2) //Ex
    {
      codes = new String[]{"f92HwoevUh","2yj8a567p5","mEDME9xGHj"};
      
      Level lev = new RayExcercise(app, 0);
      levels.add(lev);
      lev.init();
      
      lev = new RayExcercise(app, 1);
      levels.add(lev);
      //lev.init();
      
      lev = new RayExcercise(app, 2);
      levels.add(lev);
      
    }
    else
    {
      Level lev = new PipelineLevel(app);
      levels.add(lev);
      lev.init();
    }
    currentLevel = 0;
  }
  
  void draw()
  {
    if(currentLevel < levels.size())
      currentLevel().draw();
  }
  
  void mouseWheel(int delta) {
    if(currentLevel < levels.size())
      currentLevel().mouseWheel(delta);
  }
  
  void mouseMoved()
  {
    if(currentLevel < levels.size())
      currentLevel().mouseMoved();
  }
  
  void mouseClicked()
  {
    if(currentLevel < levels.size())
      currentLevel().mouseClicked();
  }
    
  
  void mousePressed()
  {
     if(currentLevel < levels.size())
      currentLevel().mousePressed();
  }
  
  
  void mouseReleased()
  {
    if(currentLevel < levels.size())
      currentLevel().mouseReleased();
  }
  
  void keyPressed()
  {
     if(currentLevel < levels.size())
      currentLevel().keyPressed();
  }
  
  void controlEvent(ControlEvent ev)
  {
     if(currentLevel < levels.size())
      currentLevel().controlEvent(ev);
  }

  
  boolean next(String s)
  {
    if(currentLevel >= levels.size() - 1)
      return false;
    if(codes != null && !s.equals(codes[currentLevel]))
       return false;
    currentLevel().dispose();
    currentLevel++;
    currentLevel().init();
    return true;
  }
  
  Level currentLevel()
  {
    return (Level)levels.get(currentLevel);
  }
}
