#version 130
//Demo: 4. Circle as object

precision highp float;

in vec2 pixelCoords;
out vec4 pixelColor;


uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;

struct Circle {
  vec2 position;
  float radius;
  vec4 color;
};

bool testCircle(Circle c, vec2 pixelCoords)
{
  return distance(c.position,pixelCoords) < c.radius;
}

void main(void)
{ 
  pixelColor = vec4(0,0,1,1);
  Circle c1 = Circle(vec2(0.5,0.5), 0.1, vec4(1,0,0,1));
  if(testCircle(c1, pixelCoords))
    pixelColor = c1.color;
}
