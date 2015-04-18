#version 130
//Demo: 2. Attributes and uniforms
  
precision highp float;

in vec2 pixelCoords;
out vec4 pixelColor;


uniform vec2 mouse;
uniform float time;


void main(void)
{ 
  pixelColor = vec4(pixelCoords, 0.5 + 0.5*cos(time),1);  
}
