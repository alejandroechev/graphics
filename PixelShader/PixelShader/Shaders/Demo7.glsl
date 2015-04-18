#version 130
//Demo: 8. Randomness 2

precision highp float;

in vec2 pixelCoords;
out vec4 pixelColor;


uniform vec2 mouse;
uniform float time;

float rand(vec2 seed){
    return fract(sin(dot(seed.xy ,vec2(12.9898,78.233))) * 43758.5453);
}


void main(void)
{ 

  pixelColor = vec4(rand(pixelCoords), rand(pixelCoords), rand(pixelCoords),1);  
  
}
