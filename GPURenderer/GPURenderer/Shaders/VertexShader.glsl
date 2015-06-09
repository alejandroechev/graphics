#version 330

precision highp float;

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

uniform vec3 ambientColor;

uniform vec3 cameraPosition;
uniform mat4 modelToWorld;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;

in vec3 inPosition;
in vec3 inNormal;

out vec4 shadedColor;

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


void main(void)
{
  vec4 worldPosition = modelToWorld * vec4(inPosition, 1);
  vec4 worldNormal = modelToWorld * vec4(inNormal, 0);
  shadedColor = shade(worldPosition.xyz, worldNormal.xyz);
  gl_Position = projectionMatrix * viewMatrix * worldPosition;
}