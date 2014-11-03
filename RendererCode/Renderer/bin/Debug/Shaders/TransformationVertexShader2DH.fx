#version 130
#define PI 3.1415926535897932384626433832795

precision highp float;

in vec3 inPosition;
out vec2 pixelCoords;

uniform float time;
uniform vec2 mouse;
uniform vec2 resolution;

mat3 getIdentity()
{
  return mat3(1,0,0, 0,1,0, 0,0,1);
}



mat3 getScaling(float sx, float sy)
{
  return mat3(sx,0,0, 0,sy,0, 0,0,1);
}

mat3 getSkewing(float kx, float ky)
{
  return mat3(1,ky,0, kx,1,0, 0,0,1);
}

mat3 getRotation(float angle)
{
  return mat3(cos(angle),sin(angle),0, -sin(angle),cos(angle),0, 0,0,1);
}

mat3 getTranslation(float tx, float ty)
{
  return mat3(1,0,0, 0,1,0, tx,ty,1);
}

mat3 getInteractive()
{
  vec2 t = (mouse / resolution - 0.5) * 2;
  return getTranslation(t.x,t.y);
}


mat3 getScaleRotateTranslate()
{
  return getTranslation(0.5,0.5) * getRotation(PI/4) * getScaling(0.5,0.5);
}








void main(void)
{
  vec2 v = inPosition.xy;


  vec3 vh = vec3(v,1);
  mat3 M =  getRotation(PI/4) * getRotation(PI/4);
  vec3 mvh =  M * vh;


  gl_Position = vec4(mvh.xy, inPosition.z, 1);
  pixelCoords = vec2((inPosition.x + 1.0)/2.0, (inPosition.y + 1.0)/2.0);
}
