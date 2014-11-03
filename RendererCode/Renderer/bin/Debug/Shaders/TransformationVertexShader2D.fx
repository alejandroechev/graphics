#version 130
#define PI 3.1415926535897932384626433832795

precision highp float;

in vec3 inPosition;
out vec2 pixelCoords;

uniform float time;
uniform vec2 mouse;
uniform vec2 resolution;

mat2 getIdentity()
{
  return mat2(1,0, 0,1);
}



mat2 getScaling(float sx, float sy)
{
  return mat2(sx,0, 0,sy);
}

mat2 getSkewing(float kx, float ky)
{
  return mat2(1,ky, kx,1);
}

mat2 getRotation(float angle)
{
  return mat2(cos(angle),sin(angle), -sin(angle),cos(angle));
}

mat2 getInteractive()
{
  vec2 adjMouse = mouse / resolution;
  return getScaling(adjMouse.x, adjMouse.y) * getRotation(time / 100);
}










void main(void)
{
  vec2 v = inPosition.xy;


  mat2 M =  getInteractive();
  vec2 mv = M * v;





  gl_Position = vec4(mv, inPosition.z, 1);
  pixelCoords = vec2((inPosition.x + 1.0)/2.0, (inPosition.y + 1.0)/2.0);
}
