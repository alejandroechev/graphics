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

Sphere spheres[1];
Material materials[6];
Light lights[1];
Camera camera = Camera(vec3(0, 10, -10));
Scene scene = Scene(vec4(0.2,0.2,0.2,1), vec4(0.8,0.8,1.0,1));

float deltaT = 0.01;
float minT = 5;
float maxT = 15.0;

vec4 mod289(vec4 x)
{
  return x - floor(x * (1.0 / 289.0)) * 289.0;
}

vec4 permute(vec4 x)
{
  return mod289(((x*34.0)+1.0)*x);
}

vec4 taylorInvSqrt(vec4 r)
{
  return 1.79284291400159 - 0.85373472095314 * r;
}

vec2 fade(vec2 t) {
  return t*t*t*(t*(t*6.0-15.0)+10.0);
}

// Classic Perlin noise
float noise(vec2 P)
{
  vec4 Pi = floor(P.xyxy) + vec4(0.0, 0.0, 1.0, 1.0);
  vec4 Pf = fract(P.xyxy) - vec4(0.0, 0.0, 1.0, 1.0);
  Pi = mod289(Pi); // To avoid truncation effects in permutation
  vec4 ix = Pi.xzxz;
  vec4 iy = Pi.yyww;
  vec4 fx = Pf.xzxz;
  vec4 fy = Pf.yyww;

  vec4 i = permute(permute(ix) + iy);

  vec4 gx = fract(i * (1.0 / 41.0)) * 2.0 - 1.0 ;
  vec4 gy = abs(gx) - 0.5 ;
  vec4 tx = floor(gx + 0.5);
  gx = gx - tx;

  vec2 g00 = vec2(gx.x,gy.x);
  vec2 g10 = vec2(gx.y,gy.y);
  vec2 g01 = vec2(gx.z,gy.z);
  vec2 g11 = vec2(gx.w,gy.w);

  vec4 norm = taylorInvSqrt(vec4(dot(g00, g00), dot(g01, g01), dot(g10, g10), dot(g11, g11)));
  g00 *= norm.x;  
  g01 *= norm.y;  
  g10 *= norm.z;  
  g11 *= norm.w;  

  float n00 = dot(g00, vec2(fx.x, fy.x));
  float n10 = dot(g10, vec2(fx.y, fy.y));
  float n01 = dot(g01, vec2(fx.z, fy.z));
  float n11 = dot(g11, vec2(fx.w, fy.w));

  vec2 fade_xy = fade(Pf.xy);
  vec2 n_x = mix(vec2(n00, n01), vec2(n10, n11), fade_xy.x);
  float n_xy = mix(n_x.x, n_x.y, fade_xy.y);
  return 2.3 * n_xy;
}

float sinusoidal(vec2 p)
{
  return sin(p.x + 0.1*time) * sin(p.y + 0.1*time);
  
}

float fbm(vec3 coords, int octaves, float lacunarity, float gain)
{
  float sum = 0.0;
  float amp = 1.0;
  
  int i;

  for(i = 0; i < octaves; i+=1)
  {
    amp *= gain; 
    sum += amp * sinusoidal(coords.xz);
    coords *= lacunarity;
  }
  return sum;

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

float f(vec3 p)
{
  return fbm(p, 5,2.4,0.4);
  //return 0.2*sinusoidal(5*p.xz);
  //return 0;
}



vec3 getNormal( vec3 p )
{
  float dx = f(vec3(p.x-eps,p.yz)) - f(vec3(p.x+eps,p.yz));
  float dy = 2 * eps;//f(vec3(p.x, p.y-eps,p.z)) - f(vec3(p.x,p.y+eps,p.z)); 
  float dz = f(vec3(p.xy,p.z-eps)) - f(vec3(p.xy,p.z+eps)); 
  return normalize( vec3(dx,dy,dz) );
}



float intersectFunction(Ray ray)
{  
  for(float t = minT; t < maxT; t += deltaT)
  {
    vec3 p = ray.position + t*ray.direction;
    float fValue = f(p);
    if(p.y < fValue)
    {
      return t;
    }
  }
  return infinity;
}

//Interpolate two last t
float intersectFunction2(Ray ray)
{
  float lastFValue = 0.0;
  float lastYValue = 0.0;
  for(float t = minT; t < maxT; t += deltaT)
  {
    vec3 p = ray.position + t*ray.direction;
    float fValue = f(p);
    if(p.y < fValue)
    {
      return t - deltaT + deltaT * (lastFValue - lastYValue)/(lastFValue - lastYValue + p.y - fValue);
    }
    lastFValue = fValue;
    lastYValue = p.y;
  }
  return infinity;
}

//Dynamic dt
float intersectFunction3(Ray ray)
{
  float lastFValue = 0.0;
  float lastYValue = 0.0;
  float dynamicDeltaT = deltaT;
  for(float t = minT; t < maxT; t += dynamicDeltaT)
  {
    vec3 p = ray.position + t*ray.direction;
    float fValue = f(p);
    if(p.y < fValue)
    {
      return t - dynamicDeltaT + dynamicDeltaT * (lastFValue - lastYValue)/(lastFValue - lastYValue + p.y - fValue);
    }
    lastFValue = fValue;
    lastYValue = p.y;
    dynamicDeltaT = 0.01*t;
  }
  return infinity;
}

vec4 rayMarch(Ray ray, Material mat)
{
  float t1 = intersectFunction3(ray);  
  if(!isinf(t1))
  {
    vec3 p = ray.position + t1*ray.direction;
    vec3 n = getNormal(p);
    //if(p.y + noise(p.xz) > 0.7)
    //  return blinnPhongShading(p, n, materials[2]);
    //else if(p.y < -0.2)
    //  return blinnPhongShading(p, n, materials[0]);
    //else
    return blinnPhongShading(p, n, materials[0]);

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
}

void main(void)
{
  //Initialize elements
  init();

  //Interactivity and real time stuff
  vec2 adjustedMouse =  vec2((2.0 * mouse.x / resolution.x) - 1.0, (2.0 * (resolution.y - mouse.y) / resolution.y) - 1.0);
  lights[0].position = vec3(adjustedMouse.x, 2.0, adjustedMouse.y);  //Y is flipped
  camera.position = vec3(0, 2.0, -10.0);

  //Ray definition
  vec3 pixel = vec3(2.0*pixelCoords.x - 1.0, 2.0*pixelCoords.y - 1.0, 0);
  Ray ray = Ray(camera.position, normalize(pixel - camera.position));

  //Raytrace objects
  pixelColor = rayMarch(ray, materials[5]);  
  //float color = (1.0 + fbm(pixelCoords, 10,2.5,0.4))/1.0; 
  //pixelColor = color*vec4(1);
}
