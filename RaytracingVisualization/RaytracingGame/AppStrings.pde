static class AppStrings
{
  static HashMap<String, String> english;
  static HashMap<String, String> spanish;
  static HashMap<String, HashMap<String, String>> languageSelector;
  static String currentLanguage = "en";
  
  static 
  {
    english = new HashMap<String, String>();
    spanish = new HashMap<String, String>();
    languageSelector = new HashMap<String, HashMap<String, String>>();
    languageSelector.put("en", english);
    languageSelector.put("es", spanish);
        
   english.put("activity","Activity");
    spanish.put("activity","Actividad");
    english.put("iterations","Iterations");
    spanish.put("iterations","Iteraciones");
    english.put("instructions","Instructions");
    spanish.put("instructions","Instrucciones");
    english.put("scene","Scene");
    spanish.put("scene","Escena");
    english.put("rays","Rays");
    spanish.put("rays","Rayos");
    english.put("fov","FOV");
    spanish.put("fov","FOV");
    english.put("position","Position");
    spanish.put("position","Posicion");
    english.put("target","Target");
    spanish.put("target","Target");
    english.put("up","Up");
    spanish.put("up","Up");
    english.put("display","Display");
    spanish.put("display","Display");
    english.put("pixels","Pixels");
    spanish.put("pixels","Pixeles");
    english.put("submit","Submit Answer");
    spanish.put("submit","Responder");
    english.put("next","Next Activity");
    spanish.put("next","Siguiente");
    english.put("correct","Correct!");
    spanish.put("correct","Correcto!");
    english.put("incorrect","Incorrect, try again.");
    spanish.put("incorrect","Incorrecto, intenta denuevo.");
    english.put("tools","Tools");
    spanish.put("tools","Herramientas");
    english.put("protractor","Protractor");
    spanish.put("protractor","Transportador");
    english.put("ruler","Ruler");
    spanish.put("ruler","Regla");
    english.put("lines","Draw Lines");
    spanish.put("lines","Dibujar Lineas");
    english.put("eraser","Eraser");
    spanish.put("eraser","Borrador");
    english.put("eraseAll","Erase All Lines");
    spanish.put("eraseAll","Borrar Todo");
    english.put("lineColor","Line Color");
    spanish.put("lineColor","Color Linea");
    english.put("circle","Circle");
    spanish.put("circle","Circulo");
    english.put("radius","Radius");
    spanish.put("radius","Radio");
    english.put("shininess","Shininess");
    spanish.put("shininess","Shininess");
    english.put("reflectiveness","Reflectiveness");
    spanish.put("reflectiveness","Reflectividad");
    english.put("diffuse","Diffuse Color");
    spanish.put("diffuse","Color Difuso");
    english.put("specular","Specular Color");
    spanish.put("specular","Color Especular");
    english.put("ambient","Ambient Light");
    spanish.put("ambient","Luz Ambiente");
    english.put("addCircle","Add Circle");
    spanish.put("addCircle","Agregar Circulo");
    english.put("light","Light");
    spanish.put("light","Luz");
    english.put("lightColor","Color");
    spanish.put("lightColor","Color");
    english.put("attenuationLinear","Linear Attenuation ");
    spanish.put("attenuationLinear","Atenuacion Lineal");
    english.put("attenuationQuad","Quadratic Attenuation");
    spanish.put("attenuationQuad","Atenuacion Cuadratica");
    english.put("addLight","Add Light");
    spanish.put("addLight","Agregar Luz");
    english.put("delete","Delete Circle/Light");
    spanish.put("delete","Borrar Circulo/Luz");
    english.put("options","Options");
    spanish.put("options","Opciones");
    english.put("showEyeRays","Show Eye Rays");
    spanish.put("showEyeRays","Rayos de Vision");
    english.put("showShadowRays","Show Shadow Rays");
    spanish.put("showShadowRays","Rayos de Sombra");
    english.put("showReflectedRays","Show Reflected Rays");
    spanish.put("showReflectedRays","Rayos de Reflexion");
    english.put("showNormals","Show Normals");
    spanish.put("showNormals","Normales");
    english.put("showHalfVectors","Show Half Vectors");
    spanish.put("showHalfVectors","Vectores Medios");
    english.put("showDisplay","Show Display");
    spanish.put("showDisplay","Display");
    english.put("showImagePlane","Show Image Plane");
    spanish.put("showImagePlane","Plano de Imagen");
    english.put("showCameraGUI","Show Camera GUI");
    spanish.put("showCameraGUI","Interfaz Camara");
    english.put("pipelineStages","Pipeline Stages");
    spanish.put("pipelineStages","Etapas del Pipeline");
  }
  
  static String getString(String key)
  {
    if(!languageSelector.containsKey(currentLanguage))
      return null;
    HashMap<String, String> lang = languageSelector.get(currentLanguage);
    if(!lang.containsKey(key))
      return null;
    return lang.get(key);
    
  }
}
