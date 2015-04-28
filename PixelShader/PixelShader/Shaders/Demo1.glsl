#version 130
//Demo: 1. Anatomy of a pixel shader
  
precision highp float;

in vec2 pixelCoords;
out vec4 pixelColor;

void main(void)
{ 
  pixelColor = vec4(1, pixelCoords.x, pixelCoords.y, 1);
}
