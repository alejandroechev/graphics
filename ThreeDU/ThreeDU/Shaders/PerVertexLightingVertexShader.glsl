#version 330

precision highp float;

struct Light {
  vec3 position;
  vec3 color;
};
Light light0 = Light(vec3(0,0,1), vec3(1,1,1));

vec3 backgroundColor;
vec3 ambientColor = vec3(0.2,0.2,0.2);
vec3 eye = vec3(0,0,1);

mat4 projectionMatrix;
mat4 viewMatrix;
mat4 modelMatrix;

in vec3 inPosition;
in vec3 inDiffuseColor;
//in vec3 inNormal;
//in vec3 inSpecularColor;
//in vec3 inTexCoords;

out vec4 shadedColor;

vec3 blinnPhongShading(vec3 p, vec3 n, vec3 diffuse, vec3 specular, float shininess, vec3 lightPosition, vec3 lightColor)
{
  n = normalize(n);
  vec3 v = eye - p;
  v = normalize(v);
  vec3 l = lightPosition - p;
  l = normalize(l);
  vec3 h = v + l;
  h = normalize(h); 
  vec3 shadedColor = lightColor * (diffuse * max(dot(n,l),0.0) + specular * pow(max(dot(n,h),0.0), shininess));  
  return shadedColor;
}  

vec3 shade(vec3 p, vec3 n, vec3 diffuse, vec3 specular, float shininess)
{
  vec3 color = blinnPhongShading(p,n,diffuse, specular, shininess, light0.position, light0.color); 
  return ambientColor * diffuse + color;
}  


void main(void)
{
  shadedColor = vec4(shade(inPosition, vec3(0,0,1), inDiffuseColor, vec3(0,0,0), 1), 1.0);
  gl_Position = vec4(inPosition, 1);
}