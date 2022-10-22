// ***********************************************************************
// Assembly         : Calc32WPF_net
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 04-06-2020
// ***********************************************************************
// <copyright file="Shader3.c" company="Calc32WPF_net">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
    // Normalized pixel coordinates (from -1 to 1)
    vec2 uv = (fragCoord-.5*iResolution.xy)/iResolution.y;
	
	vec3 col = vec3(0);
	
	vec2 gv = fract( uv*5. );
	
	col.rg = gv;
	
	fragColor = vec4(col,1.);
}