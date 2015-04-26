#version 130

precision highp float;

in vec2 pixelCoords;
out vec4 pixelColor;

uniform vec2 mouse;
uniform float time;

const int numberOfPartitions = 20;

float rand(vec2 seed){
    return fract(sin(dot(seed.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

float randNext(float previous){
	return rand(vec2(previous));
}

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
  pixelColor = vec4(1,1,1,1);
  int closestCircleIndex = -1;
  float distanceToClosestCircle = 100000;
  int closestCircleIndexForMouse = -1;
  float distanceToClosestCircleForMouse = 100000;
  Circle circles[numberOfPartitions]; 
  float randVal = 0.1;
  bool inCircle = false;
  for(int i=0; i<numberOfPartitions; i++)
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
	vec2 velocity = vec2(0,0);
	randVal = randNext(randVal);
	velocity.x = randVal/5 * cos(time);
	randVal = randNext(randVal);
	velocity.y = randVal/5 * cos(time);
	
	pos = pos + velocity;

	circles[i] = Circle(pos, 0.005, color, velocity);  

	if(testCircle(circles[i], pixelCoords))
	{
		pixelColor = vec4(0,0,0,1);
		inCircle = true;
	}

	float distanceToPixel = distance(pixelCoords, pos);
	if(distanceToPixel < distanceToClosestCircle)
	{
		distanceToClosestCircle = distanceToPixel;
		closestCircleIndex = i;
	}

	float distanceToMouse = distance(mouse, pos);
	if(distanceToMouse < distanceToClosestCircleForMouse)
	{
		distanceToClosestCircleForMouse = distanceToMouse;
		closestCircleIndexForMouse = i;
	}
  }

  if(!inCircle)
  {
    if(closestCircleIndex == closestCircleIndexForMouse)
		pixelColor = vec4(0,0,0,1);
	else
		pixelColor = circles[closestCircleIndex].color;
  }
}
