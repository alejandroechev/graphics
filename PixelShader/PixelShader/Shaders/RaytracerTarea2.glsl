#version 330

precision highp float;

in vec2 pixelCoords;
out vec4 pixelColor;

const float infinity = 1. / 0.;

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
  float reflectivity;
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

const int numberOfSpheres = 8;
const int numLights = 2;
const int numberOfMaterials = 5;
const int maxNumberOfReflections = 2;

Sphere spheres[numberOfSpheres];
Material materials[numberOfMaterials];
Light lights[numLights];
Camera camera = Camera(vec3(0.5, 0.5, 1.5));
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
  for(int i=0; i < maxNumberOfReflections + 1; i++)
  {
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
		if(mat.reflectivity > 0)
		{
			ray.position = p;
			ray.direction = normalize(reflect(ray.direction, n));
			frac *= mat.reflectivity;
		}
		else
		{
			break;
		}
	  }  
	 
	  accumulatedColor += scene.background * frac;
  }
  return accumulatedColor;
}

void init()
{ 
  materials[0] = Material(vec4(0.156,0.126,0.507,1), vec4(1,1,1,1), 100, 0.3);   // Blue specular
  materials[1] = Material(vec4(0.656,0.626,0.107,1), vec4(0,0,0,1), 1, 0.5);     // Yellow
  materials[2] = Material(vec4(0.739, 0.725, 0.765, 1), vec4(0,0,0,1), 1, 0.5);  // White
  materials[3] = Material(vec4(0.639, 0.06, 0.062, 1), vec4(0,0,0,1), 1, 0.5);   // Red
  materials[4] = Material(vec4(0.156, 0.426, 0.107, 1), vec4(0,0,0,1), 1, 0.5);  // Green

  lights[0] = Light(vec3(0.5,0.99,0.5),  vec4(0.8,0.7,0.6,1));
  lights[1] = Light(vec3(0.5,0.99,-0.5),  vec4(0.2,0.2,0.2,1));

  spheres[0] = Sphere(vec3(0.35, 0.24, -0.72), 0.2, 0); //Blue sphere
  spheres[1] = Sphere(vec3(0.82, 0.2, -0.25), 0.19, 1); //Yellow sphere
  spheres[2] = Sphere(vec3(0.5, 0.5, -1001), 1000, 2); //Back wall
  spheres[3] = Sphere(vec3(0.5, 1001.1, -0.5), 1000, 2); //Ceiling
  spheres[4] = Sphere(vec3(0.5, -1000.1, -0.5), 1000, 2); //Floor
  spheres[5] = Sphere(vec3(1001.1, 0.5, -0.5), 1000, 4); //Right wall
  spheres[6] = Sphere(vec3(-1000, 0.5, -0.5), 999.9, 3); //Left wall
  spheres[7] = Sphere(vec3(0.5, 0.7, -0.5), 0.03, 2); //White sphere
  
}

void main(void)
{
  //Initialize elements
  init();

  //Light movement 
  lights[0].position = vec3(mouse.x, mouse.y, 0.0); 
		  		  
  //Object movement
  float speed = 1;
  spheres[0].position = vec3(0.5, 0.24, -0.5) + 0.3 * vec3(sin(speed*(time)),0,cos(speed*(time)));
  spheres[1].position = vec3(0.5, 0.2, -0.5) + 0.23 * vec3(sin(speed*(time) + 10),0,cos(speed*(time)+ 10));
  spheres[7].position = vec3(0.5, 0.7, -0.5) + 0.2 * vec3(0, sin(speed*(time)), 0);
		   	
  //Raytrace objects
  vec3 pixel = vec3(pixelCoords, 0);
  Ray ray = Ray(camera.position, normalize(pixel - camera.position));
  pixelColor = rayTrace(ray);
}
