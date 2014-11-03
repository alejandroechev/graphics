#version 130

precision highp float;



uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;

in vec3 inPosition;
in vec3 inDiffuseColor;
in vec3 inNormal;
in vec3 inSpecularColor;
in vec3 inTexCoords;

out vec3 fragPosition;
out vec3 fragDiffuseColor;
out vec3 fragNormal;
out vec3 fragSpecularColor;
out vec3 fragTexCoords;

uniform sampler2D displacementMap; 
uniform float time;




void main(void)
{
  fragPosition = inPosition;
  fragDiffuseColor = inDiffuseColor;
  fragNormal = inNormal;
  fragSpecularColor = inSpecularColor;
  fragTexCoords = inTexCoords;
  gl_Position = projectionMatrix * viewMatrix * vec4(fragPosition, 1);
}