#version 330

precision highp float;

struct Light {
  vec3 position;
  vec3 color;
};
uniform Light light0;

uniform vec3 backgroundColor;
uniform vec3 ambientColor;

uniform vec3 eye;

uniform sampler2D diffuseTexture;

vec3 blinnPhongShading(vec3 p, vec3 n, vec3 diffuse, vec3 specular, float shininess, vec3 lightPosition, vec3 lightColor, vec2 texCoords)
{
  vec3 v = eye - p;
  v = normalize(v);
  n =  perturb_normal(n, v, texCoords);
  vec3 l = lightPosition - p;
  l = normalize(l);
  vec3 h = v + l;
  h = normalize(h); 
  vec3 shadedColor = lightColor * (diffuse * max(dot(n,l),0.0) + specular * pow(max(dot(n,h),0.0), shininess));  
  return shadedColor;
}  

vec3 shade(vec3 p, vec3 n, vec3 diffuse, vec3 specular, float shininess, vec2 texCoords)
{
  vec3 color = blinnPhongShading(p,n,diffuse, specular, shininess, light0.position, light0.color, texCoords); 
  return ambientColor * diffuse + color;
}  

in vec3 fragPosition;
in vec3 fragDiffuseColor;
in vec3 fragNormal;
in vec3 fragSpecularColor;
in vec3 fragTexCoords;

out vec4 outFragColor;

void main(void)
{
  vec4 texColor = texture(diffuseTexture, fragTexCoords.xy);
  outFragColor = vec4(shade(fragPosition, fragNormal, texColor.xyz * fragDiffuseColor, fragSpecularColor, fragTexCoords.z, fragTexCoords.xy), 1.0);
}