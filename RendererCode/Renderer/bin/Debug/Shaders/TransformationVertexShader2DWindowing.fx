#version 130
#define M_PI 3.1415926535897932384626433832795

precision highp float;

in vec3 inPosition;
out vec2 pixelCoords;
void main(void)
{
  vec2 v = (inPosition.xy + 1) / vec2(2,4);

  vec2 ab = vec2(0.2,0.2);
  //vec2 AB = 
  
  mat3 translate1 = mat3(1,0,0,0,1,0,0,0.5,1);
  mat3 scale = mat3(0.5,0,0,0,0.5,0,0,0,1);
  mat3 translate2 = mat3(1,0,0,0,1,0,0,0.5,1);
  
  vec3 vh = vec3(v,1);

  vec3 mvh =  vh;


  gl_Position = vec4(mvh.xy, inPosition.z, 1);
  pixelCoords = vec2((inPosition.x + 1.0)/2.0, (inPosition.y + 1.0)/2.0);
}
