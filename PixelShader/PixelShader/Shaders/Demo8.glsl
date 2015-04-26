#version 130
//Demo: 8. Randomness 2

precision highp float;

in vec2 pixelCoords;
out vec4 pixelColor;


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

float rand(vec2 seed){
    return fract(sin(dot(seed.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

float randNext(float previous){
	return rand(vec2(previous));
}

void main(void)
{ 

  pixelColor = vec4(pixelCoords, 0.5 + 0.5*cos(time),1);  
  Circle circles[100];
  float randVal = 0.1;
  for(int i=0; i<100; i++)
  {
	vec4 color = vec4(0,0,0,1);
	randVal = randNext(randVal);
	color.r = randVal;
	randVal = randNext(randVal);
	color.g = randVal;
	randVal = randNext(randVal);
	color.b = randVal;
	vec2 pos = vec2(0,0);
	randVal = randNext(randVal);
	pos.x = randVal;
	randVal = randNext(randVal);
	pos.y = randVal;
	float size = 0.05;
	randVal = randNext(randVal);
	size = size * randVal;

	circles[i] = Circle(pos, size, color, vec2(0,0));  
    if(testCircle(circles[i], pixelCoords))
      pixelColor = (0.5 + 0.5*cos(time)) * circles[i].color;
  }
}
