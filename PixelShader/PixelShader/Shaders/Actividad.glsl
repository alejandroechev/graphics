#version 130

precision highp float;

in vec2 pixelCoords;
out vec4 pixelColor;

uniform vec2 mouse;
uniform float time;

const bool showPoints = true;
const bool animate = true;
const int numberOfPartitions = 10;










float rand(vec2 seed){
    return fract(sin(dot(seed.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

vec2 rand2(vec2 seed){
	return fract(sin(vec2(dot(seed,vec2(127.1,311.7)),dot(seed,vec2(269.5,183.3))))*43758.5453);
}

vec3 rand3(vec2 seed){
	return fract(sin(vec3(dot(seed,vec2(123.1,314.7)),dot(seed,vec2(219.5,83.3)),dot(seed,vec2(49.5,223.3))))*43758.5453);
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
  Circle circles[numberOfPartitions * numberOfPartitions]; 
  float randVal = 0.1;
  bool inCircle = false;
  int index = 0;
  for(int i=0; i<numberOfPartitions; i++)
  {
    for(int j=0; j<numberOfPartitions; j++)
	{
		vec2 seed = vec2(i+0.5,j+0.5);
		float randValue = rand(seed);
		vec4 color = vec4(rand3(seed),1);
		vec2 pos = rand2(seed);
		vec2 velocity = vec2(0,0);
		velocity.x = randValue/5 * cos(time);
		velocity.y = randValue/5 * sin(time);
	
		if(animate)
			pos = pos + velocity;

		circles[index] = Circle(pos, 0.005, color, velocity);  

		if(testCircle(circles[index], pixelCoords) && showPoints)
		{
		    pixelColor = vec4(0,0,0,1);
			inCircle = true;
		}

		float distanceToPixel = distance(pixelCoords, pos);
		if(distanceToPixel < distanceToClosestCircle)
		{
			distanceToClosestCircle = distanceToPixel;
			closestCircleIndex = index;
		}

		float distanceToMouse = distance(mouse, pos);
		if(distanceToMouse < distanceToClosestCircleForMouse)
		{
			distanceToClosestCircleForMouse = distanceToMouse;
			closestCircleIndexForMouse = index;
		}
		index++;
	}
  }

  if(!inCircle)
  {
    vec4 color = circles[closestCircleIndex].color;
    if(closestCircleIndex == closestCircleIndexForMouse)
		pixelColor = vec4(0,0,0,1);
	else
		pixelColor = color;
  }
}
