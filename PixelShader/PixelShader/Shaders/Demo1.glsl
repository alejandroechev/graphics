#version 130
//Demo: 1. Anatomy of a pixel shader
  
precision highp float;

in vec2 pixelCoords;
out vec4 pixelColor;


uniform vec2 mouse;
uniform float time;

void main(void)
{ 
  pixelColor = vec4(pixelCoords.x, pixelCoords.y, 1,1)
}
