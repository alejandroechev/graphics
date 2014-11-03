#version 130

precision highp float;

in vec2 pixelCoords;
out vec4 pixelColor;

const float infinity = 1. / 0.;
const float eps = 0.00001;
const int numLights = 1;
const int numSpheres = 2;

uniform vec2 resolution;
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

Sphere spheres[2];
Material materials[6];
Light lights[1];
Camera camera = Camera(vec3(0, 10, -10));
Scene scene = Scene(vec4(0.2,0.2,0.2,1), vec4(0.8,0.8,1.0,1));

float deltaT = 0.1;
float minT = 17.0;
float maxT = 18.0;


//Lambert shading
//vec4 lambertShading(vec3 p, vec3 n, Material material)
//{
//  vec3 l = lights[i].position - p;
//  vec4 shadedColor = scene.ambient * material.diffuse;
//  shadedColor = shadedColor + lights[0].color * (dot(n,l) * material.diffuse);
//  return shadedColor;
//} 

//Blinn phong shading
vec4 blinnPhongShading(vec3 p, vec3 n, Material material)
{
  n = normalize(n);
  vec3 v = camera.position - p;
  v = normalize(v);

  vec4 shadedColor = scene.ambient * material.diffuse;
  for(int i=0; i<numLights; i++)
  {
    vec3 l = lights[i].position - p;
    l = normalize(l);  
    vec3 h = v + l;
    h = normalize(h);

    shadedColor = shadedColor + lights[i].color * (dot(n,l) * material.diffuse + pow(dot(n,h), material.shininess) * material.specular);	  
  }
  return shadedColor;
}  


bool intersectSphere(vec3 p, Sphere sphere)
{
  return length(p - sphere.position) < sphere.radius;
}

vec4 rayMarch(Ray ray)
{
  for(float t = minT; t < maxT; t += deltaT)
  {
    vec3 p = ray.position + t*ray.direction;
    vec3 d = (p - spheres[0].position);
    vec3 wp = p;
    //if(intersectSphere(p, spheres[0]))
    //{
    //  vec3 n = normalize(p - spheres[0].position);
    //  return blinnPhongShading(p, n, materials[spheres[0].materialIndex]);
    //}
    if(intersectSphere(wp, spheres[1]))
    {
      vec3 n = normalize(wp - spheres[1].position);
      return blinnPhongShading(wp, n, materials[spheres[1].materialIndex]);
    }
  }
  return scene.background;
}



void init()
{ 

  materials[0] = Material(vec4(0.156,0.126,0.507,1), vec4(1,1,1,1), 100);   // Blue specular
  materials[1] = Material(vec4(0.656,0.626,0.107,1), vec4(1,1,1,1), 1000);     // Yellow
  materials[2] = Material(vec4(0.739, 0.725, 0.765, 1), vec4(1,1,1,1), 1000);  // White
  materials[3] = Material(vec4(0.639, 0.06, 0.062, 1), vec4(1,1,1,1), 1000);   // Red
  materials[4] = Material(vec4(0.156, 0.426, 0.107, 1), vec4(0,0,0,0), 1);  // Green
  materials[5] = Material(vec4(0.5, 0.2, 0.0, 1), vec4(0,0,0,0), 1);  // Brown

  lights[0] = Light(vec3(0.0,2.0,0.0),  vec4(0.8,0.7,0.6,1));

  spheres[0] = Sphere(vec3(0,0,0.5), 0.4, 1);
  spheres[1] = Sphere(vec3(0,0,7.5), 0.5, 0);
}

void main(void)
{
  //Initialize elements
  init();

  //Interactivity and real time stuff
  vec2 adjustedMouse =  vec2((2.0 * mouse.x / resolution.x) - 1.0, (2.0 * (resolution.y - mouse.y) / resolution.y) - 1.0);
  lights[0].position = vec3(0, 0, -10.0);  //Y is flipped
  camera.position = vec3(0, 0, -10.0);

  //Ray definition
  vec3 pixel = vec3(2.0*pixelCoords.x - 1.0, 2.0*pixelCoords.y - 1.0, 0);
  Ray ray = Ray(camera.position, normalize(pixel - camera.position));

  //spheres[1].position = spheres[0].position + vec3(cos(time), 0,sin(time));

  //Raymarch objects
  pixelColor = rayMarch(ray);
}
