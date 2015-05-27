#version 330

precision highp float;

in vec4 shadedColor;
out vec4 outFragColor;
      
void main(void)
{
  outFragColor = shadedColor;
}