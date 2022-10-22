// ***********************************************************************
// Assembly         : Calc32WPF_net
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 04-06-2020
// ***********************************************************************
// <copyright file="sound1.c" company="Calc32WPF_net">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
float patlength = 1.;
float pi2 = 6.2831;

vec2 sfilter;


float melody1(float note,float patt)
{
    float fltt = (.5-.5*sin(floor(note*2.)*pi2*.125))*(5.+floor(patt *.5));
    return  exp(floor((floor(fltt)+.25+.25*cos(patt*pi2*.125))*12./8.)/24./log(2.));
}

vec2 mainSound( float time )
{
    float pattime = mod(time, patlength);
    vec2 sound = vec2(0);
 
    float pattime2 = mod(time/patlength, patlength*12.);
    
    float flst = melody1(pattime*6.,pattime2);
     
    float sharp = .2-.05*cos(pattime*12.*pi2);
    sound += smoothstep(-sharp,sharp, sin(pi2*440.0*flst*time +vec2(sin(time*3.),cos(time*2.)))-.5)*exp(-2.0*pattime)*.2 ; 

    float flst2 = melody1(pattime*2.,pattime2);
    
    float sharp2 = .42-.3*cos(pattime*4.*pi2);
    sound += (smoothstep(-sharp2,sharp2, sin(pi2*220.0*flst2*time ))*.1)-.5;

    float flst3 = melody1(pattime,pattime2);
    
    float sharp3 = .22+.2*sin(pattime*2.*pi2);
    sound += smoothstep(-sharp3,sharp3,  sin(pi2*55.*flst3*time +vec2(sin(time*1.5)*2.,cos(time*1.5)*2.) )-.5)*exp(-2.0*pattime)*.25;
    
    /*
    float fltt = floor(time*4.)*2.9;
    float flst = .6+.2*sin(fltt);
    sfilter = sfilter *flst + sound*(1.-flst);
    */
     
    
    // A 440 Hz wave that attenuates quickly overt time
    return sound;
} 