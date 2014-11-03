  static class PipelineStages
  {
    static int None = -1;
    static int Init = 0;
    static int WoorldCoords = 1;
    static int CameraCoords = 2;
    static int Shading = 3;    
    static int Perspective = 4;
    static int ProjectionCoords = 5;
    static int Clipping = 6;
    static int ViewportCoords = 7;
    static int Rasterization = 8;
    static int TextureMapping = 9;
    static int Visibility = 10;
    
    static String[] StagesNames = new String[]{"Triangles","Model to World", "World to Camera", "Shading", "Persective Transform", "Camera to Clipping", 
                    "Clipping", "Viewport Transform", "Rasterization", "Texture Mapping", "Visibility"};
  }
