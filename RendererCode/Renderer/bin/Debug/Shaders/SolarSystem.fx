#version 130

precision highp float;

in vec2 pixelCoords;
out vec4 pixelColor;


uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;

struct Sun {
  vec2 position;
  float radius;
  vec4 color;
};

struct Planet {
  float radius;
  float orbitDistance;
  float orbitSpeed;
  vec4 color;
};

Sun sun = Sun(vec2(0.5,0.5), 0.1, vec4(1,1,0,1));
Planet planets[3];


bool isInside(vec2 position, float radius, vec2 pixelCoords)
{
  return distance(position,pixelCoords) < radius;
}

void main(void)
{ 
  
  pixelColor = vec4(0,0,0,1);

  planets[0] = Planet(0.04,  0.27, 0.3, vec4(1.0,0.5,0,1));
  planets[1] = Planet(0.02,  0.38, 0.2, vec4(0,0,1,1));
  planets[2] = Planet(0.023, 0.45, 0.1, vec4(1,0,0,1));
  
  if(isInside(sun.position, sun.radius, pixelCoords))
    pixelColor = sun.color;
  else
  {
    for(int i=0; i<3; i++)
    {
      float speed = planets[i].orbitSpeed;
      if(isInside(sun.position, sun.radius, mouse/resolution))
        speed = 0.0;
      vec2 position = sun.position + vec2(planets[i].orbitDistance*cos(speed * time), 
        planets[i].orbitDistance * sin(speed * time)); 
      if(isInside(position, planets[i].radius, pixelCoords))
        pixelColor = planets[i].color;
    }
  }
}
