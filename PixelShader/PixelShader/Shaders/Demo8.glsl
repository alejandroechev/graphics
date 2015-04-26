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

vec2 rand2(vec2 seed){
	return fract(sin(vec2(dot(seed,vec2(127.1,311.7)),dot(seed,vec2(269.5,183.3))))*43758.5453);
}

vec3 rand3(vec2 seed){
	return fract(sin(vec3(dot(seed,vec2(123.1,314.7)),dot(seed,vec2(219.5,83.3)),dot(seed,vec2(49.5,223.3))))*43758.5453);
}

void main(void)
{ 

  pixelColor = vec4(pixelCoords, 0.5 + 0.5*cos(time),1);  
  float randVal = 0.5;
  int numRows = 8;
  for(int i=0; i<numRows; i++)
  {
    for(int j=0; j<numRows; j++)
	{
		vec2 seed = vec2(i,j);
		vec4 color = vec4(rand3(seed),1);
		vec2 pos = rand2(seed);
		float size = 0.01;
		
		Circle circle = Circle(pos, size, color, vec2(0,0));  
		if(testCircle(circle, pixelCoords))
		  pixelColor = (0.5 + 0.5*cos(time)) * circle.color;
	}
  }
}
