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

// Created by inigo quilez - iq/2013
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

//stereo thanks to Croqueteer
//#define STEREO 

float hash( float n )
{
  return fract(sin(n)*43758.5453123);
}

vec3 noised( in vec2 x )
{
  vec2 p = floor(x);
  vec2 f = fract(x);

  vec2 u = f*f*(3.0-2.0*f);

  float n = p.x + p.y*57.0;

  float a = hash(n+  0.0);
  float b = hash(n+  1.0);
  float c = hash(n+ 57.0);
  float d = hash(n+ 58.0);
  return vec3(a+(b-a)*u.x+(c-a)*u.y+(a-b-c+d)*u.x*u.y,
    30.0*f*f*(f*(f-2.0)+1.0)*(vec2(b-a,c-a)+(a-b-c+d)*u.yx));

}

float noise( in vec2 x )
{
  vec2 p = floor(x);
  vec2 f = fract(x);

  f = f*f*(3.0-2.0*f);

  float n = p.x + p.y*57.0;

  float res = mix(mix( hash(n+  0.0), hash(n+  1.0),f.x),
    mix( hash(n+ 57.0), hash(n+ 58.0),f.x),f.y);

  return res;
}

const mat2 m2 = mat2(1.6,-1.2,1.2,1.6);

float fbm( vec2 p )
{
  float f = 0.0;

  f += 0.5000*noise( p ); p = m2*p*2.02;
  f += 0.2500*noise( p ); p = m2*p*2.03;
  f += 0.1250*noise( p ); p = m2*p*2.01;
  f += 0.0625*noise( p );

  return f/0.9375;
}

float terrain( in vec2 x )
{
  vec2  p = x*0.003;
  float a = 0.0;
  float b = 1.0;
  vec2  d = vec2(0.0);
  for( int i=0; i<6; i++ )
  {
    vec3 n = noised(p);
    d += n.yz;
    a += b*n.x/(1.0+dot(d,d));
    b *= 0.5;
    p = m2*p;
  }

  return 140.0*a;
}

float terrain2( in vec2 x )
{
  vec2  p = x*0.003;
  float a = 0.0;
  float b = 1.0;
  vec2  d = vec2(0.0);
  for( int i=0; i<14; i++ )
  {
    vec3 n = noised(p);
    d += n.yz;
    a += b*n.x/(1.0+dot(d,d));
    b *= 0.5;
    p=m2*p;
  }

  return 140.0*a;
}

float terrain3( in vec2 x )
{
  vec2  p = x*0.003;
  float a = 0.0;
  float b = 1.0;
  vec2  d = vec2(0.0);
  for( int i=0; i<4; i++ )
  {
    vec3 n = noised(p);
    d += n.yz;
    a += b*n.x/(1.0+dot(d,d));
    b *= 0.5;
    p = m2*p;
  }

  return 140.0*a;
}

float map( in vec3 p )
{
  float h = terrain(p.xz);
  return p.y - h;
}

float map2( in vec3 p )
{
  float h = terrain2(p.xz);
  return p.y - h;
}

float interesct( in vec3 ro, in vec3 rd )
{
  float h = 1.0;
  float t = 1.0;
  for( int i=0; i<128; i++ )
  {
    if( h<0.1 || t>2000.0 ) break;
    t += 0.5*h*(1.0+0.0002*t);
    h = map( ro + t*rd );
  }

  if( h>10.0 ) t = -1.0;
  return t;
}

float sinteresct(in vec3 rO, in vec3 rD )
{
  float res = 1.0;
  float t = 0.0;
  for( int j=0; j<50; j++ )
  {
    //if( t>1000.0 ) break;
    vec3 p = rO + t*rD;

    float h = map( p );

    res = min( res, 16.0*h/t );
    t += h;

  }

  return clamp( res, 0.0, 1.0 );
}

vec3 calcNormal( in vec3 pos, float t )
{
  float e = 0.001;
  e = 0.001*t;
  vec3  eps = vec3(e,0.0,0.0);
  vec3 nor;
  nor.x = map2(pos+eps.xyy) - map2(pos-eps.xyy);
  nor.y = map2(pos+eps.yxy) - map2(pos-eps.yxy);
  nor.z = map2(pos+eps.yyx) - map2(pos-eps.yyx);
  return normalize(nor);
}

vec3 camPath( float time )
{
  vec2 p = 600.0*vec2( cos(1.4+0.37*time), 
    cos(3.2+0.31*time) );

  return vec3( p.x, 0.0, p.y );
}

void main(void)
{
  vec2 xy = pixelCoords;

  vec2 s = xy*vec2(1.75,1.0);

  float rtime = time*.15 + 4.0*mouse.x/resolution.x;

  vec3 light1 = normalize( vec3(  0.4, 0.22,  0.6 ) );


  vec3 ro = camPath( rtime );
  vec3 ta = camPath( rtime + 3.0 );
  ro.y = terrain3( ro.xz ) + 11.0;
  ta.y = ro.y - 100.0;

  float cr = 0.2*cos(0.1*rtime);
  vec3  cw = normalize(ta-ro);
  vec3  cp = vec3(sin(cr), cos(cr),0.0);
  vec3  cu = normalize( cross(cw,cp) );
  vec3  cv = normalize( cross(cu,cw) );
  vec3  rd = normalize( s.x*cu + s.y*cv + 1.5*cw );



  float sundot = clamp(dot(rd,light1),0.0,1.0);
  vec3 col;
  float t = interesct( ro, rd );
  if( t<0.0 )
  {
    col = vec3(0.85,.95,1.0)*(1.0-0.5*rd.y);
    col += 0.25*vec3(1.0,0.8,0.4)*pow( sundot,12.0 );

  }
  else
  {
    vec3 pos = ro + t*rd;

    vec3 nor = calcNormal( pos, t );

    float r = noise( 7.0*pos.xz );

    col = (r*0.25+0.75)*0.9*mix( vec3(0.08,0.05,0.03), vec3(0.13,0.10,0.08), clamp(terrain2( vec2(pos.x,pos.y*48.0))/200.0,0.0,1.0) );
    col = mix( col, 0.15*vec3(0.45,.23,0.04)*(0.50+0.50*r),smoothstep(0.70,0.9,nor.y) );
    col = mix( col, 0.10*vec3(0.30,.30,0.00)*(0.25+0.75*r),smoothstep(0.95,1.0,nor.y) );
    col *= 0.9;
    // snow
    float h = smoothstep(55.0,80.0,pos.y + 25.0*fbm(0.01*pos.xz) );
    float e = smoothstep(1.0-0.5*h,1.0-0.1*h,nor.y);
    float o = 0.3 + 0.7*smoothstep(0.0,0.1,nor.x+h*h);
    float s = h*e*o;
    col = mix( col, 0.35*vec3(0.62,0.65,0.7), smoothstep( 0.1, 0.9, s ) );

    // lighting		
    float amb = clamp(0.5+0.5*nor.y,0.0,1.0);
    float dif = clamp( dot( light1, nor ), 0.0, 1.0 );
    float bac = clamp( 0.2 + 0.8*dot( normalize( vec3(-light1.x, 0.0, light1.z ) ), nor ), 0.0, 1.0 );
    float sh = 1.0; if( dif>=0.0001 ) sh = sinteresct(pos+light1*20.0,light1);

    vec3 lin  = vec3(0.0);
    lin += dif*vec3(8.00,5.50,3.00)*vec3( sh, sh*sh*0.5+0.5*sh, sh*sh*0.8+0.2*sh );
    lin += amb*vec3(0.30,0.50,0.60);
    lin += bac*vec3(0.60,0.60,0.60);

    col *= lin;

    float fo = 1.0-exp(-0.0007*t);
    vec3 fco = vec3(0.55,0.65,0.75) + 0.6*vec3(1.0,0.8,0.5)*pow( sundot, 4.0 );
    col = mix( col, fco, fo );

    col += 0.4*vec3(1.0,0.8,0.4)*pow( sundot, 8.0 )*(1.0-exp(-0.005*t));
  }


  col = pow(col,vec3(0.45));

  col = col*0.6 + 0.4*col*col*(3.0-2.0*col);

  vec2 uv = xy*0.5+0.5;
  col *= 0.7 + 0.3*pow(16.0*uv.x*uv.y*(1.0-uv.x)*(1.0-uv.y),0.1);


  pixelColor=vec4(col,1.0);
}
