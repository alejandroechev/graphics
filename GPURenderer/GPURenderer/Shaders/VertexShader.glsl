/*
Vertex Shader:

El vertex shader se ejecutará para todos los vértices que fueron enviados desde la CPU a la GPU y almacenados en los distintos Vertex Buffer 
de esta.

Cada vértice puede tener asociado multiples atributos de entradas (en el codigo más abajo se identifican con la palabra clave "in" antes de la definicion).
Para que estos atributos lleguen, no basta con solo definirlos en el Vertex Shader, la información de los atributos de cada vértice debe ser enviada desde la
CPU a los vertex buffer GPU, y además se deben hacer las configuraciones correspondiente a través de la API de OpenGL para que cada atributo correcto
este asociado al vertex buffer que le corresponde (ver estos detalles en la clase MainWindow.cs)

En este renderer básico se pasan dos atributos por vértice, lo mínimo necesario para hacer viewing y shading: la posición y la normal del vértice.

El resultado del procesamiento

Además de los atributos, la CPU puede enviar información uniforme para todos los shader, los que se almacenan en variables con el modificador "uniform".
En este ejemplo se están pasando las matrices de viewing y la posición de la cámara de esta forma. Variables como la luz y el material son definidas localmente
en el shader, pero perfectamente podrían pasarse como uniforms.


*/
#version 130

precision highp float;

//Variables y estructuras locales
struct Light {
  vec3 position;
  vec3 color;
};
Light light = Light(vec3(0,0,10), vec3(1,1,1));

struct Material {
  vec3 diffuse;
  vec3 specular;
  float shininess;
};
Material material = Material(vec3(1,0,0), vec3(1,1,1), 100);

vec3 ambientColor = vec3(0.2,0.2,0.2);

//Uniforms
uniform vec3 cameraPosition;
uniform mat4 modelToWorld;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;

//Atributos de entrada
in vec3 inPosition;
in vec3 inNormal;

//Atributos de salida
out vec4 shadedColor;
//"out vec4 gl_position;" Este atributo viene dado por defecto, no hay que definirlo, pero es necesario almacenar en el la posición modificada del vértice

//Funciones internas
vec3 blinnPhongShading(vec3 p, vec3 n, Material material, Light light)
{
  n = normalize(n);
  vec3 v = normalize(cameraPosition - p);
  vec3 l = normalize(light.position - p);
  vec3 h = normalize(v + l);
  vec3 shadedColor = light.color * (material.diffuse * max(dot(n,l),0.0) + material.specular * pow(max(dot(n,h),0.0), material.shininess));  
  return shadedColor;
}  

vec4 shade(vec3 p, vec3 n)
{
  vec3 color = blinnPhongShading(p, n, material, light); 
  return vec4(ambientColor * material.diffuse + color, 1);
}  

//Codigo principal del shader
void main(void)
{
  vec4 worldPosition = modelToWorld * vec4(inPosition, 1); //se realiza primero la transofmración de modelo a mundo de la posición
  vec4 worldNormal = modelToWorld * vec4(inNormal, 0); //y de la normal
  shadedColor = shade(worldPosition.xyz, worldNormal.xyz); //se hace shading en coordenadas de mundo
  gl_Position = projectionMatrix * viewMatrix * worldPosition; //se hace el viewing de mundo a clipping
}