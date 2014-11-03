#version 130

precision highp float;

in vec2 pixelCoords;
out vec4 pixelColor;

const float infinity = 1. / 0.;
const float eps = 0.00001;
const int numLights = 1;

uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;


struct Sphere {
  vec3 position;
  float radius;
  vec4 diffuse;
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

Sphere spheres[3];
Light lights[1];
Camera camera = Camera(vec3(0.5, 5, -10));
Scene scene = Scene(vec4(0.4,0.4,0.4,1), vec4(0.01,0.01,0.01,1));

//ray-sphere intersection
float intersectSphere(Ray ray, Sphere sphere)
{
  float a = dot(ray.direction, ray.direction);
  float b = dot(ray.position - sphere.position, ray.direction);
  float c = dot(ray.position - sphere.position, ray.position - sphere.position) - sphere.radius*sphere.radius;

  float discr = b*b - a*c;
  if(discr < 0.0)
    return infinity;

  discr = sqrt(discr);
  float t0 = (-b - discr) / a;
  float t1 = (-b + discr) / a;

  float tMin = min(t0, t1);
  if(tMin < 0.0)
    return infinity;

  return tMin;
}

float intersectAll(Ray ray, inout int minSphere)
{
  float tMin = infinity;
  for(int i = 0; i < 3; i++)
  {
    float t = intersectSphere(ray, spheres[i]);
    if(t < tMin)
    {
      tMin = t;
      minSphere = i;
    }
  }
  return tMin;
}

//Lambert shading
vec4 lambertShading(vec3 p, vec3 n, vec4 diffuse)
{
  vec3 l = vec3(0.5,0,-0.5); //lights[0].position - p;
  float dl = length(l);
  l = normalize(l);  
  
  vec3 shp = p + 0.01*l;
  Ray shadow = Ray(shp, l);
  int minSphere = -1;
  float tMin = intersectAll(shadow, minSphere);

  vec4 shadedColor = scene.ambient * diffuse; 
  if(tMin > dl)
    shadedColor = shadedColor + lights[0].color * (max(dot(n,l),0) * diffuse);
 
  return shadedColor;
}  

bool isInShadow(vec3 p, Sphere sphere)
{
  for(int i=0; i<numLights; i++)
  {
    vec3 shadowDir = normalize(lights[i].position - p);
    Ray shadowRay = Ray(p + 0.1 * shadowDir, shadowDir);    
    float tShadow = intersect(shadowRay, sphere);
    if(!isinf(tShadow))
      return true;
  }
  return false;
}


vec4 rayTrace(Ray ray)
{
  int minSphere = -1;
  float tMin = intersectAll(ray, minSphere);
  if(tMin >= infinity)
    return scene.background;

  vec3 p = ray.position + tMin*ray.direction;
  vec3 n = normalize(p - spheres[minSphere].position);

  return lambertShading(p, n, spheres[minSphere].diffuse);
}


void init()
{ 
  spheres[0] = Sphere(vec3(0.5, 0.5, 0.5), 0.07, vec4(0.656,0.626,0.107,1));
  spheres[1] = Sphere(vec3(0.01, 0.5, 0.0), 0.02, vec4(0.639, 0.06, 0.062, 1));
  spheres[2] = Sphere(vec3(0.25, 0.5, 0.0), 0.03, vec4(0.156, 0.426, 0.107, 1));

  lights[0] = Light(vec3(0.0,0.5,0.0),  vec4(0.8,0.7,0.6,1));
}

void main(void)
{
  vec2 normalizedMouse = mouse/resolution;
  camera.position = vec3(0.5, 5 * normalizedMouse.y, -10);

  //Initialize elements
  init();

  //Ray definition
  vec3 pixel = vec3(pixelCoords.x, pixelCoords.y, 0);
  Ray ray = Ray(camera.position, normalize(pixel - camera.position));

  spheres[1].position = vec3(spheres[0].position.x + 0.2*cos(0.04*time), 0.5, spheres[0].position.z + 0.2*sin(0.04*time));
  spheres[2].position = vec3(spheres[0].position.x + 0.4*cos(0.06*time), 0.5, spheres[0].position.z + 0.4*sin(0.06*time));


  //Raytrace objects
  pixelColor = rayTrace(ray);  
}
