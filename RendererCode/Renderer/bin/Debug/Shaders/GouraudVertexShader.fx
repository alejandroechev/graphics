#version 130

precision highp float;

struct Light {
  vec3 position;
  vec3 color;
};
uniform Light light0;

uniform vec3 backgroundColor;
uniform vec3 ambientColor;

uniform vec3 eye;

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;

in vec3 inPosition;
in vec3 inDiffuseColor;
in vec3 inNormal;
in vec3 inSpecularColor;
in vec3 inTexCoords;

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
  shadedColor = vec4(shade(inPosition, inNormal, inDiffuseColor, inSpecularColor, inTexCoords.z), 1.0);
  gl_Position = projectionMatrix * viewMatrix * vec4(inPosition, 1);
}