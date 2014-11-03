#version 130

precision highp float;

struct Light {
  vec3 position;
  vec3 color;
};
uniform Light light0;

uniform vec3 backgroundColor;
uniform vec3 ambientColor;

uniform vec3 eye;

uniform sampler2D diffuseTexture;
uniform sampler2D normalMap;

mat3 cotangent_frame(vec3 N, vec3 p, vec2 uv)
{
  // get edge vectors of the pixel triangle
  vec3 dp1 = dFdx( p );
  vec3 dp2 = dFdy( p );
  vec2 duv1 = dFdx( uv );
  vec2 duv2 = dFdy( uv );

  // solve the linear system
  vec3 dp2perp = cross( dp2, N );
  vec3 dp1perp = cross( N, dp1 );
  vec3 T = dp2perp * duv1.x + dp1perp * duv2.x;
  vec3 B = dp2perp * duv1.y + dp1perp * duv2.y;

  // construct a scale-invariant frame 
  float invmax = inversesqrt( max( dot(T,T), dot(B,B) ) );
  return mat3( T * invmax, B * invmax, N );
}

vec3 perturb_normal( vec3 N, vec3 V, vec2 texcoord )
{
  // assume N, the interpolated vertex normal and 
  // V, the view vector (vertex to eye)
  vec3 map = texture(normalMap, texcoord ).xyz;
  map = map * 255./127. - 128./127.;
  mat3 TBN = cotangent_frame(N, -V, texcoord);
  return normalize(TBN * map);
}

vec3 blinnPhongShading(vec3 p, vec3 n, vec3 diffuse, vec3 specular, float shininess, vec3 lightPosition, vec3 lightColor, vec2 texCoords)
{
  vec3 v = eye - p;
  v = normalize(v);
  n =  perturb_normal(n, v, texCoords);
  vec3 l = lightPosition - p;
  l = normalize(l);
  vec3 h = v + l;
  h = normalize(h); 
  vec3 shadedColor = lightColor * (diffuse * max(dot(n,l),0.0) + specular * pow(max(dot(n,h),0.0), shininess));  
  return shadedColor;
}  

vec3 shade(vec3 p, vec3 n, vec3 diffuse, vec3 specular, float shininess, vec2 texCoords)
{
  vec3 color = blinnPhongShading(p,n,diffuse, specular, shininess, light0.position, light0.color, texCoords); 
  return ambientColor * diffuse + color;
}  

in vec3 fragPosition;
in vec3 fragDiffuseColor;
in vec3 fragNormal;
in vec3 fragSpecularColor;
in vec3 fragTexCoords;

out vec4 outFragColor;

void main(void)
{
  vec4 texColor = texture(diffuseTexture, fragTexCoords.xy);
  outFragColor = vec4(shade(fragPosition, fragNormal, texColor.xyz * fragDiffuseColor, fragSpecularColor, fragTexCoords.z, fragTexCoords.xy), 1.0);
}