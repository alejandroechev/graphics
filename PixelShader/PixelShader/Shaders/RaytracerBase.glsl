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

const int numberOfSpheres = 1;
const int numLights = 1;
const int numberOfMaterials = 1;
const int numberOfRaysPerPixelRoot = 5;

Sphere spheres[numberOfSpheres];
Material materials[numberOfMaterials];
Light lights[numLights];
Camera camera = Camera(vec3(0.5, 0.5, 2.0));
Scene scene = Scene(vec4(0.1,0.1,0.1,1), vec4(0,0,0,1));


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

//shadows
bool isInShadow(vec3 p, Sphere sphere, Light light)
{  
  float lightDistance = distance(light.position, p);
  vec3 shadowDir = normalize(light.position - p);
  Ray shadowRay = Ray(p + 0.1 * shadowDir, shadowDir);    
  float tShadow = intersect(shadowRay, sphere);
  if(!isinf(tShadow) && tShadow < lightDistance)
	return true;
  
  return false;
}

bool isInOtherSphereShadow(vec3 p, Sphere thisSphere, Light light)
{
	for(int i=0; i<numberOfSpheres; i++)
	{
		if(isInShadow(p, spheres[i], light))
			return true;
	}
	return false;
}

//blinn-phong shading
vec4 blinnPhongShading(vec3 p, vec3 n, Sphere sphere)
{
  Material material	= materials[sphere.materialIndex];
  vec3 v = camera.position - p;
  v = normalize(v);
 
  vec4 shadedColor = scene.ambient * material.diffuse;
  for(int i=0; i<numLights; i++)
  {
	vec4 lightColor = lights[i].color;
	if(isInOtherSphereShadow(p, sphere, lights[i]))
		lightColor = vec4(0,0,0,1);
    vec3 l = lights[i].position - p;
    l = normalize(l);  
    vec3 h = v + l;
    h = normalize(h);

    shadedColor = shadedColor + lightColor * (max(0,dot(n,l)) * material.diffuse + pow(max(0,dot(n,h)), material.shininess) * material.specular);	  
  }
  return shadedColor;
}  



vec4 rayTrace(Ray ray)
{
  vec4 accumulatedColor = vec4(0,0,0,1);
  float frac = 1.0;
  
	  float tMin = infinity;
	  int sphereMin = -1;
	  for(int i=0; i<numberOfSpheres; i++)
	  {	
		float t = intersect(ray, spheres[i]);
		if(t < tMin)
		{
			tMin = t;
			sphereMin = i;
		}
	  }

	  if(!isinf(tMin))
	  {
		vec3 p = ray.position + tMin*ray.direction;
		vec3 n = normalize(p - spheres[sphereMin].position);
		Material mat = materials[spheres[sphereMin].materialIndex];
		vec4 localColor = blinnPhongShading(p, n, spheres[sphereMin]);
		accumulatedColor += localColor * frac;
	  }
	  
	 
	  accumulatedColor += scene.background * frac;
  
  return accumulatedColor;
}

void init()
{ 
  materials[0] = Material(vec4(0.639, 0.06, 0.062, 1), vec4(1,1,1,1), 100);   // Red
  
  lights[0] = Light(vec3(0.5,1.0,0.5),  vec4(0.8,0.7,0.6,1));
  
  spheres[0] = Sphere(vec3(0.5, 0.5, -2), 0.3, 0); //Blue sphere
  
}

void main(void)
{
  //Initialize elements
  init();
    
  vec3 color = vec3(0,0,0);
  float delta = 1.0 / resolution.x / (numberOfRaysPerPixelRoot + 1);
  float rayTime = 0.0;
  float exposureTime = 0.2;
  float deltaTime = exposureTime / (numberOfRaysPerPixelRoot * numberOfRaysPerPixelRoot);
  float speed = 3;
  for(int i=0; i<numberOfRaysPerPixelRoot; i++)
  {
	for(int j=0; j<numberOfRaysPerPixelRoot; j++)
	{ 
		  //Ray definition
		  vec3 pixel = vec3(pixelCoords.x + i*delta, pixelCoords.y + j*delta, 0);
		  Ray ray = Ray(camera.position, normalize(pixel - camera.position));
		  		  
		  spheres[0].position = vec3(0.5, 0.5, -2) + 0.4 * vec3(sin(speed*(time + rayTime)),0,0);
		   	
		  //Raytrace objects
		  color += rayTrace(ray).rgb;
		  rayTime += deltaTime;
	}
  }  
  pixelColor = vec4(color/(numberOfRaysPerPixelRoot*numberOfRaysPerPixelRoot),1);
}
