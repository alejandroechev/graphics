#version 130
//Demo: 1. Anatomy of a pixel shader
  
precision highp float;

in vec2 pixelCoords;
out vec4 pixelColor;

uniform vec2 mouse;
uniform float time;

const int number_of_iterations = 100;
const vec3 colorIn = vec3(1,0.75,0.5);
const vec3 colorOut = vec3(0,0.5,1);

vec2 iteration(vec2 z, vec2 c)
{
	return vec2(z.x*z.x - z.y*z.y + c.x, 2*z.x*z.y + c.y);
}

void main(void)
{ 
  vec2 c = 4 * pixelCoords  - 2;
  float zoom = 1/pow((time + 1),1);
  vec2 mouseCorrected = 4 * mouse - 2;
  
  c = zoom * (c - (1/zoom) * mouseCorrected);	
  
  vec2 z = vec2(0);
  
  float finalIt = number_of_iterations;
  for(int it = 0; it < number_of_iterations; it++)
  {
	z = iteration(z, c);
	if(length(z) > 2)
		finalIt = it;
  }

  float interpolator = (number_of_iterations - finalIt)/number_of_iterations;
  vec3 color = mix(colorOut, colorIn, interpolator);
  pixelColor = vec4(color, 1);
}
