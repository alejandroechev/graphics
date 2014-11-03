#version 130
#define PI 3.1415926535897932384626433832795

precision highp float;

in vec3 inPosition;
out vec2 pixelCoords;
out vec4 outPixelColor;

uniform float time;
uniform vec2 mouse;
uniform vec2 resolution;


mat4 getIdentity()
{
  return mat4(1,0,0,0, 0,1,0,0, 0,0,1,0, 0,0,0,1);
}



mat4 getScaling(float sx, float sy, float sz)
{
  return mat4(sx,0,0,0, 0,sy,0,0, 0,0,sz,0, 0,0,0,1);
}

mat4 getTranslation(float tx, float ty, float tz)
{
  return mat4(1,0,0,0, 0,1,0,0, 0,0,1,0, tx,ty,tz,1);
}

mat4 getRotationX(float angle)
{
  return mat4(1,0,0,0, 0,cos(angle),sin(angle),0, 0,-sin(angle),cos(angle),0, 0,0,0,1);
}

mat4 getRotationY(float angle)
{
  return mat4(cos(angle),0,-sin(angle),0, 0,1,0,0, sin(angle),0,cos(angle),0, 0,0,0,1);
}

mat4 getRotationZ(float angle)
{
  return mat4(cos(angle),sin(angle),0,0, -sin(angle),cos(angle),0,0, 0,0,1,0, 0,0,0,1);
}


mat4 getInteractive()
{
  return getScaling(0.2, 0.2, 0.2) * getRotationX(time / 100) * getRotationY(PI/4);
}



void main(void)
{
  vec4 v = vec4(inPosition.xyz,1);
  
  mat4 M = getInteractive();
  vec4 mv = M * v;
  
  gl_Position = mv;
  pixelCoords = vec2((inPosition.x + 1.0)/2.0, (inPosition.y + 1.0)/2.0);
  outPixelColor = vec4((inPosition + 1)/2,1);
}
