#version 130
//Demo: 6. Mouse interaction

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
  vec2 velocity;
};

bool testCircle(Circle c, vec2 pixelCoords)
{
  return distance(c.position,pixelCoords) < c.radius;
}

void main(void)
{ 

  pixelColor = vec4(0,0,1,1);
  Circle circles[3];
  circles[0] = Circle(vec2(0.1,0.1), 0.05, vec4(1,1,0,1), vec2(0,0));
  circles[1] = Circle(vec2(0.1,0.4), 0.02, vec4(0,1,1,1), vec2(0.01,0.0));
  circles[2] = Circle(vec2(0.1,0.7), 0.033, vec4(1,0,0,1), vec2(0.005,0.0));
  
  circles[0].position = mouse/resolution;
  
  for(int i=0; i<3; i++)
  {
    circles[i].position= circles[i].position + circles[i].velocity * time;
    if(testCircle(circles[i], pixelCoords))
      pixelColor = circles[i].color;
  }
}
