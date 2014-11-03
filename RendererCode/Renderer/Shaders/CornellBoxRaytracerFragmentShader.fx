#version 130

precision highp float;

in vec2 pixelCoords;
out vec4 pixelColor;

const float infinity = 1. / 0.;
const int numLights = 1;

vec2 resolution = vec2(512,512);
uniform vec2 mouse;
uniform float time;


struct Sphere {
  vec3 position;
  float radius;
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
Material materials[5];
Light lights[2];
Camera camera = Camera(vec3(278, 273, 800));
Scene scene = Scene(vec4(0.2,0.2,0.2,1), vec4(0,0,0,1));

float boxMaxDepth = -559.2;

//ray-sphere intersection
float intersect(Ray ray, Sphere sphere)
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
  float t1 = intersect(ray, spheres[0]);
  float t2 = intersect(ray, spheres[1]);
  float tFloor = -ray.position.y / ray.direction.y;	
  float tCeiling = (550.0-ray.position.y) / ray.direction.y;	
  float tLeft = (-ray.position.x) / ray.direction.x;	
  float tRight = (550.0-ray.position.x) / ray.direction.x;	
  float tBack = (-550.0-ray.position.z) / ray.direction.z;	

  vec3 p = vec3(0,0,0);
  vec3 n = vec3(0,0,0);
  if(!isinf(t1) && (isinf(t2) || t1 < t2))
  {
    p = ray.position + t1*ray.direction;
    n = p - spheres[0].position;
    if(isInShadow(p, spheres[1]))
      return scene.ambient * materials[0].diffuse;
    return blinnPhongShading(p, n, materials[0]);
  }
  else if(!isinf(t2) && (isinf(t1) || t2 < t1))
  {
    p = ray.position + t2*ray.direction;
    n = p - spheres[1].position;
    if(isInShadow(p, spheres[0]))
      return scene.ambient * materials[1].diffuse;   
    return blinnPhongShading(p, n, materials[1]);
  }

  p = ray.position + tFloor * ray.direction;
  if(p.x > 0.0 && p.x < 550.0 && p.z > boxMaxDepth && p.z < 0.0)
  {
    n = vec3(0,1,0);
    if(isInShadow(p, spheres[0]) || isInShadow(p, spheres[1]))
      return scene.ambient * materials[2].diffuse;
    return blinnPhongShading(p, n, materials[2]);  	
  }

  p = ray.position + tCeiling * ray.direction;
  if(p.x > 0.0 && p.x < 550.0 && p.z > boxMaxDepth && p.z < 0.0)
  {
    n = vec3(0,-1,0);
    if(isInShadow(p, spheres[0]) || isInShadow(p, spheres[1]))
      return scene.ambient * materials[2].diffuse;
    return blinnPhongShading(p, n, materials[2]);  	
  }

  p = ray.position + tLeft * ray.direction;
  if(p.y > 0.0 && p.y < 550.0 && p.z > boxMaxDepth && p.z < 0.0)
  {
    n = vec3(1,0,0);
    if(isInShadow(p, spheres[0]) || isInShadow(p, spheres[1]))
      return scene.ambient * materials[3].diffuse;   
    return blinnPhongShading(p, n, materials[3]);  	
  }

  p = ray.position + tRight * ray.direction;
  if(p.y > 0.0 && p.y < 550.0 && p.z > boxMaxDepth && p.z < 0.0)
  {
    n = vec3(-1,0,0);
    if(isInShadow(p, spheres[0]) || isInShadow(p, spheres[1]))
      return scene.ambient * materials[4].diffuse;
    return blinnPhongShading(p, n, materials[4]);  	
  }

  p = ray.position + tBack * ray.direction;
  if(p.y > 0.0 && p.y < 550.0 && p.x > 0.0 && p.x < 550.0)
  {
    n = vec3(0,0,1);
    if(isInShadow(p, spheres[0]) || isInShadow(p, spheres[1]))
      return scene.ambient * materials[2].diffuse;
    return blinnPhongShading(p, n, materials[2]);   	
  }

  return scene.background;
}

void init()
{ 
  spheres[0] = Sphere(vec3(180, 120, -370), 120);
  spheres[1] = Sphere(vec3(420, 100, -130), 100);

  materials[0] = Material(vec4(0.156,0.126,0.507,1), vec4(1,1,1,1), 100);   // Blue specular
  materials[1] = Material(vec4(0.656,0.626,0.107,1), vec4(0,0,0,1), 1);     // Yellow
  materials[2] = Material(vec4(0.739, 0.725, 0.765, 1), vec4(0,0,0,1), 1);  // White
  materials[3] = Material(vec4(0.639, 0.06, 0.062, 1), vec4(0,0,0,1), 1);   // Red
  materials[4] = Material(vec4(0.156, 0.426, 0.107, 1), vec4(0,0,0,1), 1);  // Green

  lights[0] = Light(vec3(0.5,1.0,0.5),  vec4(0.8,0.7,0.6,1));
  lights[1] = Light(vec3(250,250,-250),  vec4(0.8,0.7,0.6,1));
}

void main(void)
{
  //Initialize elements
  init();

  //Interactivity and real time stuff 
  lights[0].position = vec3(2 * mouse.x, 2 * mouse.y, 0.0);  //Y is flipped
  //lights[1].position = vec3(mouse.x, resolution.y - mouse.y, -500.0);  //Y is flipped
  //spheres[0].position.x += 55 * sin(time/10.0);
  //spheres[1].position.z += 50 * cos(time/10.0);

  //Ray definition
  vec3 pixel = vec3(resolution.x * pixelCoords.x, resolution.y * pixelCoords.y, 0);
  Ray ray = Ray(camera.position, normalize(pixel - camera.position));
 
  //Raytrace objects
  pixelColor = rayTrace(ray);  
  //outFragColor = vec4(1,0,0,1);
}
