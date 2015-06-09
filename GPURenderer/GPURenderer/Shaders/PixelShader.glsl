/*Pixel Shader: 

  El pixel shader se ejecutará para todos los pixeles generados al rasterizar. 

  A cada pixel le pueden llegar 1 a N propiedades interpoladas de los vértices del triángulo raserizado. 
  Estas propiedades deben estar definidas como atributos de entrada en el Pixel Shader (i.e. variables que empiezan con la palabra clave in)
  y asu vez como atributos de salida del Vertex Shader asociado (i.e. variables que empieza con la palabra clave out).

  Cada atributo out del veretex shader debe tener su correspondiente atributo in en el pixel shader, y debe tener el mismo nombre.

  En este caso el unico atributo in del pixel shader es "vec4 shadedColor" que coressponderá a la interpolación baricentrica del color de 
  shading calculado en cada vértice del triángulo que al rasterizar generó este pixel.

  El pixel shader en su cuerpo puede realizar distintos tipos de procesamiento para modificar el color que finalmente se entregará como 
  resultado para este pixel. En particular obtener el color de texturas para combinar con el color de shading.

  En este ejemplo el pixel shader "no hace nada", lo que se conoce como "pass through" osea deja pasar el color recibido tal cual. Para hacer
  esto se defien un atributo de salida denominado outFragColor que obtiene el valor de shadedColor
*/
#version 130

precision highp float;

in vec4 shadedColor;
out vec4 outFragColor;
      
void main(void)
{
  outFragColor = shadedColor;
}