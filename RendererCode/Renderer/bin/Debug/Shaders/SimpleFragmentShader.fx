#version 130
  
precision highp float;

in vec2 pixelCoords;
in vec4 outPixelColor;
out vec4 pixelColor;


uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;

void main(void)
{ 
  pixelColor = outPixelColor;
}
