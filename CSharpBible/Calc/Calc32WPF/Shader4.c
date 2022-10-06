
vec2 hr = vec2(1.,sqrt(3.));

float PI = 3.1415;//atan(-1.,0.);

float hash1d(float st)
{return fract(sin(st)*12.454);}

float hash(vec2 st)
{return fract(sin(dot(st, vec2(12.4,4.14)))*12.44);}

float moda(inout vec2 p, float rep)
{
    float per = 2.*PI/rep;
    float a = atan(p.y,p.x);
    float l = length(p);
    float id = floor((a-per*.5)/per);
    a = mod(a+per*.5,per)-per*.5;
    p = vec2(cos(a),sin(a))*l;
    return id;
    
}

mat2 rot(float a)
{return mat2(cos(a),sin(a),-sin(a),cos(a));}

float stmin(float a,float b, float k, float n){

    float st = k/n;
    float u = b-k;
    return min(min(a,b), .5*(u+a+abs(mod(u-a+st,2.*st)-st)));
}

vec4 hg(vec2 uv,float d)
{
    // Zoom
    uv*= d;
    vec2 ga = mod(uv,hr)-hr*.5,
        gb = mod(uv-hr*.5, hr) -hr*.5;
    vec2 guv = dot(ga,ga) < dot(gb,gb) ? ga:gb;
    vec2 gid = uv-guv;
    
    vec2 gg = abs(guv);
    guv.y = .5-max(gg.x, dot(gg,normalize(hr)));
    return vec4(guv,gid);        
}

vec2 hid;

float dm(vec2 uv)
{
    vec4 hs=hg(uv,3.);
    hid = hs.zw;
    return smoothstep(0.012,0.15,hs.y) * sin(length(hid)-iTime);
}

float od(vec3 p, float d)
{return dot(p,normalize(sign(p)))-d;}

float room(vec3 p)
{
    return -od(p, 9.);
}

float gl = 0.;
float water(vec3 p)
{
    p.y +=2.;
    p.y += dm(p.xz)*.1;
    float d= abs(p.y)-0.5;
    gl += .01/(.01+d);
    return d;
}

vec3 getcam(vec3 ro, vec3 t, vec2 uv)
{
 vec3 f = normalize(t-ro);
    vec3 l = normalize(cross(vec3(0.,1.,0.),f));
    vec3 u = normalize(cross(f,l));
    return normalize(f+l*uv.x+u*uv.y);
}

float cyl(vec3 p, float r, float h)
{
// return length(p)-r;
 return max(length(p.xy)-r, abs(p.z)-h);   
}

float g2 = 0.;
float tid = 0.;
float monster(vec3 p)
{
    float o = od(p + vec3(0.,1.-sin(iTime)*.3,0.) ,.7);
    p.xz *= rot(.8*sin(p.y*.4-iTime*PI*.5));
    tid = moda(p.xz,8.);
    p.x -= 2.;
    float c = cyl(p.xzy,.2-p.y*.08,5.);
    float d= min(c,o);
    g2 += 0.002/(0.01+d);
    return min(o,c);
}

float SDF(vec3 p){
 return stmin(stmin(room(p),water(p),1.,5.),monster(p),.5,3.);
 //   return length(p)-1.;
    }

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
    // Normalized pixel coordinates (from 0 to 1)
    vec2 uv = fragCoord/iResolution.xy - vec2(.5);

    uv /= vec2(iResolution.y/iResolution.x,1.);
    vec3 col = vec3(0.);
    
    vec3 ro;
    ro.xz = vec2(.001,-8.) *rot(iTime*.2);
    ro.y = 1.5+sin(iTime*.5);
    vec3 p = ro;
    vec3 rd = normalize(vec3(uv,1.));
    rd = getcam(ro, vec3(0.), uv);
    
    float shad = 0.;
    
    for (float i=0.; i<100.; i++)
    {
     float d = SDF(p);
         if (d < .001)
         {
             shad = i/100.;
             break;
         }
        p += d*rd;
    }
    
    col = vec3(shad); 
       col+= gl *vec3(hash(hid + iTime*.002),hash(hid *.5 + iTime*.003),1.)*.2;
       col+= g2 * vec3(.8,.5,.5*hash1d(tid*2.))*.3;
        
    // Output to screen
    fragColor = vec4(col,1.);
}