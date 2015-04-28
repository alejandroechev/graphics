#version 130

precision highp float;

in vec2 pixelCoords;
out vec4 pixelColor;

const float infinity = 1. / 0.;
const vec2 resolution = vec2(512,512);

uniform vec2 mouse;
uniform float time;


struct Sphere {
  vec3 position;
  float radius;
  int materialIndex;
};

struct Material {
  vec4 diffuse;
  vec4 specular;
  float shininess;
};

struct Light {
  vec3 position;
  vec4 color;
};

struct Ray {
  vec3 position;
  vec3 direction;
};

struct Camera {
  vec3 position;
};

struct Scene {
  vec4 ambient;
  vec4 background;
};




void main(void)
{  
  pixelColor = vec4(pixelCoords,1,1);
}
