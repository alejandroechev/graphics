#version 130

precision highp float;

in vec2 pixelCoords;
out vec4 pixelColor;

void main(void)
{  
  pixelColor = vec4(pixelCoords.x, pixelCoords.y, 1,1);
}
